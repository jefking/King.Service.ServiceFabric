namespace King.Service.ServiceFabric.Tests
{
    using NUnit.Framework;
    using System;
    using System.Fabric;

    [TestFixture]
    public class RunTimeTests
    {
        [Test]
        public void Constructor()
        {
            new RunTime<object>(Guid.NewGuid().ToString());
        }

        [Test]
        public void ConstructorQueueNameNull()
        {
            Assert.That(() => new RunTime<object>(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Run()
        {
            var rt = new RunTime<object>(Guid.NewGuid().ToString());
            //Can't Test deeper?
            Assert.That(() => rt.Run(), Throws.TypeOf<FabricConnectionDeniedException>());
        }
    }
}