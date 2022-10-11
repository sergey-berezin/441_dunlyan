using System.Diagnostics;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace EmotionFer_Plus{
    public  class EmotionFerPlus
    {       
       public InferenceSession session;
       public EmotionFerPlus()
       {
         this.session = new InferenceSession("../Lib_EmotionFerPlus/emotion-ferplus-7.onnx");
       }
       public  async Task<IEnumerable<(string,double)>> EmotionalAnalysisAsync(byte [] image,CancellationTokenSource cts)
       {     
            var result=new List<(string,double)>();
            var _Stream = new MemoryStream(image);
            Image<Rgb24> _image;
            _image = await Image.LoadAsync<Rgb24>(_Stream,cts.Token);
            if (cts.IsCancellationRequested)
                return result;

             _image.Mutate(ctx => {
                    ctx.Resize(new Size(64,64));
             });
            if (cts.IsCancellationRequested)
                return result;
            await Task.Factory.StartNew(() =>
            {
                var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("Input3", GrayscaleImageToTensor(_image)) };
                IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results;
                Monitor.Enter(session);
                try
                {
                    results = session.Run(inputs);
                    var emotions =EmotionFerPlus.Softmax(results.First(v => v.Name == "Plus692_Output_0").AsEnumerable<float>().ToArray());
                    string[] keys = { "neutral", "happiness", "surprise", "sadness", "anger", "disgust", "fear", "contempt" };
                    foreach(var i in keys.Zip(emotions))
                    {
                        result.Add(i);
                    }
                }
                finally
                {
                    Monitor.Exit(session); 
                }
            },TaskCreationOptions.LongRunning);
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

