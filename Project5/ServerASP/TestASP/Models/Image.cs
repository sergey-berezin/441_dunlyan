using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestASP.Models
{
    public class _Image
    {
        public int _ImageId { get; set; }
        public byte[] byte_image { get; set; }
    }
    public class Result
    {
        public int ResultId { get; set; }

        public double res { get; set; }

        public string EmotionName { get; set; }

        public int ImageId { get; set; }
    }
    public class Hashcode
    {
        public int hashkey { get; set; }

        [Key]
        [ForeignKey(nameof(_Image))]
        public int ImageId { get; set; }
    }
}
