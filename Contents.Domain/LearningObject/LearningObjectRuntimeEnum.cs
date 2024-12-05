using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contents.Domain.LearningObject
{
    public class LearningObjectRuntimeEnum
    {
        public static readonly string ASSESSMENT = "ASSESSMENT";
        public static readonly string AUDIO_MP3 = "AUDIO_MP3";
        public static readonly string DISC = "DISC";
        public static readonly string DOCUMENT_PDF = "DOCUMENT_PDF";
        public static readonly string DOCUMENT_EDITORIAL = "DOCUMENT_EDITORIAL";
        public static readonly string INTERACTIVE_ASSESS_AND_TEST = "INTERACTIVE_ASSESS_AND_TEST";
        public static readonly string INTERACTIVE_SCORM = "INTERACTIVE_SCORM";
        public static readonly string LPI360 = "LPI360";
        public static readonly string PICTURE_FILE = "PICTURE_FILE";
        public static readonly string TRAINING_BLENDEDX = "TRAINING_BLENDEDX";
        public static readonly string VIDEO_MP4 = "VIDEO_MP4";
        public static readonly string VIDEO_YOUTUBE = "VIDEO_YOUTUBE";
        public static readonly string WEBSITE_URL = "WEBSITE_URL";

        public static List<string> GetTypes()
        {
            List<string> types = new()
            {
                ASSESSMENT,
                AUDIO_MP3,
                DISC,
                DOCUMENT_PDF,
                DOCUMENT_EDITORIAL,
                INTERACTIVE_ASSESS_AND_TEST,
                INTERACTIVE_SCORM,
                LPI360,
                PICTURE_FILE,
                TRAINING_BLENDEDX,
                VIDEO_MP4,
                VIDEO_YOUTUBE,
                WEBSITE_URL
            };

            return types;
        }
    }
}
