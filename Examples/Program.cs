using System;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parseasy Examples");
            Console.WriteLine("================\n");

            Console.WriteLine("1. Basic Example");
            Console.WriteLine("2. Advanced Example");
            Console.Write("\nSelect an example to run (1-2): ");

            string input = Console.ReadLine();
            Console.WriteLine();

            switch (input)
            {
                case "1":
                    BasicExamples.Run();
                    break;
                case "2":
                    AdvancedExamples.Run();
                    break;
                default:
                    Console.WriteLine("Invalid selection. Running Basic Example by default.");
                    BasicExamples.Run();
                    break;
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}