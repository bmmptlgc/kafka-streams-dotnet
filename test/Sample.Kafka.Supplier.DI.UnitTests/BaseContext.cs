using System.Reflection;
using Avro.Generic;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using sample_kafka_supplier_di;
using Sample.Kafka.Supplier.DI.UnitTests.TopologyDriverTests;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Kafka;
using Streamiz.Kafka.Net.SchemaRegistry.SerDes.Avro;
using Streamiz.Kafka.Net.SchemaRegistry.SerDes.Mock;
using Streamiz.Kafka.Net.SerDes;

namespace Sample.Kafka.Supplier.DI.UnitTests
{
    internal class BaseContext : ArrangeActAssert
    {
        protected ConsumeResult<byte[], byte[]> ConsumeResult = new();
        protected int ExpectedNumberProducedMessages = 1;
        protected Mock<IProducer<byte[], byte[]>> ProducerMock = new();

        private IHostedService _hostedService { get; set; }
        private int _producedMessagesCounter = 0;

        protected override void Arrange()
        {
            base.Arrange();

            var streamConfig = new StreamConfig<StringSerDes, SchemaAvroSerDes<GenericRecord>>
            {
                ApplicationId = "kafka-topic-splitter",
                BootstrapServers = "mock://test",
                SchemaRegistryUrl = "mock://test",
                FollowMetadata = true
            };
            
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
        
            adminClientMock
                .Setup(ks => ks.GetMetadata(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Returns(new Metadata(
                    new List<BrokerMetadata> { new(1, "kafka-mock", 0) },
                    new List<TopicMetadata> {
                        new(
                            "mt_tests-ordercreated",
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
                .Callback<TimeSpan>(_ =>
                {
                    var serDes = (SchemaAvroSerDes<GenericRecord>)streamConfig.DefaultValueSerDes;
                    
                    // get the field info
                    var registryClient = (MockSchemaRegistryClient)serDes
                        .GetType()
                        .GetField("registryClient", BindingFlags.Instance | BindingFlags.NonPublic)?
                        .GetValue(serDes)!;

                    registryClient.RegisterSchemaAsync("orders-value", OrderCreated._SCHEMA.ToString());
                    
                    listener.PartitionsAssigned(
                        consumerMock.Object, new List<TopicPartition>
                        {
                            new("orders", new Partition(0))
                        });
                })
                .Returns(resultQueue.Dequeue)
                .Callback<TimeSpan>(_ => resultQueue.Enqueue(null));
            
            kafkaSupplierMock
                .Setup(ks => ks.GetConsumer(It.IsAny<ConsumerConfig>(), It.IsAny<IConsumerRebalanceListener>()))
                .Callback<ConsumerConfig, IConsumerRebalanceListener>((_, r) => listener = r)
                .Returns(consumerMock.Object);

            ProducerMock.SetupGet(producer => producer.Name).Returns("kafka-topic-splitter-producer");
            
            kafkaSupplierMock
                .Setup(ks => ks.GetProducer(It.IsAny<ProducerConfig>()))
                .Returns(ProducerMock.Object);

            kafkaSupplierMock
                .Setup(ks => ks.GetRestoreConsumer(It.IsAny<ConsumerConfig>()))
                .Returns(Mock.Of<IConsumer<byte[],byte[]>>());
            
            var factory = new CustomWebApplicationFactory<Program>(
                kafkaSupplierMock.Object, streamConfig);

            factory.CreateClient();

            _hostedService = factory.Services.GetRequiredService<IEnumerable<IHostedService>>()
                .FirstOrDefault(service => service.GetType() == typeof(TopicSplitterService))!;
        }
        
        protected override async Task Act()
        {
            await base.Act();

            await WaitOnProducedMessages();
        }

        protected override void CleanUpTest()
        {
            base.CleanUpTest();
            
            _hostedService.StopAsync(CancellationToken.None);

            ProducerMock.Reset();
            _producedMessagesCounter = 0;
        }

        private async Task WaitOnProducedMessages()
        {
            await Task.Run(() =>
            {
                var startTime = DateTime.Now;

                do
                {
                    if (startTime + TimeSpan.FromSeconds(2) < DateTime.Now)
                        break;
                } while (_producedMessagesCounter < ExpectedNumberProducedMessages);
            });
        }
    }
}