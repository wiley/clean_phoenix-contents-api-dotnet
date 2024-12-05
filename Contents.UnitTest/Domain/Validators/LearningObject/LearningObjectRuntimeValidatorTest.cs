using Contents.Domain.Validators.LearningObject;
using NUnit.Framework;

namespace Contents.UnitTest.Domain.Validators
{
    public class LearningObjectRuntimeValidatorTest
    {
        private LearningObjectRuntimeValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new LearningObjectRuntimeValidator();
        }

        [Test]
        [TestCase("ASSESSMENT")]
        [TestCase("AUDIO_MP3")]
        [TestCase("DISC")]
        [TestCase("DOCUMENT_PDF")]
        [TestCase("DOCUMENT_EDITORIAL")]
        [TestCase("INTERACTIVE_ASSESS_AND_TEST")]
        [TestCase("INTERACTIVE_SCORM")]
        [TestCase("LPI360")]
        [TestCase("PICTURE_FILE")]
        [TestCase("TRAINING_BLENDEDX")]
        [TestCase("VIDEO_MP4")]
        [TestCase("VIDEO_YOUTUBE")]
        [TestCase("WEBSITE_URL")]
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
