using System.Linq;
using System.Web.Mvc;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;

namespace MvcMusicStore.Controllers
{
    [HandleError]
    [Authorize(Roles = "Administrator")]
    public class StoreManagerController : Controller
    {
        private IMusicStoreContext storeContext;// = new MusicStoreEntities();

        public StoreManagerController(IMusicStoreContext storeContext)
        {
            this.storeContext = storeContext;
        }

        //
        // GET: /StoreManager/

        public ActionResult Index()
        {
            var albums = storeContext.Albums
                .Include("Genre").Include("Artist")
                .ToList();

            return View(storeContext.Albums);
        }

        // 
        // GET: /StoreManager/Create

        public ActionResult Create()
        {
            var viewModel = new StoreManagerViewModel
            {
                Album = new Album(),
                Genres = storeContext.Genres.ToList(),
                Artists = storeContext.Artists.ToList()
            };

            return View(viewModel);
        }

        //
        // POST: /StoreManager/Create

        [HttpPost]
        public ActionResult Create(Album album)
        {
            try
            {
                //Save Album
                storeContext.AddToAlbums(album);
                storeContext.SaveChanges();

                return Redirect("/");
            }
            catch
            {
                //Invalid - redisplay with errors

                var viewModel = new StoreManagerViewModel
                {
                    Album = album,
                    Genres = storeContext.Genres.ToList(),
                    Artists = storeContext.Artists.ToList()
                };

                return View(viewModel);
            }
        }

        //
        // GET: /StoreManager/Edit/5

        public ActionResult Edit(int id)
        {
            var viewModel = new StoreManagerViewModel
            {
                Album = storeContext.Albums.Single(a => a.AlbumId == id),
                Genres = storeContext.Genres.ToList(),
                Artists = storeContext.Artists.ToList()
            };

            return View(viewModel);
        }

        //
        // POST: /StoreManager/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection formValues)
        {
            var album = storeContext.Albums.Single(a => a.AlbumId == id);

            try
            {
                //Save Album

                UpdateModel(album, "Album");
                storeContext.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                var viewModel = new StoreManagerViewModel
                {
                    Album = album,
                    Genres = storeContext.Genres.ToList(),
                    Artists = storeContext.Artists.ToList()
                };

                return View(viewModel);
            }
        }

        //
        // GET: /StoreManager/Delete/5

        public ActionResult Delete(int id)
        {
            var album = storeContext.Albums.Single(a => a.AlbumId == id);

            return View(album);
        }

        //
        // POST: /StoreManager/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, string confirmButton)
        {
            var album = storeContext.Albums
                .Include("OrderDetails").Include("Carts")
                .Single(a => a.AlbumId == id);

            // For simplicity, we're allowing deleting of albums 
            // with existing orders We've set up OnDelete = Cascade 
            // on the Album->OrderDetails and Album->Carts relationships

            storeContext.DeleteObject(album);
            storeContext.SaveChanges();

            return View("Deleted");
        }
    }
}