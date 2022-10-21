using System.Diagnostics;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Threading.Tasks.Dataflow;

namespace EmotionFer_Plus{
    public  class EmotionFerPlus
    {       
       public InferenceSession session;
       public EmotionFerPlus()
       {
            using var modelStream = typeof(EmotionFerPlus).Assembly.GetManifestResourceStream("emotion.onnx");
            using var memoryStream = new MemoryStream();
            if (modelStream != null)
                modelStream.CopyTo(memoryStream);
            this.session = new InferenceSession(memoryStream.ToArray());
       }
       public  async Task<List<(string, double)>> EmotionalAnalysisAsync(byte [] image,CancellationTokenSource cts)
       {     
            List<(string,double)> result=new List<(string,double)>();
           
        
            await Task<List<(string, double)>>.Factory.StartNew(() =>
            {   
                 var _Stream = new MemoryStream(image);
                Image<Rgb24> _image=Image.Load<Rgb24>(_Stream);
        
                _image.Mutate(ctx => {
                    ctx.Resize(new Size(64,64));
                });
                var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("Input3", GrayscaleImageToTensor(_image)) };
                IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results;
                Monitor.Enter(session);
                try
                {
                    results = session.Run(inputs);
                }
                finally
                {
                    Monitor.Exit(session); 
                }
                var emotions =EmotionFerPlus.Softmax(results.First(v => v.Name == "Plus692_Output_0").AsEnumerable<float>().ToArray());
                string[] keys = { "neutral", "happiness", "surprise", "sadness", "anger", "disgust", "fear", "contempt" };
                foreach(var i in keys.Zip(emotions))
                {
                   result.Add(i);
                }
                return result;
            },cts.Token,TaskCreationOptions.LongRunning,TaskScheduler.Default);
            return result;
       }
        public  DenseTensor<float> GrayscaleImageToTensor(Image<Rgb24> img)
        {
            var w = img.Width;
            var h = img.Height;
            var t = new DenseTensor<float>(new[] { 1, 1, h, w });

            img.ProcessPixelRows(pa => 
            {
                for (int y = 0; y < h; y++)
                {           
                    Span<Rgb24> pixelSpan = pa.GetRowSpan(y);
                for (int x = 0; x < w; x++)
                {
                    t[0, 0, y, x] = pixelSpan[x].R; // B and G are the same
                }
                }
            });
            return t;
        }
        public  string MetadataToString(NodeMetadata metadata)
        => $"{metadata.ElementType}[{String.Join(",", metadata.Dimensions.Select(i => i.ToString()))}]";
        public static float[] Softmax(float[] z)
        {
            var exps = z.Select(x => Math.Exp(x)).ToArray();
            var sum = exps.Sum();
            return exps.Select(x => (float)(x / sum)).ToArray();
        }
    
    }   
}

