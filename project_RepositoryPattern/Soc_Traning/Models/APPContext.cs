using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Traning.Models
{
    public class APPContext : DbContext
    {
        public APPContext(DbContextOptions<APPContext> options) : base(options)
        {

        }

        public DbSet<UserModel> Users { get; set; }
    }
}
