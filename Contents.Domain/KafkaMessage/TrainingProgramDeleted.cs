using System;

namespace Contents.Domain.KafkaMessage
{
    public class TrainingProgramDeleted
    {
        public Guid Id { get; set; }
        public int UpdatedBy { get; set; }
    }
}
