using System;
using System.IO;
using m.transport.Interfaces;
using m.transport.iOS.DIServices;
using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(LoadAndSaveFiles))]
namespace m.transport.iOS.DIServices
{
	public class LoadAndSaveFiles : ILoadAndSaveFiles
	{
		public void SaveText(string filename, string text)
		{
			var filePath = GetFilePath(filename);
			File.WriteAllText(filePath, text);
		}

		public string LoadText(string filename)
		{
			var filePath = GetFilePath(filename);
			return File.ReadAllText(filePath);
		}
		public void SaveBinary(string filename, byte[] bytes)
		{
			var filePath = GetFilePath(filename);
			File.WriteAllBytes(filePath, bytes);
		}
		public byte[] LoadBinary(string filename)
		{
			var filePath = GetFilePath(filename);
			return File.ReadAllBytes(filePath);
		}

		public bool FileExists(string filename)
		{
			var filePath = GetFilePath(filename);
			return File.Exists(filePath);
		}

		public string GetFilePath(string filename)
		{
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var filePath = Path.Combine(documentsPath, filename);
			return filePath;
		}

		public void Delete(string filename)
		{
			var filePath = GetFilePath(filename);
			File.Delete(filePath);
		}

		public Stream GetReadFileStream(string path) {

			MemoryStream ms = new MemoryStream ();

			var urls = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
			var p = urls [0].Path;
			path = Path.Combine (p, path);

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