using System.Collections.Generic;

namespace Contents.Domain.LearningObject
{
    public class LearningObjectTypeEnum
    {
        public static readonly string ASSESSMENT = "ASSESSMENT";
        public static readonly string AUDIO = "AUDIO";
        public static readonly string DOCUMENT = "DOCUMENT";
        public static readonly string INTERACTIVE = "INTERACTIVE";
        public static readonly string PICTURE = "PICTURE";
        public static readonly string TRAINING = "TRAINING";
        public static readonly string VIDEO = "VIDEO";
        public static readonly string WEBSITE = "WEBSITE";

        public static List<string> GetTypes()
        {
            List<string> types = new()
            {
                ASSESSMENT,
                AUDIO,
                DOCUMENT,
                INTERACTIVE,
                PICTURE,
                TRAINING,
                VIDEO,
                WEBSITE
            };

            return types;
        }
    }
}
