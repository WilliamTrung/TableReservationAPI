using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationContext
{
    public class TableReservationContext : DbContext
    {
        public virtual DbSet<Rate> Rates { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<Table> Tables { get; set; } = null!;
        public virtual DbSet<TableType> TableTypes { get; set; } = null!;
        public virtual DbSet<TableStatus> TableStatuses { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        public TableReservationContext()
        {
                
        }
        public TableReservationContext(DbContextOptions options) : base(options)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(optionsBuilder != null)
            {
                optionsBuilder.UseNpgsql(Global.ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReviewRating>().HasKey(c => new { c.ReviewId, c.RateId});
            modelBuilder.Entity<ReviewRating>().HasOne(c => c.Review).WithMany(c => c.ReviewRatings).HasForeignKey(c => c.ReviewId);
            modelBuilder.Entity<ReviewRating>().HasOne(c => c.Rate).WithMany(c => c.ReviewRatings).HasForeignKey(c => c.RateId);

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "Administrator"
                },
                new Role
                {
                    Id = 3,
                    Name = "Customer"
                },
                new Role
                {
                    Id = 2,
                    Name = "Reception"
                });
            modelBuilder.Entity<TableStatus>().HasData(
                new TableStatus
                {
                    Id = 1,
                    Description = "Vacant"
                },
                new TableStatus
                {
                    Id = 2,
                    Description = "Occupied"
                },
                new TableStatus
                {
                    Id = 3,
                    Description = "Unavailable"
                }
                );
            modelBuilder.Entity<TableType>().HasData(
                new TableType
                {
                    Id = 1,
                    Private = true,
                    Seat = 2
                },
                new TableType
                {
                    Id = 2,
                    Private = true,
                    Seat = 2
                },
                new TableType
                {
                    Id = 3,
                    Private = false,
                    Seat = 4
                },
                new TableType
                {
                    Id = 4,
                    Private = false,
                    Seat = 4
                }
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
