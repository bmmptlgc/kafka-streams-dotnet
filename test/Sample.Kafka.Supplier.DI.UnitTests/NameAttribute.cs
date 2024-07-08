using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    public class TestAttribute : NUnitAttribute, ISimpleTestBuilder
    {
        private readonly NUnitTestCaseBuilder _builder = new();

        public string Name { get; set; }

        public TestMethod BuildFrom(IMethodInfo method, Test suite)
        {
            var testMethod = _builder.BuildTestMethod(method, suite, null);

            testMethod.Name = Name;

            return testMethod;
        }
    }
}