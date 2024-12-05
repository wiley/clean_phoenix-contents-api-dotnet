using Contents.Domain.Validators;
using NUnit.Framework;
using System.Collections.Generic;

namespace Contents.UnitTest.Domain.Validators
{
    public class MaxItemsValidatorTest
    {
        private MaxItemsValidator _maxItemsValidator;

        [SetUp]
        public void Setup()
        {
            int maxItems = 2;
            _maxItemsValidator = new MaxItemsValidator(maxItems);

        }

        [Test]
        public void IsValid_Success()
        {
            List<int> value = new() { 1, 2 };
            Assert.IsTrue(_maxItemsValidator.IsValid(value));
        }

        [Test]
        public void IsValid_Fail()
        {
            List<int> value = new() { 1, 2, 3 };
            Assert.IsFalse(_maxItemsValidator.IsValid(value));
        }

    }
}
