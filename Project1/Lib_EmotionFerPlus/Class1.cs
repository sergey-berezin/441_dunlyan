using System.Diagnostics;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace EmotionFer_Plus{
    public static class EmotionFerPlus
    {       
       public static async Task EmotionalAnalysisAsync(string [] images,bool [] cancel_Task)
       {
            using var session = new InferenceSession("../Lib_EmotionFerPlus/emotion-ferplus-7.onnx");  
            //CancellationTokenSource используется для подачи сигнала CancellationToken на запрос отмены.
            foreach(var kv in session.InputMetadata)
            Console.WriteLine($"{kv.Key}: {EmotionFerPlus.MetadataToString(kv.Value)}");
            foreach(var kv in session.OutputMetadata)
            Console.WriteLine($"{kv.Key}: {EmotionFerPlus.MetadataToString(kv.Value)}]");
            int i=0;
            foreach (var str in images)
            {   
               CancellationTokenSource cts = new  CancellationTokenSource(); 
               await ProcessAsync(session,str,cts,cts.Token,cancel_Task[i++]);
            }
            
       }
        public static async Task ProcessAsync(InferenceSession session,string str,CancellationTokenSource cts,CancellationToken token,bool cancel_Task)
        {       
            if(cancel_Task)
              cts.Cancel();
            if (token.IsCancellationRequested) 
            {
                Console.WriteLine($"The task '{str} ' was canceled!");
            }   
            else{
                    await Task.Run(()=> {       
                    using Image<Rgb24> image = Image.Load<Rgb24>(str);
                    image.Mutate(ctx => {
                    ctx.Resize(new Size(64,64));
                    //ctx.Grayscale();
                    });
                    var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("Input3", GrayscaleImageToTensor(image)) };
                    using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);
                    var emotions =EmotionFerPlus.Softmax(results.First(v => v.Name == "Plus692_Output_0").AsEnumerable<float>().ToArray());

                    string[] keys = { "neutral", "happiness", "surprise", "sadness", "anger", "disgust", "fear", "contempt" };
                
                    Console.WriteLine($"{str}'s result of the analysis:");
                    foreach(var i in keys.Zip(emotions))
                    Console.WriteLine($"{i.First}: {i.Second}"); 
                    });
            }            
        }
        public static DenseTensor<float> GrayscaleImageToTensor(Image<Rgb24> img)
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
        public static string MetadataToString(NodeMetadata metadata)
        => $"{metadata.ElementType}[{String.Join(",", metadata.Dimensions.Select(i => i.ToString()))}]";
        public static float[] Softmax(float[] z)
        {
            var exps = z.Select(x => Math.Exp(x)).ToArray();
            var sum = exps.Sum();
            return exps.Select(x => (float)(x / sum)).ToArray();
        }
    
    }   
}

