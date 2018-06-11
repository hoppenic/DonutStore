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


    }

    public class DonutStoreUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }



    }
}
