using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace m.transport.Interfaces
{
	public interface ILoadAndSaveFiles
	{
		bool FileExists(string fileName);
		void SaveText(string filename, string text);
		string LoadText(string filename);
		void SaveBinary(string filename, byte[] bytes);
		byte[] LoadBinary(string filename);
		string GetFilePath(string filename);
		void Delete(string filename);
		Stream GetReadFileStream(string path);
		void DeleteFile(string path);
	}
}
