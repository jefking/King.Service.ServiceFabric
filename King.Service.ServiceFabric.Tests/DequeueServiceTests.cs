namespace King.Service.ServiceFabric.Tests
{
    using System;
    using Azure.Data;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DequeueServiceTests
    {
        [Test]
        public void Constructor()
        {
            var processor = Substitute.For<IProcessor<object>>();
            new DequeueService<object>(Guid.NewGuid().ToString(), processor);
        }

        [Test]
        public void ConstructorQueueNameNull()
        {
            var processor = Substitute.For<IProcessor<object>>();
            Assert.That(() => new DequeueService<object>(null, processor), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ConstructorProcessorNull()
        {
            Assert.That(() => new DequeueService<object>(Guid.NewGuid().ToString(), null), Throws.TypeOf<ArgumentNullException>());
        }
    }
}