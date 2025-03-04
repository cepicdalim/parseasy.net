# Parseasy

Parseasy is a modern and efficient alternative to Regex for text pattern matching and extraction in .NET applications. It provides a simple, type-safe API for parsing text based on patterns with typed placeholders.

## Features

- **Type-safe parsing**: Extract values with automatic type conversion
- **Simple pattern syntax**: Use intuitive placeholders like `{name:string}`, `{age:int}`
- **Performance optimized**: Compiled patterns and efficient memory usage
- **Thread-safe**: Safe to use in multi-threaded applications
- **Comprehensive API**: TryParse, Parse, IsMatch, and FindAllMatches methods

## Installation

```bash
dotnet add package Parseasy
```

## Usage

### Basic Parsing

```csharp
using Parseasy;

string input = "Name: John, Age: 30";
string pattern = "Name: {name:string}, Age: {age:int}";

if (Parser.TryParse(input, pattern, out var result))
{
    string name = result.GetValue<string>("name");
    int age = result.GetValue<int>("age");
    Console.WriteLine($"Name: {name}, Age: {age}");
}
```

### Parse Method (with Exception Handling)

```csharp
try
{
    var result = Parser.Parse(input, pattern);
    string name = result.GetValue<string>("name");
    int age = result.GetValue<int>("age");
    Console.WriteLine($"Name: {name}, Age: {age}");
}
catch (ParseException ex)
{
    Console.WriteLine($"Parsing failed: {ex.Message}");
}
```

### Checking if a String Matches a Pattern

```csharp
if (Parser.IsMatch(input, pattern))
{
    Console.WriteLine("Input matches the pattern.");
}
```

### Finding All Matches

```csharp
string input = "Name: John, Age: 30\nName: Jane, Age: 25";
string pattern = "Name: {name:string}, Age: {age:int}";

var matches = Parser.FindAllMatches(input, pattern);
foreach (var match in matches)
{
    string name = match.GetValue<string>("name");
    int age = match.GetValue<int>("age");
    Console.WriteLine($"Name: {name}, Age: {age}");
}
```

```csharp
string input = @"
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
string pattern = "{key:string}={value:string}";

var matches = Parser.FindAllMatches(input, pattern);
foreach (var entry in configEntries)
{
    string key = entry.GetValue<string>("key");
    string value = entry.GetValue<string>("value");
    config[key] = value;
    Console.WriteLine($"  {key} = {value}");
}
```


## Supported Types

Parseasy supports the following types in placeholders:

- `string`: Any text
- `int`: Integer numbers
- `decimal`, `double`, `float`: Floating-point numbers
- `bool`: Boolean values (true/false)
- `date`, `datetime`: Date and time values
- `guid`: GUID values
- `email`: Email addresses
- `url`: URLs
- `phone`: Phone numbers
- `ip`: IP addresses

## Performance Considerations

- Patterns are compiled and cached for optimal performance
- Memory usage is minimized by reusing pattern objects
- Thread-safe implementation for concurrent access

## License

MIT