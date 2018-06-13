using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using donutstore.Models;
using Microsoft.EntityFrameworkCore;


namespace donutstore.Controllers
{
    public class CartController : Controller
    {
        //this is injecting my DonutStoreDbContext
        private readonly DonutStoreDbContext _donutStoreDbContext;

        public CartController(DonutStoreDbContext context)
        {

            _donutStoreDbContext = context;
        }


        public IActionResult Index()
        {

            Guid cartID;
            Cart cart = null;

            if (Request.Cookies.ContainsKey("cartId"))
            {
                if(Guid.TryParse(Request.Cookies["cartId"],out cartID))
                {
                    cart = _donutStoreDbContext.Carts
                        .Include(Carts => Carts.CartItems)
                        .ThenInclude(CartItems => CartItems.Product)
                        .FirstOrDefault(x => x.CookieIdentifier == cartID);
                }
            }
            if (cart == null)
            {
                cart = new Cart();

            }
            return View(cart);
        }


        public IActionResult Remove(int id)
        {
            Guid cartId;
            Cart cart = null;
            if (Request.Cookies.ContainsKey("cartId"))
            {
                if(Guid.TryParse(Request.Cookies["cartId"],out cartId))
                {
                    cart = _donutStoreDbContext.Carts
                        .Include(Carts => Carts.CartItems)
                        .ThenInclude(CartItems => CartItems.Product)
                        .FirstOrDefault(x => x.CookieIdentifier == cartId);
                }
            }

            CartItem item = cart.CartItems.FirstOrDefault(x => x.ID == id);
            cart.LastModified = DateTime.Now;
            _donutStoreDbContext.CartItems.Remove(item);
            _donutStoreDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}