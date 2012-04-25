using CodeStock.App.Design.Modules;
using GalaSoft.MvvmLight;
using Phone.Common.IOC;

namespace CodeStock.App.IOC
{
    public static class ModuleLoader
    {
        public static void Load()
        {
            if (IsLoaded) return;

            if (ViewModelBase.IsInDesignModeStatic)
            {
                IoC.LoadModules(new DesignServicesModule(), new DesignModule());
            }
            else
            {
                IoC.LoadModules(new Module(), new ServicesModule());
            }

            IsLoaded = true;
        }

        private static bool IsLoaded { get; set; }
    }
}
