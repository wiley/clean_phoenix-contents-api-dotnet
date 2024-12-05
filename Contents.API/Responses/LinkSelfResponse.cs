namespace Contents.API.Responses
{
    public class LinkSelfResponse
    {
        public Self Self { get; set; }

        public LinkSelfResponse()
        {
            Self ??= new Self();
        }
    }

    public class Self
    {
        public string Href { get; set; }
    }
}
