namespace Sample.Kafka.Supplier.DI.UnitTests
{
    internal abstract class ArrangeActAssert
    {
        [SetUp]
        public async Task SetUp()
        {
            Arrange();
            await Act();
        }

        [TearDown]
        public void TearDown() => CleanUpTest();

        [OneTimeTearDown]
        public void TestFixtureTearDown() => CleanUpTestFixture();

        protected virtual void Arrange() { }
        
        protected virtual Task Act() => Task.CompletedTask;
        
        protected virtual void CleanUpTest() { }
        
        protected virtual void CleanUpTestFixture() { }
    }
}