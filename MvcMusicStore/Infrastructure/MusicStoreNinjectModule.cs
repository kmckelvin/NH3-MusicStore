using System;
using MvcMusicStore.Controllers;
using MvcMusicStore.Models;
using NHibernate;
using Ninject.Modules;

namespace MvcMusicStore.Infrastructure
{
    public class MusicStoreNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<AccountController>().ToSelf();
            Bind<CheckoutController>().ToSelf();
            Bind<HomeController>().ToSelf();
            Bind<ShoppingCartController>().ToSelf();
            Bind<StoreController>().ToSelf();
            Bind<StoreManagerController>().ToSelf();

            // This will be injected into our controller to give us access to our context
            // without introducing a tight coupling to a static resource.
            Bind<IMusicStoreContext>().ToMethod(c => MvcApplication.GetCurrentRequestContext());
        }
    }
}