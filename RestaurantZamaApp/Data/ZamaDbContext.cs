//using Android.Content;
using Microsoft.EntityFrameworkCore;
using RestaurantZamaApp.Models;
using System.Collections.Generic;

namespace RestaurantZamaApp.Data
{
    public class ZamaDbContext : DbContext
    {
        public ZamaDbContext(DbContextOptions<ZamaDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RestaurantZamaApp.Models.Table> Tables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(250);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(250);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                // Poți adăuga alte proprietăți relevante pentru profil
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<RestaurantZamaApp.Models.Table>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Number).IsRequired();
                entity.Property(e => e.Capacity).IsRequired();
                entity.Property(e => e.Location).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Number).IsUnique();
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.TableId).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ContactPhone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.SpecialRequests).HasMaxLength(500);
                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reservations)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.Table)
                      .WithMany(t => t.Reservations)
                      .HasForeignKey(r => r.TableId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}