using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationContext.EFConfiguration
{
    public class TableTypeConfiguration
    {
        public static void Configuring(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TableType>().HasIndex(tableType => new { tableType.Seat, tableType.Private }).IsUnique();
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
                    Private = false,
                    Seat = 2
                },
                new TableType
                {
                    Id = 3,
                    Private = true,
                    Seat = 4
                },
                new TableType
                {
                    Id = 4,
                    Private = false,
                    Seat = 4
                }
                );
        }
    }
}
