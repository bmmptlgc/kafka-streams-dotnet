using System.Text;
using Avro;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Moq;
using Sample.Kafka.Supplier.DI.UnitTests.TopologyDriverTests;

namespace Sample.Kafka.Supplier.DI.UnitTests.KafkaTopicSplitter
{
    [TestFixture(TestName = "When consuming a message with a type configured to publish to a single schema topic")]
    internal class WhenConsumingAMessageWithATypeConfiguredToPublishToASingleSchemaTopic : GivenATopicSplitterConfigurationWithOneTopicToSplit
    {
        protected override void Arrange()
        {
            var order = new GenericRecord((RecordSchema) OrderCreated._SCHEMA);
            
            var entityId = Guid.NewGuid();

            order.Add("Id", Guid.NewGuid());
            order.Add("Source", "Order");
            order.Add("SourceId", entityId.ToString());
            order.Add("CreatedAt", DateTime.UtcNow);
            order.Add("Version", 0L);
            order.Add("OrderId", entityId);
            order.Add("ProductId", Guid.NewGuid());
            order.Add("Quantity", 3);
            order.Add("PromotionId", Guid.NewGuid());
            order.Add("ResellerId", Guid.NewGuid());
            order.Add("ProgramId", Guid.NewGuid());
            order.Add("PatientProfileId", Guid.NewGuid().ToString());
            order.Add("CreatedBy", null);

            var schemaRegistryClientMock = new Mock<ISchemaRegistryClient>();

            schemaRegistryClientMock
                .Setup(registry => registry
                    .RegisterSchemaAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(1);
            
            ConsumeResult = new ConsumeResult<byte[], byte[]>
            {
                Topic = "orders",
                Message = new Message<byte[], byte[]>
                {
                    Headers = new Headers
                    {
                        { "MessageType", Encoding.ASCII.GetBytes("Tests.OrderCreated") }
                    },
                    Key = Encoding.ASCII.GetBytes("83401039-76dc-4432-8503-a38a8998c87f"),
                    Value = new AvroSerializer<GenericRecord>(schemaRegistryClientMock.Object).SerializeAsync(order, new SerializationContext()).Result
                }
            };
            
            base.Arrange();
        }

        [Test(Name = "Then it should publish to the expected topic name")]
        public void ThenTtShouldReturnTheExpectedContext()
        { 
            ProducerMock.Verify(producer => producer.Produce(
                It.IsAny<TopicPartition>(),
                It.IsAny<Message<byte[],byte[]>>(),
                It.IsAny<Action<DeliveryReport<byte[],byte[]>>>()), Times.Exactly(1));
        }
    }
}
