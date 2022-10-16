using System.Diagnostics;
using EmotionFer_Plus;
class program 
{     
   static object l= new object();
    static readonly string [] images=
    {
            "images/happyness.png",  
            "images/neutral.jpg",
            "images/sadness.jpg",
            "images/surprise.jpg"
    };   // В массиве images хранятся  пути  изображении 
    
    static bool [] cancel_Task=
    {
        true,      // Если true то отменить задачу , иначе выполнить задачу
        true,
        false,
        false,
    } ;
    static async Task ResultsAsync(EmotionFerPlus Emotion,string img_address,CancellationTokenSource cts)
    {
         var image=await File.ReadAllBytesAsync(img_address);
         var result=await Emotion.EmotionalAnalysisAsync(image,cts);
         await Task.Factory.StartNew(()=> 
         {  
            lock(l) 
            {   
                System.Console.WriteLine($"the result of analys {img_address}: ");
                foreach(var item in result) 
                {   
                    Console.WriteLine($"{item.Item1}: {item.Item2}");
                }
            }
         },TaskCreationOptions.LongRunning);
    }
    static async Task  Main(string []args)
    {   
       EmotionFerPlus Emotion=new EmotionFerPlus();
       List<CancellationTokenSource> arr_cts = new List<CancellationTokenSource>();
       var tasks=new Task[images.Length];
       Console.WriteLine("Application started.");
       try
       {   
          for(int i=0;i<images.Length;i++)
          {
            arr_cts.Add(new CancellationTokenSource());
          }
          for (int i = 0; i <images.Length; i++) 
          {
            tasks[i] = ResultsAsync (Emotion,images[i], arr_cts[i]); 
          }

          for(int i=0;i<images.Length;i++)
          {
            if(cancel_Task[i])
               arr_cts[i].Cancel();
          }
          await Task.WhenAll(tasks);
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine($"Task was cancelled");
        }
        finally
        {
           Console.WriteLine("Application ending.");
        }
    }
}
