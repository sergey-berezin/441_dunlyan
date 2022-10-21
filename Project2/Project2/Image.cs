using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
    public class image_analysis
    {
        public image_analysis() { }
        public IEnumerable<(string, double)> Result;
        public string get_Result
        {
            get
            {
                string str = "";
                if (Result != null)
                {
                    str += "Analysis results for this image: \n";
                    foreach (var item in Result)
                    {
                        str += $"{item.Item1}:   {item.Item2} \n";
                    }
                }
                return str;

            }
        }
        public image_analysis(string arg_CoverImage)
        {
            CoverImage = arg_CoverImage;
        }
        public void set_values(IEnumerable<(string, double)> arg)
        {
            Result = arg;
        }
        public string CoverImage { get; set; }
    }
    public class Images
    {
        public Images() { }
        public List<image_analysis> image_s;
    }

}

