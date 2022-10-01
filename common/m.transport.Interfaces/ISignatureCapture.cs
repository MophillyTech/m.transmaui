using System;

namespace m.transport.Interfaces
{
	public class Signature
	{
		public string Filename { get; set; }

		public byte[] Bytes { get; set; }
	}

	public interface ISignatureCapture
	{
		void GetSignature(ILoadAndSaveFiles fileRepo, string userName, Action onStoredSignature, Action<Signature> onSuccess, Action<Exception> onError = null);
	}
}

