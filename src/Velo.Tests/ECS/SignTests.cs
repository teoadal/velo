using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS
{
    public class SignTests : EcsTestClass
    {
        public SignTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Iteration()
        {
            var components = BuildComponents();
            var sign = SignBuilder.Create(components);

            var unique = new HashSet<int>();
            foreach (var componentId in sign)
            {
                Assert.True(unique.Add(componentId));
            }
            
            Assert.Equal(components.Length, unique.Count);
        }
    }
}