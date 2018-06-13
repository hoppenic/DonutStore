using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using donutstore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace donutstore.Controllers
{
    public class ProductController : Controller
    {
        private readonly DonutStoreDbContext _donutStoreDbContext;

        public ProductController(DonutStoreDbContext donutStoreDbContext)
        {

            _donutStoreDbContext = donutStoreDbContext;
        }


        public IActionResult Index()
        {
            List<Product> products = _donutStoreDbContext.Products.ToList();
            return View(products);
        }



        public IActionResult Details(int? ID)
        {
            if (ID.HasValue)
            {
                Product p = _donutStoreDbContext.Products.Find(ID.Value);
                return View(p);
            }

            return NotFound();
        }


        [HttpPost]
        public IActionResult Details(int ID,int quantity = 1)
        {
            Guid cartId;
            Cart cart = null;
            if (Request.Cookies.ContainsKey("cartId"))
            {
                if(Guid.TryParse(Request.Cookies["cartid"], out cartId))
                {
                    cart = _donutStoreDbContext.Carts
                    .Include(Carts => Carts.CartItems)
                    .ThenInclude(CartItems => CartItems.Product)
                    .FirstOrDefault(x => x.CookieIdentifier == cartId);

                }
            }
            if (cart == null)
            {
                cart = new Cart();
                cartId = Guid.NewGuid();
                cart.CookieIdentifier = cartId;

                _donutStoreDbContext.Carts.Add(cart);
                Response.Cookies.Append("cartId", cartId.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTime.UtcNow.AddYears(100) });

            }

            CartItem item = cart.CartItems.FirstOrDefault(x => x.Product.ID == ID);
            if (item == null)
            {
                item = new CartItem();
                item.Product = _donutStoreDbContext.Products.Find(ID);
                cart.CartItems.Add(item);
            }
            item.Quantity += quantity;
            cart.LastModified = DateTime.Now;

            _donutStoreDbContext.SaveChanges();
            return RedirectToAction("Index", "Cart");


        }


    }
}