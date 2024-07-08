using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Moq;
using sample_kafka_supplier_di;
using sample_kafka_supplier_di.Options;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Mock;
using Streamiz.Kafka.Net.SchemaRegistry.SerDes.Avro;
using Streamiz.Kafka.Net.SerDes;

namespace Sample.Kafka.Supplier.DI.UnitTests.TopologyDriverTests
{
    internal class BaseContext : ArrangeActAssert
    {
        private TopologyTestDriver _topologyTestDriver;

        protected readonly List<TopicConfig> Topics = new();
        protected readonly Dictionary<string, TestInputTopic<string, GenericRecord>> InputTopics = new();
        protected readonly Dictionary<string, TestOutputTopic<string, GenericRecord>> OutputTopics = new();
        protected readonly Headers Headers = new();
        protected ConsumeResult<string, GenericRecord> Output = new();
        protected List<ConsumeResult<string, GenericRecord>> Outputs = new();
            
        protected override void Arrange()
        {
            base.Arrange();

            var topology = TopicSplitterService.BuildTopology(Topics);
            
            var config = new StreamConfig<StringSerDes, SchemaAvroSerDes<GenericRecord>>
            {
                ApplicationId = "test-topic-splitter",
                SchemaRegistryUrl = "mock://test",
                FollowMetadata = true,
                MaxPollIntervalMs = 100
            };

            _topologyTestDriver = new TopologyTestDriver(topology, config, TopologyTestDriver.Mode.ASYNC_CLUSTER_IN_MEMORY);

            foreach (var topicConfig in Topics)
            {
                var inputTopic = _topologyTestDriver.CreateInputTopic<string, GenericRecord>(
                    topicConfig.SourceTopic);
                
                InputTopics.Add(topicConfig.SourceTopic, inputTopic);
                    
                foreach (var messageType in topicConfig.MessageTypes)
                {
                    var outputTopic = _topologyTestDriver.CreateOuputTopic<string, GenericRecord>(
                        $"single-{messageType}");
            
                    OutputTopics.Add(messageType, outputTopic);
                }
            }
        }
        
        protected override void CleanUpTest()
        {
            base.CleanUpTest();

            _topologyTestDriver.Dispose();
        }
    }
}