using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using notificationapi.Models;

namespace notificationapi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {


        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ViewNotification> ViewNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>().ToTable("NOTIFICATIONS", schema: "dbo");
            modelBuilder.Entity<ViewNotification>().HasNoKey().ToView("ViewNotification_Query");

            base.OnModelCreating(modelBuilder);
        }
    }
}