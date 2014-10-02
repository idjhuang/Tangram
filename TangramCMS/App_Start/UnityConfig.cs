using Microsoft.Practices.Unity;
using System.Web.Http;
using TangramCMS.Repositories;
using Unity.WebApi;

namespace TangramCMS
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ICmsCollectionRepository, CmsCollectionRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ICmsService, CmsService>(new HierarchicalLifetimeManager());
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}