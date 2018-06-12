using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using donutstore.Models;
using Microsoft.Extensions.Configuration;

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



    }
}