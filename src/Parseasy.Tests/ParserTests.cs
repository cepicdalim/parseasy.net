using System;
using System.Linq;
using NUnit.Framework;

namespace Parseasy.Tests
{
    public class ParserTests
    {
        [Test]
        public void TryParse_ValidInput_ReturnsTrue()
        {
            // Arrange
            string input = "Name: John, Age: 30";
            string pattern = "Name: {name:string}, Age: {age:int}";

            // Act
            bool result = Parser.TryParse(input, pattern, out var parseResult);

            // Assert
            Assert.That(result);
            Assert.That(parseResult, Is.Not.Null);
            Assert.That(parseResult.GetValue<string>("name"), Is.EqualTo("John"));
            Assert.That(parseResult.GetValue<int>("age"), Is.EqualTo(30));
        }

        [Test]
        public void TryParse_InvalidInput_ReturnsFalse()
        {
            // Arrange
            string input = "Name: John, Age: thirty";
            string pattern = "Name: {name:string}, Age: {age:int}";

            // Act
            bool result = Parser.TryParse(input, pattern, out var parseResult);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(parseResult, Is.Null);
        }

        [Test]
        public void Parse_ValidInput_ReturnsParseResult()
        {
            // Arrange
            string input = "Name: John, Age: 30";
            string pattern = "Name: {name:string}, Age: {age:int}";

            // Act
            var parseResult = Parser.Parse(input, pattern);

            // Assert
            Assert.That(parseResult, Is.Not.Null);
            Assert.That(parseResult.GetValue<string>("name"), Is.EqualTo("John"));
            Assert.That(parseResult.GetValue<int>("age"), Is.EqualTo(30));
        }

        [Test]
        public void Parse_InvalidInput_ThrowsParseException()
        {
            // Arrange
            string input = "Name: John, Age: thirty";
            string pattern = "Name: {name:string}, Age: {age:int}";

            // Act & Assert
            Assert.Throws<ParseException>(() => Parser.Parse(input, pattern));
        }

        [Test]
        public void IsMatch_ValidInput_ReturnsTrue()
        {
            // Arrange
            string input = "Name: John, Age: 30";
            string pattern = "Name: {name:string}, Age: {age:int}";

            // Act
            bool result = Parser.IsMatch(input, pattern);

            // Assert
            Assert.That(result);
        }

        [Test]
        public void IsMatch_InvalidInput_ReturnsFalse()
        {
            // Arrange
            string input = "Name: John, Age: thirty";
            string pattern = "Name: {name:string}, Age: {age:int}";

            // Act
            bool result = Parser.IsMatch(input, pattern);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void FindAllMatches_MultipleMatches_ReturnsAllMatches()
        {
            // Arrange
            string input = "Name: John, Age: 30\nName: Jane, Age: 25";
            string pattern = "Name: {name:string}, Age: {age:int}";

            // Act
            var matches = Parser.FindAllMatches(input, pattern).ToList();

            // Assert
            Assert.That(matches.Count, Is.EqualTo(2));
            Assert.That(matches[0].GetValue<string>("name"), Is.EqualTo("John"));
            Assert.That(matches[0].GetValue<int>("age"), Is.EqualTo(30));
            Assert.That(matches[1].GetValue<string>("name"), Is.EqualTo("Jane"));
            Assert.That(matches[1].GetValue<int>("age"), Is.EqualTo(25));
        }

        [Test]
        public void TryGetValue_ExistingName_ReturnsTrue()
        {
            // Arrange
            string input = "Name: John, Age: 30";
            string pattern = "Name: {name:string}, Age: {age:int}";
            Parser.TryParse(input, pattern, out var parseResult);

            // Act
            bool result = parseResult.TryGetValue<string>("name", out var name);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(parseResult.TryGetValue<int>("age", out var age), Is.True);
            Assert.That(age, Is.EqualTo(30));
            Assert.That(name, Is.EqualTo("John"));
        }

        [Test]
        public void TryGetValue_NonExistingName_ReturnsFalse()
        {
            // Arrange
            string input = "Name: John, Age: 30";
            string pattern = "Name: {name:string}, Age: {age:int}";
            Parser.TryParse(input, pattern, out var parseResult);

            // Act
            bool result = parseResult.TryGetValue<string>("email", out var email);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(email, Is.Null);
        }

        [Test]
        public void GetNames_ReturnsAllNames()
        {
            // Arrange
            string input = "Name: John, Age: 30";
            string pattern = "Name: {name:string}, Age: {age:int}";
            var parseResult = Parser.Parse(input, pattern);

            // Act
            var names = parseResult.GetNames().ToList();

            // Assert
            Assert.That(names.Count, Is.EqualTo(2));
            Assert.That(names.Contains("name"));
            Assert.That(names.Contains("age"));
        }

        [Test]
        public void Parse_ComplexPattern_ReturnsCorrectValues()
        {
            // Arrange
            string input = "User: John Doe, Email: john@example.com, DOB: 1990-05-15, Active: true";
            string pattern = "User: {name:string}, Email: {email:email}, DOB: {dob:date}, Active: {active:bool}";

            // Act
            var parseResult = Parser.Parse(input, pattern);

            // Assert
            Assert.That(parseResult.GetValue<string>("name"), Is.EqualTo("John Doe"));
            Assert.That(parseResult.GetValue<string>("email"), Is.EqualTo("john@example.com"));
            Assert.That(parseResult.GetValue<DateTime>("dob"), Is.EqualTo(new DateTime(1990, 5, 15)));
            Assert.That(parseResult.GetValue<bool>("active"));
        }

        [Test]
        public void Parse_Multiline_ComplexPattern_ReturnsCorrectValues()
        {
            // Arrange
            string input = @"
            key1=value1
            #ignore
            key2=value2
            //ignore
            key3=value3
            ";
            string pattern = "{key:string}={value:string}";

            // Act
            var parseResult = Parser.FindAllMatches(input, pattern).ToList();

            // Assert
            Assert.That(parseResult.Count, Is.EqualTo(3));

            for (int i = 0; i < 3; i++)
            {
                Assert.That(parseResult[i].GetValue<string>("key").Trim(), Is.EqualTo($"key{i + 1}"));
                Assert.That(parseResult[i].GetValue<string>("value").Trim(), Is.EqualTo($"value{i + 1}"));
            }
        }
    }
}
