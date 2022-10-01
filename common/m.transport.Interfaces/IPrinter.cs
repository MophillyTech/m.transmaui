using System;

namespace m.transport.Interfaces
{
	public interface IPrinter
	{
		bool IsPrinterAvailable ();
		void PrintText(string text, bool DoubleHeight = false, bool NextLine = true, bool IsBold = false, int margin = 0);
		void PrintBarcode(string text);
		void SkipLines(int count);
		void PrintSignature (string path, bool type);
		void PrintLogo(int n = 0);
		void PrintData();
		void PrintReceipt (byte[] data, byte[] index);
		void Clear();
		byte[] GetData();
		void Close();
		int Length();
	}
}

