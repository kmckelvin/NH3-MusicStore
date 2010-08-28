using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace MvcMusicStore.Models
{
    public class MusicStoreContext : IMusicStoreContext
    {
        private readonly ISession _currentSession;

        public MusicStoreContext(ISession currentSession)
        {
            _currentSession = currentSession;
        }

        public ISession Session
        {
            get { return _currentSession; }
        }

        public IQueryable<Album> Albums
        {
            get { return _currentSession.Query<Album>(); }
        }

        public IQueryable<Artist> Artists
        {
            get { return _currentSession.Query<Artist>(); }
        }

        public IQueryable<Cart> Carts
        {
            get { return _currentSession.Query<Cart>(); }
        }

        public IQueryable<Genre> Genres
        {
            get { return _currentSession.Query<Genre>(); }
        }

        public IQueryable<Order> Orders
        {
            get { return _currentSession.Query<Order>(); }
        }
    }
}