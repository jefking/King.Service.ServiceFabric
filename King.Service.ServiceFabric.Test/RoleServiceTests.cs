﻿namespace King.Service.ServiceFabric.Tests
{
    using Microsoft.ServiceFabric.Services.Runtime;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

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

        [Test]
        public async Task RunAsyncOnStartFalse()
        {
            var config = new object();
            var manager = Substitute.For<IRoleTaskManager<object>>();
            manager.OnStart(config).Returns(false);

            var rs = new RoleServiceFake<object>(manager, config);

            await rs.RunTest(CancellationToken.None);

            manager.Received().OnStart(config);
        }

        [Test]
        public async Task RunAsync()
        {
            var config = new object();
            var manager = Substitute.For<IRoleTaskManager<object>>();
            manager.OnStart(config).Returns(true);

            var rs = new RoleServiceFake<object>(manager, config);

            var ct = new CancellationToken(true);

            await rs.RunTest(ct);

            manager.Received().OnStart(config);
            manager.Received().Run();
            manager.Received().OnStop();
        }

        [Test]
        public async Task RunAsyncWithLoop()
        {
            var config = new object();
            var manager = Substitute.For<IRoleTaskManager<object>>();
            manager.OnStart(config).Returns(true);

            var rs = new RoleServiceFake<object>(manager, config);

            var ct = new CancellationTokenSource();

            using (var t = new Timer(new TimerCallback((object obj) => { ct.Cancel(); }), null, 3, Timeout.Infinite))
            {
                await rs.RunTest(ct.Token);

                manager.Received().OnStart(config);
                manager.Received().Run();
                manager.Received().OnStop();
            }
        }
    }
}