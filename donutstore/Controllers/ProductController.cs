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
        private readonly DonutStoreDbContext _context;

        public ProductController(DonutStoreDbContext context)
        {

            _context = context;
        }


        public IActionResult Index()
        {
            List<Product> products = _context.Products.ToList();
            return View(products);
        }



        public IActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                Product p = _context.Products.Find(id.Value);
                return View(p);
            }

            return NotFound();
        }


        [HttpPost]
        public IActionResult Details(int id,int quantity = 1)
        {
            Guid cartId;
            Cart cart = null;
            if (Request.Cookies.ContainsKey("cartId"))
            {
                if(Guid.TryParse(Request.Cookies["cartid"], out cartId))
                {
                    cart = _context.Carts
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

                _context.Carts.Add(cart);
                Response.Cookies.Append("cartId", cartId.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTime.UtcNow.AddYears(100) });

            }


        }



    }
}