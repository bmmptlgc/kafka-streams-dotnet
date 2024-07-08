using sample_kafka_supplier_di.Options;
using Streamiz.Kafka.Net.Mock;

namespace Sample.Kafka.Supplier.DI.UnitTests.TopologyDriverTests;

[Category("Given one topic to split")]

internal class GivenOneTopicToSplit : BaseContext
{
    protected override void Arrange()
    {
        Topics.Add(new TopicConfig
        {
            SourceTopic = TestData.OrdersTopic,
            MessageTypes = new List<string> { TestData.OrderCreatedType, TestData.OrderCompletedType }
        });
        
        base.Arrange();
    }

    protected override void Act()
    {
        base.Act();
            
        Outputs = IntegrationTestUtils
            .WaitUntilMinKeyValueRecordsReceived(OutputTopics[TestData.OrderCreatedType], 1);
    }
}