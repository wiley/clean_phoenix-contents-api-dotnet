using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WLS.KafkaMessenger.Services.Interfaces;
using Contents.Services.Interfaces;
using Contents.Domain.TrainingProgram;
using Contents.Domain.KafkaMessage;
using System.Linq;

namespace Contents.Services
{
    public class KafkaService : IKafkaService
    {
        private readonly IKafkaMessengerService _kafkaMessengerService;
        private readonly ILogger<KafkaService> _logger;

        public KafkaService(IKafkaMessengerService kafkaMessengerService, ILogger<KafkaService> logger)
        {
            _kafkaMessengerService = kafkaMessengerService;
            _logger = logger;
        }


        public async Task SendTrainingProgramKafkaMessage(TrainingProgram trainingProgram, String subject)
        {
            try
            {
                var deliveryValues = await _kafkaMessengerService.SendKafkaMessage(trainingProgram.Id.ToString(), subject, trainingProgram, "ck-phoenix-trainingProgram");
                foreach (var deliveryValue in deliveryValues.Where(v => v.Status != 0))
                {
                    _logger.LogWarning(deliveryValue.Exception, "SendTrainingProgramKafkaMessage - Error - {UniqueID}", trainingProgram.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SendTrainingProgramKafkaMessage - Error - {UniqueID}", trainingProgram.Id);
            }
        }


        public async Task SendTrainingProgramDeleteKafkaMessage(Guid id, int updatedBy, String subject)
        {
            try
            {
                await _kafkaMessengerService.SendKafkaMessage(id.ToString(), subject, new TrainingProgramDeleted { Id = id, UpdatedBy = updatedBy }, "ck-phoenix-trainingProgram");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SendTrainingProgramDeleteKafkaMessage - Error - {UniqueID}", id);
            }
        }
    }
}
