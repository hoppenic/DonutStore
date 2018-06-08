using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using donutstore.Models;

namespace donutstore.Controllers
{
    public class ProductController : Controller
    {
        private List<Product> _products;


        public ProductController()
        {
            _products = new List<Product>();
            _products.Add(new Product
            {
                ID = 1,
                Name = "Chocolate Donut",
                Description = "Delicious Chocolate Donut",
                Image = "/images/ChocolateDonut.jpeg",
                Price = 2.99m

            });

            _products.Add(new Product
            {
                ID=2,
                Name="Sprinkles Donut",
                Description="Donut with sprinkles",
                Image="/images/donut2.jpeg",
                Price=3.99m

            });

        }

        public IActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                Product p = _products.Single(x => x.ID == id.Value);
                return View(p);
            }

            return NotFound();
        }



        public IActionResult Index()
        {
            return View(_products);
        }
    }
}