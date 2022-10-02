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
        Console.WriteLine("Application started.");
        try
        {   
           await EmotionFerPlus.EmotionalAnalysisAsync(images,cancel_Task);
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Task was cancelled");
        }
        Console.WriteLine("Application ending.");
    }
}
