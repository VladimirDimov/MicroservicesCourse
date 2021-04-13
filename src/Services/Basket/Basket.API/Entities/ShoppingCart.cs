namespace Basket.API.Entities
{
    using System.Collections.Generic;
    using System.Linq;

    public class ShoppingCart
    {
        public ShoppingCart()
        {
        }

        public ShoppingCart(string userName)
        {
            UserName = userName;
        }

        public string UserName { get; set; }

        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    }
}
