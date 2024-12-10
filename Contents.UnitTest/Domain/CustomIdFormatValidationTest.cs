using Contents.Domain.Utils.CustomValidations;
using NUnit.Framework;
using System;
using System.Text;

namespace Contents.UnitTest.Domain
{
    public class CustomIdFormatValidationTest
    {
        private CustomIdFormatValidation _customIdFormatValidation;

        public CustomIdFormatValidationTest()
        {
            _customIdFormatValidation = new CustomIdFormatValidation();
        }

        [Test]
        public void ValidateIdFormat_ShouldBeReturn_True()
        {
            var id = Guid.NewGuid();
            var result = _customIdFormatValidation.IsValid(id);

            Assert.True(result);
        }

        [Test]
        [TestCase(5, true)]
        [TestCase(10, false)]
        [TestCase(15, true)]
        public void ValidateIdFormat_ShouldBeReturn_False(int size, bool lowerCase)
        {
            var id = RandomString(size, lowerCase);
            var result = _customIdFormatValidation.IsValid(id);

            Assert.False(result);
        }

        private string RandomString(int size, bool lowerCase = false)
        {
            Random _random = new Random();
            var builder = new StringBuilder(size);

            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26;

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
    }
}
