using System;
using System.Collections.Generic;
using m.transport.Interfaces;
using m.transport.Domain;
using SQLite.Net;
using SQLite.Net.Interop;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace m.transport.Data
{
	public class SqlLiteRepository<T, TKey> : IRepository<T, TKey>
		where T : class, IHaveId<TKey>, new()
	{
		public SqlLiteRepository()
		{
			using (var conn = CreateConnection())
			{
				conn.CreateTable<T>();
			}
		}

		public T GetById(TKey id)
		{
			using (var conn = CreateConnection())
			{
				Func<T> method = () => conn.Find<T>(id);
				return (T)Execute(method);
			}
		}

		public IEnumerable<T> GetAll()
		{
			using (var conn = CreateConnection())
			{
				Func<IEnumerable<T>> method = () => conn.Table<T>().ToList();
				return (IEnumerable<T>)Execute(method);
			}
		}

		public void Save(T item)
		{
			using (var conn = CreateConnection())
			{
				if ((item.Id is int) && ( item.Id.ToString() == "0" )) {
					conn.Insert (item);
				}

				Action method = () => conn.InsertOrReplace(item);
				Execute(method);
			}
		}

		public void SaveAll(IEnumerable<T> items)
		{
			using (var conn = CreateConnection())
			{
				foreach (var item in items)
				{
					var localItem = item;
					Action method = () => conn.InsertOrReplace(localItem);
					Execute(method);
				}

			}
		}

		public void Delete(T item)
		{
			using (var conn = CreateConnection())
			{
				Action method = () => conn.Delete<T>(item.Id);
				Execute(method);
			}
		}

		public void DeleteAll()
		{
			using (var conn = CreateConnection())
			{
				Action method = () => conn.DeleteAll<T>();
				Execute(method);
			}
		}


		private object Execute(Delegate handler)
		{
			int numTries = 0;
			do
			{
				try
				{
					//Debug.WriteLine("Invoking Handler");
					var result = handler.DynamicInvoke();
					//Debug.WriteLine("Result {0}", result);
					return result;
				}
				catch (TargetInvocationException ex)
				{
					if (ex.InnerException is SQLiteException)
					{
						Debug.WriteLine(ex.InnerException.Message);
						Task.Delay(numTries * 1000).Wait();
						numTries++;
						//Insights.Report(ex.InnerException, ReportSeverity.Warning);
					}
					else
					{
						Debug.WriteLine("In the Else:" + ex.InnerException.Message);
						return null;
					}
				}
			} while (numTries < 5);

			Debug.WriteLine("numTries == 5, returning null");
			return null;
		}
		static SQLiteConnection CreateConnection()
		{
			return new SQLiteConnection(App.SQLitePlatform, App.DBPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, false);
		}

		//		protected void CheckConnection()
		//		{
		//			if (conn == null)
		//			{
		//				conn = new SQLiteConnection(App.SQLitePlatform,App.DBPath,SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,false);
		//			}
		//		}
		//
		//		public void Dispose()
		//		{
		//			if (conn != null)
		//			{
		//				conn.Dispose();
		//			}	conn.CreateTable<T>();
		//
		//		}
	}
}

