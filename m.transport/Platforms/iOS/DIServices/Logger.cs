using System;
using m.transport.Interfaces;
using m.transport.iOS;
using System.IO;
using Foundation;
using CoreFoundation;
using UIKit;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Logger))]
namespace m.transport.iOS
{
	public class Logger : ILogger
	{
		public Logger ()
		{
		}

		public string ReadLog() {

			if (File.Exists(this.LogFile)) {
				return File.ReadAllText (this.LogFile);
			} 

			return string.Empty;
		}

		public void Log(string message) {

			File.AppendAllText (this.LogFile, message);

		}

		public void ClearLog() {
			File.WriteAllText (this.LogFile, string.Empty);
		}

		public void CopyLog() {
			UIPasteboard.General.SetValue(new NSString(this.ReadLog()),"public.utf8-plain-text");
		}

		private string LogFile {
			get {
				string filename;
				string sFile = "log.txt";

				int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);
				if (SystemVersion >= 8)
				{
					var documents = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
					filename = Path.Combine(documents, sFile);
				}
				else
				{
					var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // iOS 7 and earlier
					filename = Path.Combine(documents, sFile);
				}

				return filename;
			}
		}

	}
}

