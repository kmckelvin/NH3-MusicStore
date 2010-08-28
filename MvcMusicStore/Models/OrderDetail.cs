namespace MvcMusicStore.Models
{
    public class OrderDetail
    {
        public virtual int OrderDetailId { get; set; }
        public virtual Order Order { get; set; }
        public virtual Album Album { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal UnitPrice { get; set; }
    }
}