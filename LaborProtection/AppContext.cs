using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LaborProtection.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LaborProtection
{
    internal class AppContext : DbContext
    {
        public DbSet<Employee> employees { get; set; }
        public DbSet<Exam> exams{ get; set; }
        public DbSet<User> users{ get; set; }
        public DbSet<Change> changes { get; set; }
        public DbSet<Dictionary> dictionary { get; set; }

        public AppContext()
        {
            Database.EnsureCreated();

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=labor;Username=postgres;Password=797827");
        }
    }
}