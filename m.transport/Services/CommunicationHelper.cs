using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Binding = System.ServiceModel.Channels.Binding;
//using Binding = Microsoft.Maui.Controls.Binding;

namespace m.transport.Services
{
	/// <summary>
	/// Helper class that is used to wrap client communication via WCF
	/// </summary>
	/// <typeparam name="T">the interface class being wrapped</typeparam>
	public class CommunicationHelper<T> : ICommunicationHelper<T>
	 where T : class
	{
		// Communication channel factory.
		ChannelFactory<T> _factory;

		// The contract interface
		T _channel;

		readonly object _lockObject = new object();
		protected object LockObject
		{
			get { return _lockObject; }
		}

		// exposed as property so that it can be mocked in tests
		public T Channel
		{
			get
			{
				lock(_lockObject)
				{
					return _channel;
				}
			}
			set
			{
				lock(_lockObject)
				{
					_channel = value;
				}
			}
		}

		// exposed as property so that it can be mocked in tests
		public ChannelFactory<T> Factory
		{
			get
			{
				lock(_lockObject)
				{
					return _factory;
				}
			}
		}

		public CommunicationHelper(Binding binding, EndpointAddress endpointAddress)
		{
			_factory = new ChannelFactory<T>(binding, endpointAddress);
		}

		public CommunicationHelper(string channelName)
		{
			_factory = new ChannelFactory<T>(channelName);
		}

		public IClientChannel ClientChannel
		{
			get
			{
				lock(_lockObject)
				{
					return (IClientChannel)_channel;
				}
			}
		}

		public string Address
		{
			get
			{
				lock(_lockObject)
				{
					return _factory.Endpoint.Address.ToString();
				}
			}
		}

		public void EnsureChannelCreated()
		{
			lock(_lockObject)
			{
				EnsureChannelCreatedInternal();
			}
		}

		// must be called from within a lock
		internal protected void EnsureChannelCreatedInternal()
		{
			if ((_channel == null) ||
			 (((IClientChannel)_channel).State == CommunicationState.Faulted) ||
			 (((IClientChannel)_channel).State == CommunicationState.Closed)
			 )
			{

				_channel = _factory.CreateChannel(_factory.Endpoint.Address);
				((IClientChannel)_channel).Open();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if (!disposing) return;
			if (_lockObject == null) return;
			lock(_lockObject)
			{
				if (_factory == null) return;
				_factory.Close();
				_factory = null;
			}
		}

		public object CallService(Func<T, object> handler)
		{
			return CallServiceInternal(handler);
		}

		public void CallService(Action<T> handler)
		{
			CallServiceInternal(handler);
		}

		public virtual object CallServiceInternal(Delegate handler)
		{
			if (handler == null) throw new ArgumentNullException("handler");
			object returnObj;
			try
			{
				try
				{
					T localChannel;
					lock(_lockObject)
					{
						EnsureChannelCreatedInternal();
						localChannel = _channel;
					}
					returnObj = handler.DynamicInvoke(localChannel);
				}
				catch (TargetInvocationException e)
				{
					throw e.InnerException;
				}
			}
			catch (FaultException)
			{
				throw;
			}
			catch (CommunicationException e)
			{
				throw new CommunicationHelperException(e.Message, e);
			}
			catch (TimeoutException e)
			{
				throw new CommunicationHelperException(e.Message, e);
			}
			return returnObj;
		}
	}
}
