using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcMusicStore.Models
{
    public class ShoppingCart
    {
        private IMusicStoreContext storeContext;
        string shoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(HttpContextBase httpContext, IMusicStoreContext storeContext)
        {
            var cart = new ShoppingCart(storeContext);
            cart.shoppingCartId = cart.GetCartId(httpContext);
            return cart;
        }

        private ShoppingCart(IMusicStoreContext storeContext)
        {
            this.storeContext = storeContext;
        }

        public void AddToCart(Album album)
        {
            using (var tx = storeContext.Session.BeginTransaction())
            {
                var cartItem = storeContext.Carts.SingleOrDefault(
                    c => c.CartId == shoppingCartId &&
                         c.Album == album);

                if (cartItem == null)
                {
                    // Create a new cart item
                    cartItem = new Cart
                                   {
                                       Album = album,
                                       CartId = shoppingCartId,
                                       Count = 1,
                                       DateCreated = DateTime.Now
                                   };
                }
                else
                {
                    // Add one to the quantity
                    cartItem.Count++;
                }

                // Save it
                storeContext.Session.Save(cartItem);

                tx.Commit();
            }
        }

        public void RemoveFromCart(int id)
        {
            using (var tx = storeContext.Session.BeginTransaction())
            {
                //Get the cart
                var cartItem = storeContext.Carts.Single(
                    cart => cart.CartId == shoppingCartId
                            && cart.RecordId == id);

                if (cartItem != null)
                {
                    if (cartItem.Count > 1)
                    {
                        cartItem.Count--;
                        storeContext.Session.Save(cartItem);
                    }
                    else
                    {
                        storeContext.Session.Delete(cartItem);
                    }
                }

                tx.Commit();
            }
        }

        public void EmptyCart()
        {
            using (var tx = storeContext.Session.BeginTransaction())
            {
                var cartItems = storeContext.Carts
                    .Where(cart => cart.CartId == shoppingCartId);

                foreach (var cartItem in cartItems)
                {
                    storeContext.Session.Delete(cartItem);
                }

                tx.Commit();
            }
        }

        public IQueryable<Cart> GetCartItems()
        {
            var cartItems = from cart in storeContext.Carts
                            where cart.CartId == shoppingCartId
                            select cart;

            return cartItems;
        }

        public int GetCount()
        {
            using (var tx = storeContext.Session.BeginTransaction())
            {
                int? count = (from cartItems in storeContext.Carts
                              where cartItems.CartId == shoppingCartId
                              select (int?) cartItems.Count).Sum();

                tx.Commit();

                return count ?? 0;
            }
        }

        public decimal GetTotal()
        {
            using (var tx = storeContext.Session.BeginTransaction())
            {
                decimal? total =
                    (from cartItems in storeContext.Carts
                     where cartItems.CartId == shoppingCartId
                     select (int?) cartItems.Count*cartItems.Album.Price)
                        .Sum();

                tx.Commit();

                return total ?? decimal.Zero;
            }
        }

        public int CreateOrder(Order order)
        {
            using (var tx = storeContext.Session.BeginTransaction())
            {
                var cartItems = GetCartItems();

                //Iterate the items in the cart, adding Order Details for each
                foreach (var cartItem in cartItems)
                {
                    var orderDetail = new OrderDetail
                                          {
                                              Album = cartItem.Album,
                                              Order = order,
                                              UnitPrice = cartItem.Album.Price
                                          };

                    order.OrderDetails.Add(orderDetail);
                }

                //Save the order
                storeContext.Session.Save(order);
                tx.Commit();
            }

            //Empty the shopping cart
            EmptyCart();

            //Return the OrderId as a confirmation number
            return order.OrderId;
        }

        // We're using HttpContextBase to allow access to cookies.
        public String GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrEmpty(context.User.Identity.Name))
                {
                    // User is logged in, associate the cart with their username
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid Class
                    Guid tempCartId = Guid.NewGuid();

                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            using (var tx = storeContext.Session.BeginTransaction())
            {
                var shoppingCart = storeContext.Carts
                    .Where(c => c.CartId == shoppingCartId);

                foreach (Cart item in shoppingCart)
                {
                    item.CartId = userName;
                }

                tx.Commit();
            }
        }
    }
}