using System.Diagnostics;
using EmotionFer_Plus;
class program 
{   
    static readonly string [] images=
    {
            "../images/happyness.png",  
            "../images/neutral.jpg",
            "../images/sadness.jpg",
            "../images/surprise.jpg"
    };   // В массиве images хранятся  пути  изображении 
    
    static bool [] cancel_Task=
    {
        true,      // Если true то отменить задачу , иначе выполнить задачу
        true,
        false,
        true
    } ;
    static async Task  Main(string []args)
    {   
       string []results;
        Console.WriteLine("Application started.");

        try
        {   
          results=await EmotionFerPlus.EmotionalAnalysisAsync(images,cancel_Task);

          int i=0;
          foreach(var str in results)
          { 
             if(!cancel_Task[i])
                System.Console.WriteLine($"{images[i]}'s result of the analysis:");
            System.Console.WriteLine(str); 
            i++;
          }     
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Task was cancelled");
        }
        finally
        {
           
             Console.WriteLine("Application ending.");
        }
    }
}
