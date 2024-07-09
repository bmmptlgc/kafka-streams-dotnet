using Confluent.Kafka;
using Moq;
using sample_kafka_supplier_di;
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

            IConsumerRebalanceListener listener = null;

            var resultQueue = new Queue<ConsumeResult<byte[], byte[]>?>(new[] { ConsumeResult }!);

            consumerMock
                .Setup(consumer => consumer.Consume(It.IsAny<TimeSpan>()))
                .Callback<TimeSpan>(_ => listener.PartitionsAssigned(
                    consumerMock.Object, new List<TopicPartition>
                    {
                        new("orders", new Partition(0))
                    }))
                .Returns(resultQueue.Dequeue)
                .Callback<TimeSpan>(_ => resultQueue.Enqueue(null));
            
            kafkaSupplierMock
                .Setup(ks => ks.GetConsumer(It.IsAny<ConsumerConfig>(), It.IsAny<IConsumerRebalanceListener>()))
                .Callback<ConsumerConfig, IConsumerRebalanceListener>((_, r) => listener = r)
                .Returns(consumerMock.Object);

            var producerMock = new Mock<IProducer<byte[], byte[]>>();
            producerMock.SetupGet(producer => producer.Name).Returns("kafka-topic-splitter-producer");
            
            kafkaSupplierMock
                .Setup(ks => ks.GetProducer(It.IsAny<ProducerConfig>()))
                .Returns(producerMock.Object);

            var factory = new CustomWebApplicationFactory<Program>(
                kafkaSupplierMock.Object);
            
            factory.CreateClient();
        }

        protected override async Task Act()
        {
            await base.Act();
        }
    }
}