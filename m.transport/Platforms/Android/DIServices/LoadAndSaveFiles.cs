using System;
using System.IO;
using m.transport.Android.DIServices;
using m.transport.Interfaces;
using Android.Runtime;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(LoadAndSaveFiles))]
namespace m.transport.Android.DIServices
{
	public class LoadAndSaveFiles : ILoadAndSaveFiles
	{
		string documentsPath;

		public LoadAndSaveFiles()
		{
			documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}

		public void SaveText(string filename, string text)
		{
			File.WriteAllText(GetFilePath(filename), text);
		}
		public string LoadText(string filename)
		{
			return File.ReadAllText(GetFilePath(filename));
		}
		public void SaveBinary(string filename, byte[] bytes)
		{
			File.WriteAllBytes(GetFilePath(filename), bytes);
		}
		public byte[] LoadBinary(string filename)
		{
			return File.ReadAllBytes(GetFilePath(filename));
		}

		public bool FileExists(string fileName)
		{
			return File.Exists(GetFilePath(fileName));
		}

		public string GetFilePath(string filename)
		{
			return Path.Combine(documentsPath, filename);
		}

		public void Delete(string filename)
		{
			File.Delete(GetFilePath(filename));
		}

		public Stream GetReadFileStream(string path) {

			MemoryStream ms = new MemoryStream ();

			using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				byte[] bytes = new byte[file.Length];
				file.Read(bytes, 0, (int)file.Length);
				ms.Write(bytes, 0, (int)file.Length);
			}

			ms.Position = 0;

			return ms;
		}

		public void DeleteFile(string path) {
			File.Delete (path);
		}

	}
}