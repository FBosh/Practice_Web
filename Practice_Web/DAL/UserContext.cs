using Practice_Web.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Practice_Web.DAL
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}