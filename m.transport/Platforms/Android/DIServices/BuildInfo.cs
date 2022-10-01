using System;
using m.transport.Interfaces;
using m.transport.Android;
using Android.Content;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(BuildInfo))]
namespace m.transport.Android
{
	public class BuildInfo : IBuildInfo
	{
		public string Version
		{
			get
			{
				//return Forms.Context.PackageManager.GetPackageInfo(Forms.Context.PackageName, 0).VersionCode.ToString();
				return Forms.Context.PackageManager.GetPackageInfo(Forms.Context.PackageName, 0).VersionName;
			}
		}

		public string BuildNumber
		{
			get
			{
				//return Forms.Context.PackageManager.GetPackageInfo(Forms.Context.PackageName, 0).VersionName;
				return Forms.Context.PackageManager.GetPackageInfo(Forms.Context.PackageName, 0).VersionCode.ToString();
			}
		}

		public string BundleID
		{
			get
			{
				return Forms.Context.PackageManager.GetPackageInfo(Forms.Context.PackageName, 0).PackageName;
			}
		}

	}
}

