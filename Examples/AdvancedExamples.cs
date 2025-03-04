using System;
using System.Collections.Generic;
using Parseasy;

namespace Examples
{
    public class AdvancedExamples
    {
        public static void Run()
        {
            Console.WriteLine("Parseasy Advanced Example");
            Console.WriteLine("========================");

            // Example 1: Parsing a log file entry
            string logEntry = "[2023-05-15 14:30:22] INFO [UserService] - User 12345 logged in from 192.168.1.100";
            string logPattern = "[{timestamp:datetime}] {level:string} [{service:string}] - {message:string}";

            Console.WriteLine($"Log Entry: {logEntry}");
            Console.WriteLine($"Pattern: {logPattern}");

            if (Parser.TryParse(logEntry, logPattern, out var logResult))
            {
                DateTime timestamp = logResult.GetValue<DateTime>("timestamp");
                string level = logResult.GetValue<string>("level");
                string service = logResult.GetValue<string>("service");
                string message = logResult.GetValue<string>("message");

                Console.WriteLine("Parsed log entry:");
                Console.WriteLine($"  Timestamp: {timestamp}");
                Console.WriteLine($"  Level: {level}");
                Console.WriteLine($"  Service: {service}");
                Console.WriteLine($"  Message: {message}");
            }

            Console.WriteLine();

            // Example 2: Parsing a CSV-like format
            string csvData = "John Doe,john@example.com,1980-04-12,true\nJane Smith,jane@example.com,1992-07-23,false";
            string csvPattern = "{name:string},{email:email},{dob:date},{active:bool}";

            Console.WriteLine("CSV Data:");
            Console.WriteLine(csvData);
            Console.WriteLine($"Pattern: {csvPattern}");
            Console.WriteLine("Parsed records:");

            var records = Parser.FindAllMatches(csvData, csvPattern);
            foreach (var record in records)
            {
                string name = record.GetValue<string>("name");
                string email = record.GetValue<string>("email");
                DateTime dob = record.GetValue<DateTime>("dob");
                bool active = record.GetValue<bool>("active");

                Console.WriteLine($"  Name: {name}, Email: {email}, DOB: {dob:yyyy-MM-dd}, Active: {active}");
            }

            Console.WriteLine();

            // Example 3: Parsing a configuration file
            string configFile = @"
# Server Configuration
host=192.168.1.10
port=8080
max_connections=100

# Database Configuration
db_host=localhost
db_port=5432
db_name=myapp
db_user=admin
";

            string configPattern = "{key:string}={value:string}";
            Console.WriteLine("Configuration File:");
            Console.WriteLine(configFile);
            Console.WriteLine($"Pattern: {configPattern}");
            Console.WriteLine("Parsed configuration:");

            var configEntries = Parser.FindAllMatches(configFile, configPattern);
            var config = new Dictionary<string, string>();

            foreach (var entry in configEntries)
            {
                string key = entry.GetValue<string>("key");
                string value = entry.GetValue<string>("value");
                config[key] = value;
                Console.WriteLine($"  {key} = {value}");
            }

            // Using the parsed configuration
            if (config.TryGetValue("port", out var portStr) && int.TryParse(portStr, out var port))
            {
                Console.WriteLine($"\nUsing port {port} from configuration");
            }
        }
    }
}