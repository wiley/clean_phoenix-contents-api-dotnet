using System.Collections.Generic;

namespace Contents.Domain.LearningObject
{
    public class LearningObjectProviderEnum
    {
        public static readonly string CUSTOMER = "CUSTOMER";
        public static readonly string CROSSKNOWLEDGE = "CROSSKNOWLEDGE";

        public static List<string> GetTypes()
        {
            List<string> types = new()
            {
                CUSTOMER,
                CROSSKNOWLEDGE
            };

            return types;
        }
    }
}
