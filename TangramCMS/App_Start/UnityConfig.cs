using Microsoft.Practices.Unity;
using System.Web.Http;
using TangramCMS.Repositories;
using Unity.WebApi;

namespace TangramCMS
{
    public static class UnityConfig
    {
        public static UnityContainer Container { get; private set; }
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            Container = container;
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ICmsService, CmsService>(new HierarchicalLifetimeManager());
            container.RegisterType<ICmsDocumentRepository, CmsDocumentRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ICmsCollectionRepository, CmsCollectionRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ICmsSelectionRepository, CmsSelectionRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ICmsAclRepository, CmsAclRepository>(new HierarchicalLifetimeManager());
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}