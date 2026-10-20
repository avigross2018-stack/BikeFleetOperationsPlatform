using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace IngestionService.Services
{
    public class ProducerService
    {
        private readonly IProducer<Null, string> _producer;
        public ProducerService(IProducer<Null, string> producer)
        {
            _producer = producer;
        }

        public async Task<DeliveryResult<Null, string>> SendAsync<T>(T model, 
        string topicName, CancellationToken cancellationToken = default)
        {
            var msg = new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(model)
            };

            return await _producer.ProduceAsync(topicName, msg, cancellationToken);
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
    }
}