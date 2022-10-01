using System;
using Autofac;
using m.transport.Data;

namespace m.transport.ViewModels
{
    public class AppSettingsViewModel : BaseViewModel
    {
        private ILoginRepository loginRepository;

        public AppSettingsViewModel()
        {
        }
    }
}
