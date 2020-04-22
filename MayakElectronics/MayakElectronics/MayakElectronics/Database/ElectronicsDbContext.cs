﻿using MayakElectronics.Data.Models;
using MayakElectronics.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Logging.Console;

  namespace MayakElectronics.Models
{
    public class ElectronicsDbContext : IdentityDbContext<AppIdentityUser>
    {
        public static readonly ILoggerFactory LoggerFactory = 
            new LoggerFactory(new[] {
                new ConsoleLoggerProvider((category, level) =>
                    category == DbLoggerCategory.Database.Command.Name &&
                    level == LogLevel.Information, true)
            });

        public ElectronicsDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(LoggerFactory)
                .EnableSensitiveDataLogging()
                //.UseSqlServer("Server=mayak.database.windows.net,1433;Database=mayak;User ID=mukhammad;Password=mbr2010IL;Encrypt=true;Connection Timeout=30;");
                //.UseSqlServer("Server=tcp:mayak.database.windows.net,1433;Initial Catalog=mayak;Persist Security Info=False;User ID=mukhammad;Password=mbr2010IL;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=helloappdb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartItem>()
                .HasOne(i => i.Product)
                .WithMany(c => c.CartItems)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Product>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Products)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Characteristic>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Characteristics)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CharValue>()
                .HasOne(i => i.Characteristic)
                .WithMany(c => c.CharValues)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CharValue>()
                .HasOne(i => i.Product)
                .WithMany(c => c.CharValues)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StoreLocation>()
                .HasOne(i => i.City)
                .WithMany(c => c.StoreLocations)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Review>()
                .HasOne(i => i.Product)
                .WithMany(c => c.Reviews)
                .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Characteristic> Characteristics { get; set; }
        public DbSet<CharValue> CharValues { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<StoreLocation> StoreLocations { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderCourier> OrdersCourier { get; set; }
        public DbSet<OrderPickUp> OrdersPickUp { get; set; }
        public DbSet<OrderPost> OrdersPost { get; set; }
        public DbSet<OrderDetailCourier> OrderDetailsCourier { get; set; }
        public DbSet<OrderDetailPickUp> OrderDetailsPickUp { get; set; }
        public DbSet<OrderDetailPost> OrderDetailsPost { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<AreBought> AreBoughts { get; set; }
     }
 }