using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationContext.EFConfiguration
{
    public class FeedbackConfiguration
    {
        public static void Configuring(in ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feedback>().HasKey(f => f.ReservationId);
        }
    }
}
