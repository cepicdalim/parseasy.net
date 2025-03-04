using System;
using Parseasy;

namespace Examples
{
    public class BasicExamples
    {
        public static void Run()
        {
            Console.WriteLine("Parseasy Basic Example");
            Console.WriteLine("======================");

            // Example 1: Basic parsing
            string input1 = "Name: John Doe, Age: 30";
            string pattern1 = "Name: {name:string}, Age: {age:int}";

            Console.WriteLine($"Input: {input1}");
            Console.WriteLine($"Pattern: {pattern1}");

            if (Parser.TryParse(input1, pattern1, out var result1))
            {
                string name = result1.GetValue<string>("name");
                int age = result1.GetValue<int>("age");
                Console.WriteLine($"Parsed values: Name = {name}, Age = {age}");
            }
            else
            {
                Console.WriteLine("Parsing failed.");
            }

            Console.WriteLine();

            // Example 2: More complex pattern
            string input2 = "User: Jane Smith, Email: jane@example.com, DOB: 1985-03-22, Active: true";
            string pattern2 = "User: {name:string}, Email: {email:email}, DOB: {dob:date}, Active: {active:bool}";

            Console.WriteLine($"Input: {input2}");
            Console.WriteLine($"Pattern: {pattern2}");

            try
            {
                var result2 = Parser.Parse(input2, pattern2);
                string name = result2.GetValue<string>("name");
                string email = result2.GetValue<string>("email");
                DateTime dob = result2.GetValue<DateTime>("dob");
                bool active = result2.GetValue<bool>("active");

                Console.WriteLine($"Parsed values:");
                Console.WriteLine($"  Name: {name}");
                Console.WriteLine($"  Email: {email}");
                Console.WriteLine($"  Date of Birth: {dob:yyyy-MM-dd}");
                Console.WriteLine($"  Active: {active}");
            }
            catch (ParseException ex)
            {
                Console.WriteLine($"Parsing failed: {ex.Message}");
            }

            Console.WriteLine();

            // Example 3: Finding all matches
            string input3 = "Product: Laptop, Price: 999.99\nProduct: Phone, Price: 499.50\nProduct: Headphones, Price: 149.99";
            string pattern3 = "Product:{product:string}, Price:{price:decimal}";

            Console.WriteLine($"Input with multiple matches:");
            Console.WriteLine(input3);
            Console.WriteLine($"Pattern: {pattern3}");
            Console.WriteLine("All matches:");

            var matches = Parser.FindAllMatches(input3, pattern3);
            foreach (var match in matches)
            {
                string product = match.GetValue<string>("product").Trim();
                decimal price = match.GetValue<decimal>("price");
                Console.WriteLine($"  Product: {product}, Price: ${price:0.00}");
            }
        }
    }
}