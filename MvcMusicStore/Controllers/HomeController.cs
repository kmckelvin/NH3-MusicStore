﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        private IMusicStoreContext storeContext;

        public HomeController(IMusicStoreContext storeContext)
        {
            this.storeContext = storeContext;
        }

        public ActionResult Index()
        {
            // Get most popular albums
            var albums = GetTopSellingAlbums(5);

            return View(albums);
        }

        private IList<Album> GetTopSellingAlbums(int count)
        {
            // Group the order details by album and return
            // the albums with the highest count

            return storeContext.Albums.ToList()
                .OrderBy(a => a.OrderDetails.Count())
                .Take(count)
                .ToList();
        }
    }
}
