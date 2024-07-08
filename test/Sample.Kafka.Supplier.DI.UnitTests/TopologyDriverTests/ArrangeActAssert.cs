namespace Sample.Kafka.Supplier.DI.UnitTests.TopologyDriverTests
{
    internal abstract class ArrangeActAssert
    {
        [SetUp]
        public void SetUp()
        {
            Arrange();
            Act();
        }

        [TearDown]
        public void TearDown() => CleanUpTest();

        [OneTimeTearDown]
        public void TestFixtureTearDown() => CleanUpTestFixture();

        protected virtual void Arrange() { }
        
        protected virtual void Act() { }
        
        protected virtual void CleanUpTest() { }
        
        protected virtual void CleanUpTestFixture() { }
    }
}