using Contents.Domain.Validators.LearningObject;
using NUnit.Framework;

namespace Contents.UnitTest.Domain.Validators
{
    public class LearningObjectTypeValidatorTest
    {
        private LearningObjectTypeValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new LearningObjectTypeValidator();
        }

        [Test]
        [TestCase("ASSESSMENT")]
        [TestCase("AUDIO")]
        [TestCase("DOCUMENT")]
        [TestCase("INTERACTIVE")]
        [TestCase("PICTURE")]
        [TestCase("TRAINING")]
        [TestCase("VIDEO")]
        public void IsValid_Success(string learningObjectType)
        {
            Assert.IsTrue(_validator.IsValid(learningObjectType));
        }

        [Test]
        [TestCase("ANY_RANDOM_STRING")]
        [TestCase("CROSSKNOWLEDGE")]
        public void IsValid_Fail(string learningObjectType)
        {
            Assert.IsFalse(_validator.IsValid(learningObjectType));
        }

    }
}
