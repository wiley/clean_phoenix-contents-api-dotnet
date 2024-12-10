using System.ComponentModel.DataAnnotations;

namespace Contents.API.Requests.Publisher
{
    public class PublisherCreateOrUpdateRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
