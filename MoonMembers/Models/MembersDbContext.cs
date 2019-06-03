using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MoonMembers.Models
{
    public class MembersDbContext : DbContext
    {
        public DbSet<Members> Members { get; set; }
    }
}