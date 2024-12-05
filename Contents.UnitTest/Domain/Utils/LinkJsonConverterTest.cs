using Contents.Domain.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Contents.UnitTest.Domain.Utils
{
    public class LinkJsonConverterTest
    {
        private LinkJsonConverter _converter;

        [SetUp]
        public void Setup()
        {
            _converter = new LinkJsonConverter();
        }

        [Test]
        public void Write_WithSchemeHostSet_ShouldWriteModifiedValue()
        {
            // Set the SchemeHost
            LinkJsonConverter.SchemeHost = "https://localhost";

            // Mock the input value
            var value = "ADDHOST/example";

            // Create a MemoryStream to capture the output
            var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            // Mock the expected output value
            var expectedOutput = "https://localhost/example";

            // Act
            _converter.Write(writer, value, new JsonSerializerOptions());
            writer.Flush();

            // Convert the MemoryStream to a string
            var output = Encoding.UTF8.GetString(stream.ToArray());

            // Remove the surrounding quotes from the output
            output = output.Trim('"');

            // Assert
            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void Write_WithNullSchemeHost_ShouldWriteOriginalValue()
        {
            // Set the SchemeHost to null
            LinkJsonConverter.SchemeHost = null;

            // Mock the input value
            var value = "ADDHOST/example";

            // Create a StringWriter to capture the output
            var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, value, new JsonSerializerOptions());
            writer.Flush();

            // Get the output from StringWriter
            var output = Encoding.UTF8.GetString(stream.ToArray());

            // Remove the surrounding quotes from the output
            output = output.Trim('"');

            // Assert
            Assert.AreEqual(value, output);
        }

        [Test]
        public void Read_ShouldReturnInputValue()
        {
            // Mock the input value
            var input = "example";

            // Create a Utf8JsonReader with the input value
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes($"\"{input}\""));

            // Act
            reader.Read();
            var result = _converter.Read(ref reader, typeof(string), new JsonSerializerOptions());

            // Assert
            Assert.AreEqual(input, result);
        }

    }
}
