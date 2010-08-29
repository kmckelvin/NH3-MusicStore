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
            // TODO: Eager-load the genres and artists here.

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
                using (var tx = storeContext.Session.BeginTransaction())
                {
                    storeContext.Session.Save(album);

                    tx.Commit();
                }

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

            using (var tx = storeContext.Session.BeginTransaction())
            {
                try
                {
                    //Save Album

                    UpdateModel(album, "Album");
                    storeContext.Session.Save(album);
                    
                    tx.Commit();
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
                finally
                {
                    // ensure the transaction is committed
                    if (tx != null && tx.WasCommitted == false)
                    {
                        tx.Commit();
                    }
                }
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
            using (var tx = storeContext.Session.BeginTransaction())
            {
                var album = storeContext.Albums.Single(a => a.AlbumId == id);

                // -- comment from the EF4 based MVC Music Store
                // For simplicity, we're allowing deleting of albums 
                // with existing orders We've set up OnDelete = Cascade 
                // on the Album->OrderDetails and Album->Carts relationships
                // -- end

                // Using NHibernate we have a similar feature, we set the
                // cascade="all-delete-orphans" on Album.OrderDetails and
                // on Album.Carts to delete existing orders.

                storeContext.Session.Delete(album);

                tx.Commit();
            }

            return View("Deleted");
        }
    }
}