using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestASP.Models;
using EmotionFer_Plus;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security;

namespace TestASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly DataContext _context;
        EmotionFerPlus Emotion = new EmotionFerPlus();
        public ImagesController(DataContext context)
        {
            _context = context;
        }
        // POST: api/Images
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        // return <int id, string res>
        public async Task<string> PostImage(_Image image, CancellationToken cts)
        {
            var img_hashcode =image.byte_image.GetHashCode();
            if (!_context.HashCodes.Any(h => h.hashkey.Equals(img_hashcode)))
            {
                if (!_context.Images.Any(b => b.byte_image.Equals(image.byte_image)))
                {
                    _context.Images.Add(image);
                    _context.SaveChanges();

                    var ima = _context.Images.OrderBy(d => d._ImageId).Last();

                    _context.HashCodes.Add(new Hashcode { hashkey= img_hashcode ,ImageId=ima._ImageId});
                    _context.SaveChanges();
         
                    var res = await Images_Analysis(image.byte_image,cts);
                    string str = "";
                    foreach(var item in res)
                    {
                        _context.Results.Add(new Result {EmotionName=item.Item1 ,res=item.Item2,ImageId=ima._ImageId});
                        str+=item.Item1+ " " + item.Item2+"\n";
                    }
                    _context.SaveChanges();
                    return str;
                }
                return "";
            }
            return "";
            //return CreatedAtAction("GetImage", new { id =image._ImageId }, image);
        }

        [HttpGet("AllImages")]
        public async Task<List<byte[]>> GetAllImages()
        {
           // List(byte[], string) res_list = new List(byte[], string)();
            List<byte[]> res_list = new List<byte[]>();
            if (_context.Images.Any())
            {
                foreach (var item in _context.Images)
                {
                    /*string str = "";
                    var query = _context.Results.Where(r => r.ImageId.Equals(item._ImageId));
                    foreach (var item2 in query)
                    {
                        str += item2.EmotionName + " " + item2.res + "\n";
                    }*/
                    res_list.Add((item.byte_image));

                }
            }

            return res_list;
        }
        [HttpGet("AllResults")]
        public async Task<List<string>> GetAllResults()
        {
            List<string> res_list = new List<string>();
            
            if (_context.Images.Any())
            {
                foreach (var item in _context.Images)
                {
                    string str = "";
                    var query = _context.Results.Where(r => r.ImageId.Equals(item._ImageId));
                    foreach (var item2 in query)
                    {
                        str += item2.EmotionName + " " + item2.res + "\n";
                    }
                    res_list.Add(str);

                }
            }

            return res_list;
        }
        /*[HttpGet("All")]
        public async Task<List<string>> GetAll()
        {
            *//*List(byte[], string) res_list = new List(byte[], string)();*//*
            List< string> res_list = new List<string>();
            if (_context.Images.Any())
            {
                foreach (var item in _context.Images)
                {
                    string str = "";
                    var query = _context.Results.Where(r => r.ImageId.Equals(item._ImageId));
                    foreach (var item2 in query)
                    {
                        str += item2.EmotionName + " " + item2.res + "\n";
                    }
                   
                    res_list.Add(str);

                }
            }

            return res_list;
        }*/
        // GET: api/Images/5
        [HttpGet]
        public async Task<List<byte[]>> GetImage(string Emotion)
        {
            List<byte[]> resImages = new List<byte[]>();

            if (_context.Images.Any())
            {
                foreach(var item in _context.Images)
                {
                    var query = _context.Results.Where(r => r.ImageId.Equals(item._ImageId));
                    var max_res=query.Max(r => r.res);
                    var query1= query.Where(r => r.res.Equals(max_res)).FirstOrDefault();

                    if (query1.EmotionName.Equals(Emotion))
                    {
                        var query2 = _context.Images.Where(i => i._ImageId.Equals(query1.ImageId)).First();
                        resImages.Add(query2.byte_image);
                    }
                }
                
            }
            return resImages;

        }
        [HttpDelete("All")]
        public async Task<IActionResult> DeleteALL()
        {
            
            if (!(_context.Images.Any() || _context.HashCodes.Any() || _context.Results.Any()))
            {
                return NotFound();
            }
            if (_context.Images.Any())
                foreach (var item in _context.Images)
                {
                    _context.Images.Remove(item);
                }
            if (_context.HashCodes.Any())
                foreach (var item in _context.HashCodes)
                {
                    _context.HashCodes.Remove(item);
                }
            if (_context.Results.Any())
                foreach (var item in _context.Results)
                {
                    _context.Results.Remove(item);
                }
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e._ImageId == id);
        }
        private async Task<List<(string, double)>> Images_Analysis( byte[] img_byte,  CancellationToken cts)
        {

            var rt = await Task.Run(async () =>
            {
                var res = await Emotion.EmotionalAnalysisAsync(img_byte, cts);

                res.Sort((x, y) => -(x.Item2.CompareTo(y.Item2)));
                return res;
            }, cts);
           
            return rt;
        }
        public class Postdata
        {
            public _Image image { get; set; }
            public CancellationTokenSource cts { get; set; }
        }
    }
}
