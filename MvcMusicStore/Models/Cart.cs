using System;

namespace MvcMusicStore.Models
{
    public class Cart
    {
        public virtual int RecordId { get; set; }
        public virtual string CartId { get; set; }
        public virtual Album Album { get; set; }
        public virtual int Count { get; set; }
        public virtual DateTime DateCreated { get; set; }
    }
}