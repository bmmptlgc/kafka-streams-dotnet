using System.Text;
using Avro;
using Avro.Generic;
using FluentAssertions;

namespace Sample.Kafka.Supplier.DI.UnitTests.TopologyDriverTests
{
    [TestFixture(TestName = "When consuming a message with a type configured to publish to a single schema topic")]
    internal class WhenConsumingAMessageWithATypeConfiguredToPublishToASingleSchemaTopic : GivenOneTopicToSplit
    {
        protected override void Arrange()
        {
            Headers.Add("MessageType", Encoding.ASCII.GetBytes(TestData.OrderCreatedType));

            base.Arrange();
        }

        protected override void Act()
        {
            var inputTopic = InputTopics[TestData.OrdersTopic];

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
            
            inputTopic.PipeInput("test-1", order, Headers);

            base.Act();
        }

        [Test(Name = "Then it should publish to the expected topic name")]
        public void ThenTtShouldReturnTheExpectedContext()
        {
            Outputs[0].Message.Key.Should().Be("test-1");
        }
    }
}
