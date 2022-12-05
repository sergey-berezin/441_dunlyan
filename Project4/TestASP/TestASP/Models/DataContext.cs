using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
namespace TestASP.Models
{
    public class DataContext :DbContext
    {
        public DbSet<_Image> Images { get; set; } = null!;
        public DbSet<Result> Results { get; set; } = null!;

        public DbSet<Hashcode> HashCodes { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=Image.db");
        public DataContext(DbContextOptions<DataContext> options)
       : base(options)
        {

        }
    }
}
