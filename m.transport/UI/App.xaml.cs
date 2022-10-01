using System.ServiceModel;
using Autofac;
using m.transport.Data;
using m.transport.Domain;
using m.transport.Interfaces;
using m.transport.Models;
using m.transport.ServiceInterface;
using m.transport.Services;
using m.transport.Svc;
using SQLite.Net.Interop;
using Akavache;
using m.transport.Cache;
using System;
using mtd = m.transport.Domain;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class App : Application
	{
		public static Autofac.IContainer Container;
		public static ContainerBuilder ContainerBuilder { get; private set; }
		public static string DBPath { get; private set; }
		public static ISQLitePlatform SQLitePlatform { get; private set; }

        public static int ScreenWidth { get; set; }
        public static int ScreenHeight { get; set; }
		public App()
		{
			InitializeComponent();
		}


		static App()
		{
            //InitializeComponent();
            BlobCache.ApplicationName = "m.transport";

			var builder = new ContainerBuilder();
            var serviceFactory = new ServiceClientFactory<ITransportServiceClient>
            {
                CreateFunc = u => new TransportServiceClient(new BasicHttpBinding(){
                    SendTimeout = new TimeSpan(0, 15, 0),
                    ReceiveTimeout = new TimeSpan(0, 15, 0),
		        }, new EndpointAddress(u)),
			};
			builder.RegisterInstance(serviceFactory)
				.As<IServiceClientFactory<ITransportServiceClient>>().SingleInstance();

            var restFactory = new ServiceClientFactory<IRestServiceClient>
            {
                CreateFunc = u => new RestServiceClient(u)
            };
            builder.RegisterInstance(restFactory)
                .As<IServiceClientFactory<IRestServiceClient>>().SingleInstance();

            builder.RegisterType<SqlLiteRepository<LoginResult, int>>().As<IRepository<LoginResult, int>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<DatsRun, int>>().As<IRepository<DatsRun, int>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<DatsRunStop, int>>().As<IRepository<DatsRunStop, int>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<DatsLocation, int>>().As<IRepository<DatsLocation, int>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<DatsVehicleV5, string>>().As<IRepository<DatsVehicleV5, string>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<SelectedLoads, int>>().As<IRepository<SelectedLoads, int>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<mtd.DamageAreaCode, string>>().As<IRepository<mtd.DamageAreaCode, string>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<mtd.DamageSeverityCode, string>>().As<IRepository<mtd.DamageSeverityCode, string>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<mtd.DamageTypeCode, string>>().As<IRepository<mtd.DamageTypeCode, string>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<Setting, string>>().As<IRepository<Setting, string>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<Paper, string>>().As<IRepository<Paper, string>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<Server, string>>().As<IRepository<Server, string>>().SingleInstance();
			builder.RegisterType<SqlLiteRepository<DamagePhoto,string>> ().As<IRepository<DamagePhoto, string>> ().SingleInstance ();
			builder.RegisterType<SqlLiteRepository<m.transport.Domain.Expense,int>> ().As<IRepository<m.transport.Domain.Expense,int>> ().SingleInstance ();
			builder.RegisterType<SqlLiteRepository<m.transport.Domain.Code, int>> ().As<IRepository<m.transport.Domain.Code, int>> ().SingleInstance ();

			builder.RegisterType<LoginRepository>().As<ILoginRepository>().SingleInstance();
			builder.RegisterType<DriverSignatureRepository>().As<IDriverSignatureRepository>().SingleInstance();
			builder.RegisterType<AppSettingsRepository>().As<IAppSettingsRepository>().SingleInstance();
			builder.RegisterType<CurrentLoadRepository>().As<ICurrentLoadRepository>().SingleInstance();
			builder.RegisterType<ExpensesRepository> ().As<IExpensesRepository> ().SingleInstance ();


            builder.RegisterType<GlobalCache>().As<ICache>().SingleInstance();

			UpdateContainer(builder);
		}
        protected override Window CreateWindow(IActivationState activationState)
        {
			Microsoft.Maui.Controls.Compatibility.Forms.Init(activationState);
            return new Microsoft.Maui.Controls.Window(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        public static void RegisterType<TConcrete, TInterface>(ContainerBuilder builder = null)
		{
			if (builder == null)
			{
				builder = new ContainerBuilder();
			}
			builder.RegisterType<TConcrete>().As<TInterface>().SingleInstance();
			UpdateContainer(builder);
		}

		private static void UpdateContainer(ContainerBuilder builder)
		{
			if (Container == null)
			{
				Container = builder.Build();
			}
			else
			{
				builder.Update(Container);
			}
		}

		//public App()
		//{
		//	InitializeComponent();
		//	MainPage = new MainPage();
		//}

		public static void SetDatabaseConnection(ISQLitePlatform platform, string path)
		{
			SQLitePlatform = platform;
			DBPath = path;
		}

	}
}
