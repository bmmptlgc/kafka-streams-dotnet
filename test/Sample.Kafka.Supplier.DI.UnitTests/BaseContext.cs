using Confluent.Kafka;
using Moq;
using sample_kafka_supplier_di;
using Sample.DI.UnitTests;
using Streamiz.Kafka.Net.Kafka;

namespace Sample.Kafka.Supplier.DI.UnitTests
{
    internal class BaseContext : ArrangeActAssert
    {
        protected ConsumeResult<byte[], byte[]> ConsumeResult = new();

        protected override void Arrange()
        {
            base.Arrange();
            
            var adminClientMock = new Mock<IAdminClient>();
            
            adminClientMock
                .Setup(ks => ks.GetMetadata(It.IsAny<TimeSpan>()))
                .Returns(new Metadata(
                    new List<BrokerMetadata> { new(1, "kafka-mock", 0) },
                    new List<TopicMetadata> {
                        new(
                            "orders",
                            new List<PartitionMetadata> {
                                new(0, 1, new [] {1}, new [] {1}, new Error(ErrorCode.NoError)) },
                            new Error(ErrorCode.NoError))
                    },
                    1, 
                    "kafka-mock"));
        
            var kafkaSupplierMock = new Mock<IKafkaSupplier>();
            
            kafkaSupplierMock
                .Setup(ks => ks.GetAdmin(It.IsAny<AdminClientConfig>()))
                .Returns(adminClientMock.Object);
        
            var consumerMock = new Mock<IConsumer<byte[], byte[]>>();
            
            consumerMock
                .SetupSequence(consumer => consumer.Consume(It.IsAny<TimeSpan>()))
                .Returns(ConsumeResult)
                .Returns((ConsumeResult<byte[], byte[]>)null);
            
            kafkaSupplierMock
                .Setup(ks => ks.GetConsumer(It.IsAny<ConsumerConfig>(), It.IsAny<IConsumerRebalanceListener>()))
                .Returns(consumerMock.Object);
            
            // var configuration = new StreamConfig<StringSerDes, SchemaAvroSerDes<GenericRecord>>
            // {
            //     ApplicationId = "kafka-topic-splitter",
            //     BootstrapServers = "kafka-mock",
            //     SchemaRegistryUrl = "registry-mock",
            //     FollowMetadata = true
            // };

            var factory = new CustomWebApplicationFactory<Program>(
                kafkaSupplierMock.Object);
            
            // var factory = new CustomWebApplicationFactory<Program>(
            //     new DefaultKafkaClientSupplierMock(configuration, consumerMock));

            factory.CreateClient();
        }

        protected override async Task Act()
        {
            await base.Act();
        }
    }
}