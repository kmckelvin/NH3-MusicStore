using System.Linq;
using NHibernate;

namespace MvcMusicStore.Models
{
    public interface IMusicStoreContext
    {
        ISession Session { get; }
        IQueryable<Album> Albums { get; }
        IQueryable<Artist> Artists { get; }
        IQueryable<Cart> Carts { get; }
        IQueryable<Genre> Genres { get; }
        IQueryable<Order> Orders { get; }
    }
}