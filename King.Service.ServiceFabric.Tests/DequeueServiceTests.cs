namespace King.Service.ServiceFabric.Tests
{
    using Azure.Data;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Runtime;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

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
        public void IsStatelessService()
        {
            var processor = Substitute.For<IProcessor<object>>();
            Assert.IsNotNull(new DequeueService<object>(Guid.NewGuid().ToString(), processor) as StatefulService);
        }

        [Test]
        public void ConstructorQueueNameNull()
        {
            var processor = Substitute.For<IProcessor<object>>();
            Assert.That(() => new DequeueService<object>(null, processor), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorProcessorNull()
        {
            Assert.That(() => new DequeueService<object>(Guid.NewGuid().ToString(), null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task RunAsyncCancelled()
        {
            var queueName = Guid.NewGuid().ToString();
            var state = Substitute.For<IReliableStateManager>();
            var processor = Substitute.For<IProcessor<object>>();
            var ds = new FakeDequeueService(queueName, processor, state);
            var token = new CancellationToken(true);
            await ds.RunTest(token);

            await processor.Received(0).Process(Arg.Any<object>());
            await state.Received(0).GetOrAddAsync<IReliableQueue<object>>(queueName);
        }

        [Test]
        public async Task RunAsyncWithMessage()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = Substitute.For<IReliableQueue<object>>();
            var msg = new ConditionalResult<object>(false, null);

            var tx = Substitute.For<ITransaction>();
            var state = Substitute.For<IReliableStateManager>();
            state.GetOrAddAsync<IReliableQueue<object>>(queueName).Returns(queue);
            state.CreateTransaction().Returns(tx);
            queue.TryDequeueAsync(tx).Returns(msg);
            var processor = Substitute.For<IProcessor<object>>();

            var ds = new FakeDequeueService(queueName, processor, state);

            var ct = new CancellationTokenSource();

            using (var t = new Timer(new TimerCallback((object obj) => { ct.Cancel(); }), null, 3, Timeout.Infinite))
            {
                await ds.RunTest(ct.Token);
            }

            await state.Received().GetOrAddAsync<IReliableQueue<object>>(queueName);
            state.Received().CreateTransaction();
            await tx.Received(0).CommitAsync();
            await queue.Received().TryDequeueAsync(tx);
            await processor.Received(0).Process(Arg.Any<object>());
            await tx.Received(0).CommitAsync();
        }

        [Test]
        public async Task RunAsyncProcessNoSuccess()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = Substitute.For<IReliableQueue<object>>();
            var data = new object();
            var msg = new ConditionalResult<object>(true, data);

            var tx = Substitute.For<ITransaction>();
            var state = Substitute.For<IReliableStateManager>();
            state.GetOrAddAsync<IReliableQueue<object>>(queueName).Returns(queue);
            state.CreateTransaction().Returns(tx);
            queue.TryDequeueAsync(tx).Returns(msg);
            var processor = Substitute.For<IProcessor<object>>();
            processor.Process(data).Returns(false);

            var ds = new FakeDequeueService(queueName, processor, state);

            var ct = new CancellationTokenSource();

            using (var t = new Timer(new TimerCallback((object obj) => { ct.Cancel(); }), null, 3, Timeout.Infinite))
            {
                await ds.RunTest(ct.Token);
            }

            await state.Received().GetOrAddAsync<IReliableQueue<object>>(queueName);
            state.Received().CreateTransaction();
            await queue.Received().TryDequeueAsync(tx);
            await processor.Received().Process(data);
            await tx.Received(0).CommitAsync();
        }

        [Test]
        public async Task RunAsync()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = Substitute.For<IReliableQueue<object>>();
            var data = new object();
            var msg = new ConditionalResult<object>(true, data);

            var tx = Substitute.For<ITransaction>();
            var state = Substitute.For<IReliableStateManager>();
            state.GetOrAddAsync<IReliableQueue<object>>(queueName).Returns(queue);
            state.CreateTransaction().Returns(tx);
            queue.TryDequeueAsync(tx).Returns(msg);
            var processor = Substitute.For<IProcessor<object>>();
            processor.Process(data).Returns(true);

            var ds = new FakeDequeueService(queueName, processor, state);

            var ct = new CancellationTokenSource();

            using (var t = new Timer(new TimerCallback((object obj) => { ct.Cancel(); }), null, 3, Timeout.Infinite))
            {
                await ds.RunTest(ct.Token);
            }

            await state.Received().GetOrAddAsync<IReliableQueue<object>>(queueName);
            state.Received().CreateTransaction();
            await queue.Received().TryDequeueAsync(tx);
            await processor.Received().Process(data);
            await tx.Received().CommitAsync();
        }
    }
}