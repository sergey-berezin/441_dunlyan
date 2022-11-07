using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Project3
{
   public class DataContext :   DbContext
   {
        public DbSet<Image> Images { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Hashkey> Hashkeys { get; set; }

        public DbSet<Bytedata> Bytedatas { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=C:\\Users\\ydl74\\source\\repos\\Project3\\Project3\\ImageAnalysis.db");
    }

    
    public class Image
    {
        public int ImageId { get; set; }
        public string Path { get; set; }

        public Image(string arg_Path)
        {
            Path=arg_Path;
            results = new List<Result>();
        }
        public List<Result> results { get; set; }
        
        public Image()
        {   
            results = new List<Result>();
        }
        public Hashkey hashkey { get; set; }
        public Bytedata bytedata { get; set; }
        public string get_Result
        {
            get
            {
                string str = "";
                if (results != null)
                {
                    foreach (var item in results)
                    {
                        str += $"{item.EmotionName}:   {item.res} \n";
                    }
                }
                return str;
            }
        }
    }

    public class Result
    {
        public int ResultId { get; set; }

        public double res { get; set; }
        
        public string EmotionName { get; set; }

        public int ImageId { get; set; }
        public Image Image { get; set; }

    }

    public class Hashkey
    {
        public int hashkey { get; set; }

        [Key]
        [ForeignKey(nameof(Image))]
        public int ImageId { get; set; }
        public Image Image { get; set; }
    }

    public class Bytedata
    {
        public byte[] byte_image { get; set; }

        [Key]
        [ForeignKey(nameof(Image))]
        public int ImageId { get; set; }
        public Image Image { get; set; }
    }
}
