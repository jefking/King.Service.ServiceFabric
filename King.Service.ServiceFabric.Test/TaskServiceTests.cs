namespace King.Service.ServiceFabric.Tests
{
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture]
    public class TaskServiceTests
    {
        [Test]
        public void Constructor()
        {
            var run = Substitute.For<IRunnable>();
            new TaskService(run);
        }

        [Test]
        public void ConstructorRunNull()
        {
            Assert.That(() => new TaskService(null), Throws.TypeOf<ArgumentNullException>());
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