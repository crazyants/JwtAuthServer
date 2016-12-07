﻿using Microsoft.EntityFrameworkCore;
using LegnicaIT.DataAccess.Models;


namespace LegnicaIT.DataAccess.Context
{
    public class JwtDbContext : DbContext
    {
        public JwtDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> User { set; get; }
        public DbSet<UserApps> UserApps { set; get; }
        public DbSet<App> App { set; get; }     
    }
}
