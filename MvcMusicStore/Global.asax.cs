using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.Models;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Context;
using NHibernate.Dialect;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Mvc;

namespace MvcMusicStore
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : NinjectHttpApplication
    {
        private static ISessionFactory SessionFactory = CreateSessionFactory();

        private static ISessionFactory CreateSessionFactory()
        {
            var config = new Configuration();

            config.DataBaseIntegration(
                db =>
                    {
                        db.ConnectionStringName = "MusicStoreConnection";
                        db.Dialect<MsSql2005Dialect>();
                    })
                .Proxy(p => p.ProxyFactoryFactory<ProxyFactoryFactory>())
                .SetProperty(NHibernate.Cfg.Environment.CurrentSessionContextClass, "web")
                .AddAssembly(typeof (Album).Assembly);

            return config.BuildSessionFactory();
        }

        public static IMusicStoreContext GetCurrentRequestContext()
        {
            // get the ISession bound to the current request
            var currentSession = SessionFactory.GetCurrentSession();

            // build a context object to wrap it
            var context = new MusicStoreContext(currentSession);

            return context;
        }

        public MvcApplication()
        {
            BeginRequest += (o, e) => CurrentSessionContext.Bind(SessionFactory.OpenSession());
            EndRequest += (o, e) => CurrentSessionContext.Unbind(SessionFactory);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Browse",                                                // Route name
                "Store/Browse/{genre}",                                  // URL with parameters
                new { controller = "Store", action = "Browse", id = "" } // Parameter defaults
            );

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }

        protected override void OnApplicationStarted()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }

        protected override IKernel CreateKernel()
        {
            return new StandardKernel(new MusicStoreNinjectModule());
        }
    }
}