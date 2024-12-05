using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Contents.Domain.TrainingProgram;
using WLS.KafkaMessenger;

namespace Contents.Services.Interfaces
{
    public interface IKafkaService
    {
        Task SendTrainingProgramKafkaMessage(TrainingProgram user, string subject);

        Task SendTrainingProgramDeleteKafkaMessage(Guid id, int updatedBy, string subject);
    }
}
