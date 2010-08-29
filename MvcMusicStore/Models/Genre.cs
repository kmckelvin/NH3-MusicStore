using System.Collections.Generic;

namespace MvcMusicStore.Models
{
    public class Genre
    {
        public virtual int GenreId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
    }
}