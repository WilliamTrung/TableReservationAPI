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
                },
                new TableType
                {
                    Id = 5,
                    Private = true,
                    Seat = 6
                },
                new TableType
                {
                    Id = 6,
                    Private = false,
                    Seat = 6
                },
                new TableType
                {
                    Id = 7,
                    Private = true,
                    Seat = 8
                },
                new TableType
                {
                    Id = 8,
                    Private = false,
                    Seat = 8
                },
                new TableType
                {
                    Id = 9,
                    Private = true,
                    Seat = 12
                },
                new TableType
                {
                    Id = 10,
                    Private = false,
                    Seat = 12
                }
                );
        }
    }
}
