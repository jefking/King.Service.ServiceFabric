namespace King.Service.ServiceFabric.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Runtime;
    using NSubstitute;
    using NUnit.Framework;
    using System.Fabric;
    [TestFixture]
    public class TaskServiceTests
    {
        [Test]
        public void Constructor()
        {
            var context = Substitute.ForPartsOf<StatelessServiceContext>();
            var run = Substitute.For<IRunnable>();
            new TaskService(context, run);
        }

        [Test]
        public void IsStatelessService()
        {
            var context = Substitute.ForPartsOf<StatelessServiceContext>();
            var run = Substitute.For<IRunnable>();
            Assert.IsNotNull(new TaskService(context, run) as StatelessService);
        }

        [Test]
        public void ConstructorRunNull()
        {
            var context = Substitute.ForPartsOf<StatelessServiceContext>();
            Assert.That(() => new TaskService(context, null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task RunAsyncOnStartFalse()
        {
            var run = Substitute.For<IRunnable>();
            run.Start().Returns(false);

            var rs = new TaskServiceFake(run);

            await rs.RunTest(CancellationToken.None);

            run.Received().Start();
        }

        [Test]
        public async Task RunAsync()
        {
            var run = Substitute.For<IRunnable>();
            run.Start().Returns(true);

            var rs = new TaskServiceFake(run);

            var ct = new CancellationToken(true);

            await rs.RunTest(ct);

            run.Received().Start();
            run.Received().Stop();
        }

        [Test]
        public async Task RunAsyncWithLoop()
        {
            var run = Substitute.For<IRunnable>();
            run.Start().Returns(true);

            var rs = new TaskServiceFake(run);

            var ct = new CancellationTokenSource();

            using (var t = new Timer(new TimerCallback((object obj) => { ct.Cancel(); }), null, 3, Timeout.Infinite))
            {

                await rs.RunTest(ct.Token);

                run.Received().Start();
                run.Received().Stop();
            }
        }
    }
}