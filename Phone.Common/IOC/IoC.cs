using Ninject;
using Ninject.Modules;

namespace Phone.Common.IOC
{
    public class IoC
    {
        private static IKernel _kernel;

        public static IKernel GetKernel()
        {
            if (null == _kernel)
            {
                var settings = new NinjectSettings { InjectAttribute = typeof(DepInjectAttribute) };
                _kernel = new StandardKernel(settings);
            }

            return _kernel;
        }

        public static T Get<T>()
        {
            return GetKernel().Get<T>();
        }

        public static T TryGet<T>()
        {
            return GetKernel().TryGet<T>();
        }

        public static void LoadModules(params INinjectModule[] modules)
        {
            GetKernel().Load(modules);
        }
    }
}
