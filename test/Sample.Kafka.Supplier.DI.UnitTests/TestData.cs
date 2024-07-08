namespace Sample.Kafka.Supplier.DI.UnitTests;

public static class TestData
{
    public const string OrdersTopic = "orders";
    public const string ProductsTopic = "products";
    
    public const string OrderCreatedType = "Test.OrderCreated";
    public const string OrderCompletedType = "Test.OrderCompleted";
    public const string OrderCancelledType = "Test.OrderCancelled";
}