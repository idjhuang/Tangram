using Microsoft.Practices.Unity;
using System.Web.Http;
using TangramService.Repositories;
using Unity.WebApi;

namespace TangramService
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
            container.RegisterType<IService, Service>(new HierarchicalLifetimeManager());
            container.RegisterType<IDocumentRepository, DocumentRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ICollectionRepository, CollectionRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ISelectionRepository, SelectionRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDocumentTypeRepository, DocumentTypeRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuthorizationRepository, AuthorizationRepository>(new HierarchicalLifetimeManager());
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}