// <copyright file="StateMachineFaultTests.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sulucz.StateMachine.Builder;

    [TestClass]
    public class StateMachineFaultTests
    {
        /// <summary>
        /// Test that a simple fault stops the machine.
        /// </summary>
        /// <returns>An async task.</returns>
        [TestMethod]
        public async Task TestSimpleFaultStopsMachine()
        {
            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var one = builder.AddState(TestEnum.Start)
                .OnStateEnter(xtc => throw new Exception());
            var two = builder.AddState(TestEnum.End);
            one.AddValidTransition(TestTransitionEnum.Stopping, two);

            var stateMachine = builder.Compile();
            var context = stateMachine.StartStateMachine(TestEnum.Start, 5, true);

            await Task.Delay(1000);
            Assert.AreEqual(StateMachineLifetime.Error, context.CurrentLifecycle);
        }

        /// <summary>
        /// Test that the local handler can save the machine.
        /// </summary>
        /// <returns>An async task.</returns>
        [TestMethod]
        public async Task TestSimplEFaultCaughtLocallyDoesntStopMachine()
        {
            var hitHandler = false;

            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var one = builder.AddState(TestEnum.Start)
                .OnStateEnter(xtc => throw new Exception())
                .SetOnEnterFaultHandler((ctx, ex) => { hitHandler = true; });
            var two = builder.AddState(TestEnum.End);
            one.AddValidTransition(TestTransitionEnum.Stopping, two);

            var stateMachine = builder.Compile();
            var context = stateMachine.StartStateMachine(TestEnum.Start, 5, true);

            await Task.Delay(1000);
            Assert.AreEqual(StateMachineLifetime.Running, context.CurrentLifecycle);
            Assert.IsTrue(hitHandler);
        }

        /// <summary>
        /// Test the globla exception handler.
        /// </summary>
        /// <returns>An async task.</returns>
        [TestMethod]
        public async Task TestSimplEFaultCaughtGloballyDoesntStopMachine()
        {
            var hitHandler = false;

            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var one = builder.AddState(TestEnum.Start)
                .OnStateEnter(xtc => throw new Exception());
            var two = builder.AddState(TestEnum.End);
            builder.SetGlobalFaultHandler((ctx, ex) => { hitHandler = true; });
            one.AddValidTransition(TestTransitionEnum.Stopping, two);

            var stateMachine = builder.Compile();
            var context = stateMachine.StartStateMachine(TestEnum.Start, 5, true);

            await Task.Delay(1000);
            Assert.AreEqual(StateMachineLifetime.Running, context.CurrentLifecycle);
            Assert.IsTrue(hitHandler);
        }

        /// <summary>
        /// Test that the local handler overrides the global handler.
        /// </summary>
        /// <returns>An async task.</returns>
        [TestMethod]
        public async Task TestRethrownLocalExceptionDoesntHitGlobalHandler()
        {
            var hitGlobalHander = false;
            var hitLocalHandler = false;

            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var one = builder.AddState(TestEnum.Start)
                .OnStateEnter(xtc => throw new Exception())
                .SetOnEnterFaultHandler((ctx, ex) =>
                {
                    hitLocalHandler = true;
                    throw ex;
                });
            var two = builder.AddState(TestEnum.End);
            builder.SetGlobalFaultHandler((ctx, ex) => { hitGlobalHander = true; });

            one.AddValidTransition(TestTransitionEnum.Stopping, two);

            var stateMachine = builder.Compile();
            var context = stateMachine.StartStateMachine(TestEnum.Start, 5, true);

            await Task.Delay(1000);
            Assert.AreEqual(StateMachineLifetime.Error, context.CurrentLifecycle);
            Assert.IsTrue(hitLocalHandler);
            Assert.IsFalse(hitGlobalHander);
        }

        /// <summary>
        /// Test that the global handler can fault the state machine.
        /// </summary>
        /// <returns>An async task.</returns>
        [TestMethod]
        public async Task TestRethrownGlobalExceptionFaultsContext()
        {
            var hitGlobalHander = false;

            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var one = builder.AddState(TestEnum.Start)
                .OnStateEnter(xtc => throw new Exception());
            var two = builder.AddState(TestEnum.End);
            builder.SetGlobalFaultHandler((ctx, ex) =>
            {
                hitGlobalHander = true;
                throw ex;
            });

            one.AddValidTransition(TestTransitionEnum.Stopping, two);

            var stateMachine = builder.Compile();
            var context = stateMachine.StartStateMachine(TestEnum.Start, 5, true);

            await Task.Delay(1000);
            Assert.AreEqual(StateMachineLifetime.Error, context.CurrentLifecycle);
            Assert.IsTrue(hitGlobalHander);
        }
    }
}
