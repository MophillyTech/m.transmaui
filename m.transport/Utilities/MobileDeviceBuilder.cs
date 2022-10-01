using System;
using m.transport.Domain;
using DeviceInfo.Plugin;
using m.transport.Interfaces;
using m.transport.Data;
using Autofac;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class MobileDeviceBuilder
	{
		public MobileDeviceBuilder ()
		{
		}

		public static MobileDevice BuildMobileDevice() {

			ILoginRepository loginRepository = App.Container.Resolve<ILoginRepository>();
			IBuildInfo build = DependencyService.Get<IBuildInfo>();
			IHardwareInfo hw = DependencyService.Get<IHardwareInfo> ();

			MobileDevice device = new MobileDevice () {
				DriverID = 0,
				Platform = CrossDeviceInfo.Current.Platform.ToString(),
				Make = hw.Manufacturer,
				Model = CrossDeviceInfo.Current.Model,
				OSVersion = CrossDeviceInfo.Current.Version,
				PhoneNumber = string.Empty,
				LastConnectionType = "Login",
				LastConnectionDate = DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Unspecified),
				MobileName = hw.Name,
				MobileCellCarrier = hw.Carrier,
				MobileScreenHeight = hw.Height,
				MobileScreenWidth = hw.Width,
				RecordStatus = "Active",
				CreationDate = DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Unspecified),
				CreatedBy = loginRepository.Username,
				UpdatedDate = DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Unspecified),
				UpdatedBy = loginRepository.Username
			};

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
				device.Model = hw.Model;
			}

			device.AppVersion = build.Version;
			device.AppBuild = Int32.Parse(build.BuildNumber);

			return device;
		}
	}
}

