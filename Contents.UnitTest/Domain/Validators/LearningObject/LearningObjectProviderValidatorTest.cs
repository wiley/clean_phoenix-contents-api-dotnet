using Contents.Domain.Validators.LearningObject;
using NUnit.Framework;

namespace Contents.UnitTest.Domain.Validators
{
    public class LearningObjectProviderValidatorTest
    {
        private LearningObjectProviderValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new LearningObjectProviderValidator();
        }

        [Test]
        [TestCase("CROSSKNOWLEDGE")]
        [TestCase("CUSTOMER")]
        public void IsValid_Success(string learningObjectType)
        {
            Assert.IsTrue(_validator.IsValid(learningObjectType));
        }

        [Test]
        [TestCase("ANY_RANDOM_STRING")]
        public void IsValid_Fail(string learningObjectType)
        {
            Assert.IsFalse(_validator.IsValid(learningObjectType));
        }

    }
}
