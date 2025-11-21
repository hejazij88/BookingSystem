using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Data
{
    public class BookingDbContext:IdentityDbContext<ApplicationUser>
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
        {
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Service>(e =>
            {
                e.HasKey(s => s.Id);
                e.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(s => s.Price)
                    .HasColumnType("decimal(18,2)");
            });


            modelBuilder.Entity<Appointment>(e =>
            {
                e.HasKey(a => a.Id);

                e.HasOne(a => a.Service)
                    .WithMany(s => s.Appointments)
                    .HasForeignKey(a => a.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }

        
    }
}
