﻿namespace King.Service.ServiceFabric.Tests
{
    using King.Azure.Data;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Runtime;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture]
    public class DequeueServiceTests
    {
        [Test]
        public void Constructor()
        {
            var processor = Substitute.For<IProcessor<object>>();
            var state = Substitute.For<IReliableStateManager>();
            new DequeueService<object>(state, Guid.NewGuid().ToString(), processor);
        }

        [Test]
        public void ConstructorQueueNameNull()
        {
            var processor = Substitute.For<IProcessor<object>>();
            var state = Substitute.For<IReliableStateManager>();
            Assert.That(() => new DequeueService<object>(state, null, processor), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorProcessorNull()
        {
            var state = Substitute.For<IReliableStateManager>();
            Assert.That(() => new DequeueService<object>(state, Guid.NewGuid().ToString(), null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task RunAsyncCancelled()
        {
            var context = Substitute.ForPartsOf<StatefulServiceContext>();
            var queueName = Guid.NewGuid().ToString();
            var processor = Substitute.For<IProcessor<object>>();
            var state = Substitute.For<IReliableStateManager>();
            var ds = new FakeDequeueService(state, queueName, processor);
            var token = new CancellationToken(true);
            await ds.RunTest(token);

            await processor.Received(0).Process(Arg.Any<object>());
            //await context.Received(0).GetOrAddAsync<IReliableQueue<object>>(queueName);
            Assert.Inconclusive("Fix Code");
        }

        [Test]
        public async Task RunAsyncWithMessage()
        {
            var state = Substitute.For<IReliableStateManager>();
            var queueName = Guid.NewGuid().ToString();
            var queue = Substitute.For<IReliableQueue<object>>();
            var msg = new ConditionalValue<object>(false, null);

            var tx = Substitute.For<ITransaction>();
            //state.GetOrAddAsync<IReliableQueue<object>>(queueName).Returns(queue);
            //state.CreateTransaction().Returns(tx);
            queue.TryDequeueAsync(tx).Returns(msg);
            var processor = Substitute.For<IProcessor<object>>();

            var ds = new FakeDequeueService(state, queueName, processor);

            var ct = new CancellationTokenSource();

            using (var t = new Timer(new TimerCallback((object obj) => { ct.Cancel(); }), null, 3, Timeout.Infinite))
            {
                await ds.RunTest(ct.Token);
            }

            //await state.Received().GetOrAddAsync<IReliableQueue<object>>(queueName);
            //state.Received().CreateTransaction();
            await tx.Received(0).CommitAsync();
            await queue.Received().TryDequeueAsync(tx);
            await processor.Received(0).Process(Arg.Any<object>());
            await tx.Received(0).CommitAsync();
            Assert.Inconclusive();
        }

        [Test]
        public async Task RunAsyncProcessNoSuccess()
        {
            var state = Substitute.For<IReliableStateManager>();
            var queueName = Guid.NewGuid().ToString();
            var queue = Substitute.For<IReliableQueue<object>>();
            var data = new object();
            var msg = new ConditionalValue<object>(true, data);

            var tx = Substitute.For<ITransaction>();
            //state.GetOrAddAsync<IReliableQueue<object>>(queueName).Returns(queue);
            //state.CreateTransaction().Returns(tx);
            queue.TryDequeueAsync(tx).Returns(msg);
            var processor = Substitute.For<IProcessor<object>>();
            processor.Process(data).Returns(false);

            var ds = new FakeDequeueService(state, queueName, processor);

            var ct = new CancellationTokenSource();

            using (var t = new Timer(new TimerCallback((object obj) => { ct.Cancel(); }), null, 3, Timeout.Infinite))
            {
                await ds.RunTest(ct.Token);
            }

            //await state.Received().GetOrAddAsync<IReliableQueue<object>>(queueName);
            //state.Received().CreateTransaction();
            await queue.Received().TryDequeueAsync(tx);
            await processor.Received().Process(data);
            await tx.Received(0).CommitAsync();

            Assert.Inconclusive();
        }

        [Test]
        public async Task RunAsync()
        {
            var state = Substitute.For<IReliableStateManager>();
            var queueName = Guid.NewGuid().ToString();
            var queue = Substitute.For<IReliableQueue<object>>();
            var data = new object();
            var msg = new ConditionalValue<object>(true, data);

            var tx = Substitute.For<ITransaction>();
            //state.GetOrAddAsync<IReliableQueue<object>>(queueName).Returns(queue);
            //state.CreateTransaction().Returns(tx);
            queue.TryDequeueAsync(tx).Returns(msg);
            var processor = Substitute.For<IProcessor<object>>();
            processor.Process(data).Returns(true);

            var ds = new FakeDequeueService(state, queueName, processor);

            var ct = new CancellationTokenSource();

            using (var t = new Timer(new TimerCallback((object obj) => { ct.Cancel(); }), null, 3, Timeout.Infinite))
            {
                await ds.RunTest(ct.Token);
            }

            //await state.Received().GetOrAddAsync<IReliableQueue<object>>(queueName);
            //state.Received().CreateTransaction();
            await queue.Received().TryDequeueAsync(tx);
            await processor.Received().Process(data);
            await tx.Received().CommitAsync();

            Assert.Inconclusive();
        }
    }
}