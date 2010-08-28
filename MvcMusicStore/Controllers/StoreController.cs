﻿using System.Linq;
using System.Web.Mvc;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;

namespace MvcMusicStore.Controllers
{
    public class StoreController : Controller
    {
        private IMusicStoreContext storeContext;

        public StoreController(IMusicStoreContext storeContext)
        {
            this.storeContext = storeContext;
        }

        //
        // GET: /Store/

        public ActionResult Index()
        {
            // Retrieve list of Genres from database
            var genres = from genre in storeContext.Genres
                         select genre.Name;

            // Set up our ViewModel
            var viewModel = new StoreIndexViewModel()
            {
                Genres = genres.ToList(),
                NumberOfGenres = genres.Count()
            };

            // Return the view
            return View(viewModel);
        }

        //
        // GET: /Store/Browse?Genre=Disco

        public ActionResult Browse(string genre)
        {
            // Retrieve Genre from database
            var genreModel = storeContext.Genres.Include("Albums")
                .Single(g => g.Name == genre);

            var viewModel = new StoreBrowseViewModel()
            {
                Genre = genreModel,
                Albums = genreModel.Albums.ToList()
            };

            return View(viewModel);
        }

        //
        // GET: /Store/Details/5

        public ActionResult Details(int id)
        {
            var album = storeContext.Albums
                .Single(a => a.AlbumId == id);

            return View(album);
        }

        //
        // GET: /Store/GenreMenu

        [ChildActionOnly]
        public ActionResult GenreMenu()
        {
            var genres = storeContext.Genres.ToList();

            return View(genres);
        }
    }
}