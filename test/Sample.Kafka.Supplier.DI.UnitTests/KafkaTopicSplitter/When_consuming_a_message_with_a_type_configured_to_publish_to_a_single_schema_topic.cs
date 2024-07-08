using System.Text;
using Confluent.Kafka;

namespace Sample.Kafka.Supplier.DI.UnitTests.KafkaTopicSplitter
{
    [TestFixture(TestName = "When consuming a message with a type configured to publish to a single schema topic")]
    internal class WhenConsumingAMessageWithATypeConfiguredToPublishToASingleSchemaTopic : GivenATopicSplitterConfigurationWithOneTopicToSplit
    {
        protected override void Arrange()
        {
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
                    Value = Encoding.ASCII.GetBytes("order1")
                }
            };
            base.Arrange();
        }

        [Test(Name = "Then it should publish to the expected topic name")]
        // [Test]
        public void ThenTtShouldReturnTheExpectedContext()
        { 

        }
    }
}
