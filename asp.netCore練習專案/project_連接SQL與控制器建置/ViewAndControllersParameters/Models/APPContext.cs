using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ViewAndControllersParameters.Models
{
    public class APPContext: DbContext
    {
        public APPContext(DbContextOptions<APPContext> options) : base(options)
        {
        }
        public DbSet<Item> Item { get; set; }
    }
}
