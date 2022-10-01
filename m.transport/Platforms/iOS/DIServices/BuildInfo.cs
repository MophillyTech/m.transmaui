using System;
using m.transport.Interfaces;
using m.transport.iOS;
using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(BuildInfo))]
namespace m.transport.iOS
{
	public class BuildInfo : IBuildInfo
	{
		public BuildInfo ()
		{
		}

		public string Version { 
			get { 
				return NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleShortVersionString")].ToString ();
			} 
		}
		public string BuildNumber { 
			get { 
				return NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleVersion")].ToString ();
			} 
		}

		public string BundleID { 
			get { 
				return NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleIdentifier")].ToString ();
			} 
		}

	}
}

