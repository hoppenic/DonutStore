using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using donutstore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;




namespace donutstore.Models
{
    public class DonutStoreDbContext : IdentityDbContext<DonutStoreUser>
    {
        public DonutStoreDbContext(): base()
        {


        }

        public DonutStoreDbContext(DbContextOptions options) : base(options)
        {


        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
    }



    public class Cart
    {   

        //cart constructor
        public Cart()
        {
            this.CartItems = new HashSet<CartItem>();
        }

        public int ID { get; set; }
        public Guid CookieIdentifier { get; set; }
        public DateTime LastModified { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }

    //cart items class
    public class CartItem
    {
        public int ID { get; set; }
        public Cart Cart { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

    }


    public class DonutStoreUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
