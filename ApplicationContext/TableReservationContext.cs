using ApplicationContext.EFConfiguration;
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
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        public TableReservationContext()
        {
                
        }
        public TableReservationContext(DbContextOptions<TableReservationContext> options) : base(options)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder != null)
            {
                optionsBuilder.UseNpgsql(Global.ConnectionString);
            }
            //base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReviewRating>().HasKey(c => new { c.ReviewId, c.RateId});
            modelBuilder.Entity<ReviewRating>().HasOne(c => c.Review).WithMany(c => c.ReviewRatings).HasForeignKey(c => c.ReviewId);
            modelBuilder.Entity<ReviewRating>().HasOne(c => c.Rate).WithMany(c => c.ReviewRatings).HasForeignKey(c => c.RateId);
            TableTypeConfiguration.Configuring(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
