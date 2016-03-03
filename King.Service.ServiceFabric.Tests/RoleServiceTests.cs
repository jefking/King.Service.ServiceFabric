namespace King.Service.ServiceFabric.Tests
{
    using System;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class RoleServiceTests
    {
        [Test]
        public void Constructor()
        {
            var manager = Substitute.For<IRoleTaskManager<object>>();
            new RoleService<object>(manager);
        }

        [Test]
        public void ConstructorWithConfig()
        {
            var manager = Substitute.For<IRoleTaskManager<object>>();
            new RoleService<object>(manager, new object());
        }

        [Test]
        public void ConstructorhNullConfig()
        {
            var manager = Substitute.For<IRoleTaskManager<object>>();
            new RoleService<object>(manager, null);
        }

        [Test]
        public void ConstructorManagerNull()
        {
            Assert.That(() => new RoleService<object>(null), Throws.TypeOf<ArgumentNullException>());
        }
    }
}