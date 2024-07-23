using System.Text;
using Avro.Generic;
using Microsoft.Extensions.Options;
using sample_kafka_supplier_di.Options;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Kafka;
using Streamiz.Kafka.Net.SchemaRegistry.SerDes.Avro;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;

namespace sample_kafka_supplier_di
{
    public class TopicSplitterService : BackgroundService
    {
        private readonly ILogger<TopicSplitterService> _logger;

        private readonly StreamConfig<StringSerDes, SchemaAvroSerDes<GenericRecord>> _streamConfig;
        private readonly TopicSplitterOptions _topicSplitterOptions;
        private readonly IKafkaSupplier _kafkaSupplier;

        public TopicSplitterService(
            StreamConfig<StringSerDes, SchemaAvroSerDes<GenericRecord>> streamConfig,
            IOptions<TopicSplitterOptions> topicSplitterOptions,
            IKafkaSupplier kafkaSupplier,
            ILogger<TopicSplitterService> logger)
        {
            _streamConfig = streamConfig ?? throw new ArgumentNullException(nameof(streamConfig));
            _topicSplitterOptions = topicSplitterOptions.Value;
            _kafkaSupplier = kafkaSupplier ?? throw new ArgumentNullException(nameof(kafkaSupplier));
            _logger = logger;
        }
    
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var topology = BuildTopology(_topicSplitterOptions.Topics);

                var stream = new KafkaStream(topology, _streamConfig, _kafkaSupplier);
            
                await stream.StartAsync(stoppingToken);
            
                _logger.LogInformation("Kafka Topic Splitter started");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Kafka Topic Splitter error");
                throw;
            }
        }

        public static Topology BuildTopology(List<TopicConfig> topics)
        {
            var builder = new StreamBuilder();

            foreach (var topicConfig in topics)
            {
                // _logger.LogInformation("Building topology for topic {Topic}", topicConfig.SourceTopic);

                var sourceStream = builder.Stream<string, GenericRecord>(topicConfig.SourceTopic);

                sourceStream
                    .Filter((_, _) =>
                    {
                        var messageTypeHeader = StreamizMetadata.GetCurrentHeadersMetadata()
                            .FirstOrDefault(h => h.Key == "MessageType")?.GetValueBytes();
                        // throw new Exception("Filter");
                        return messageTypeHeader != null &&
                               topicConfig.MessageTypes.Contains(Encoding.UTF8.GetString(messageTypeHeader));
                    })
                    .To((_, _, context) =>
                    {
                        var messageType = context.Headers
                            .FirstOrDefault(h => h.Key == "MessageType")!.GetValueBytes();
                        // throw new Exception("To");
                        return $"mt_{Encoding.UTF8.GetString(messageType).Replace(".", "-").ToLower()}";
                    }, new StringSerDes(), new SchemaAvroSerDes<GenericRecord>());
            }

            return builder.Build();
        }
    }
}