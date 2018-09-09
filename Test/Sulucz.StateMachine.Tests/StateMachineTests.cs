// <copyright file="StateMachineTests.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sulucz.StateMachine.Builder;

    /// <summary>
    /// The state machine tests.
    /// </summary>
    [TestClass]
    public class StateMachineTests
    {
        /// <summary>
        /// A timeout for gates.
        /// </summary>
        private static TimeSpan gateTimeout = TimeSpan.FromSeconds(3);

        /// <summary>
        /// test a state machine with a single transition.
        /// </summary>
        [TestMethod]
        public void TestSimpleStateMachineTransition()
        {
            var waitEvent = new ManualResetEventSlim(false);

            var stateMachineBuilder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var startState = stateMachineBuilder.AddState(TestEnum.Start);
            var endState = stateMachineBuilder.AddState(TestEnum.End)
                                              .OnStateEnter(ctx =>
                                              {
                                                  waitEvent.Set();
                                                  return Task.CompletedTask;
                                              });

            startState.AddValidTransition(TestTransitionEnum.Stopping, endState);

            var stateMachine = stateMachineBuilder.Compile();
            var context = stateMachine.StartStateMachine(TestEnum.Start, 5);

            context.Post(TestTransitionEnum.Stopping);

            if (false == waitEvent.Wait(StateMachineTests.gateTimeout))
            {
                Assert.Fail();
            }

            waitEvent.Dispose();
        }

        /// <summary>
        /// Transition should fail if another is in progress.
        /// </summary>
        [TestMethod]
        public void TestStateMachineTransitionWhileInProgress()
        {
            var waitEvent = new ManualResetEventSlim(false);

            var stateMachineBuilder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var startState = stateMachineBuilder.AddState(TestEnum.Start);
            var endState = stateMachineBuilder.AddState(TestEnum.End);

            startState.AddValidTransition(TestTransitionEnum.Stopping, endState)
                .AddTransitionFunction(ctx =>
                {
                    waitEvent.Wait(StateMachineTests.gateTimeout);
                    return Task.CompletedTask;
                });

            var stateMachine = stateMachineBuilder.Compile();
            var context = stateMachine.StartStateMachine(TestEnum.Start, 5);

            context.Post(TestTransitionEnum.Stopping);
            Assert.ThrowsException<Exception>(() => context.Post(TestTransitionEnum.Stopping));

            waitEvent.Set();
            waitEvent.Dispose();
        }

        /// <summary>
        /// Test a transition which wont work.
        /// </summary>
        [TestMethod]
        public void TestInvalidTransition()
        {
            var stateMachineBuilder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var startState = stateMachineBuilder.AddState(TestEnum.Start);
            var endState = stateMachineBuilder.AddState(TestEnum.End);

            var stateMachine = stateMachineBuilder.Compile();
            var context = stateMachine.StartStateMachine(TestEnum.Start, 5);

            Assert.ThrowsException<InvalidOperationException>(() => context.Post(TestTransitionEnum.Middling));
        }

        /// <summary>
        /// Tests to make sure the context exists.
        /// </summary>
        [TestMethod]
        public void TestContextExists()
        {
            var stateMachineBuilder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var startState = stateMachineBuilder.AddState(TestEnum.Start);

            var stateMachine = stateMachineBuilder.Compile();
            var context = stateMachine.StartStateMachine(TestEnum.Start, 5);

            Assert.AreSame(context, stateMachine.Contexts.Single());
        }

        /// <summary>
        /// Test a double transition.
        /// </summary>
        [TestMethod]
        public void TestDoubleTransition()
        {
            var gate = new ManualResetEventSlim(false);
            var stateMachineBuilder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var startState = stateMachineBuilder.AddState(TestEnum.Start);
            var middleState = stateMachineBuilder.AddState(TestEnum.Middle);
            var endState = stateMachineBuilder.AddState(TestEnum.End);

            startState.AddValidTransition(TestTransitionEnum.Middling, middleState);
            middleState.AddValidTransition(TestTransitionEnum.Stopping, endState);

            middleState.OnStateEnter(ctx =>
                {
                    ctx.Post(TestTransitionEnum.Stopping);
                    return Task.CompletedTask;
                });

            endState.OnStateEnter(ctx =>
            {
                gate.Set();
                return Task.CompletedTask;
            });

            var stateMachine = stateMachineBuilder.Compile();

            var context = stateMachine.StartStateMachine(TestEnum.Start, 4);
            context.Post(TestTransitionEnum.Middling);

            if (false == gate.Wait(10000))
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Prevent double pending transistions.
        /// </summary>
        [TestMethod]
        public void BlockDoublePendingTransition()
        {
            var gate = new ManualResetEventSlim(false);

            var stateMachineBuilder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var startState = stateMachineBuilder.AddState(TestEnum.Start);
            var endState = stateMachineBuilder.AddState(TestEnum.End);

            startState.AddValidTransition(TestTransitionEnum.Stopping, endState);
            startState.OnStateEnter(ctx =>
            {
                Assert.IsTrue(gate.Wait(StateMachineTests.gateTimeout));
                return Task.CompletedTask;
            });

            var stateMachine = stateMachineBuilder.Compile();
            var context = stateMachine.StartStateMachine(TestEnum.Start, 5, executeStageEntry: true);

            context.Post(TestTransitionEnum.Stopping);
            Assert.ThrowsException<ArgumentException>(() => context.Post(TestTransitionEnum.Stopping));

            gate.Set();
        }

        /// <summary>
        /// An error in transition doesnt break the workflow.
        /// </summary>
        [TestMethod]
        public void TransitionErrorDoesntBreakWorkflow()
        {
            var gate = new ManualResetEventSlim(false);

            var stateMachineBuilder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();
            var startState = stateMachineBuilder.AddState(TestEnum.Start);
            var endState = stateMachineBuilder.AddState(TestEnum.End);

            startState.AddValidTransition(TestTransitionEnum.Stopping, endState)
                .AddTransitionFunction(ctx =>
                {
                    throw new Exception();
                });

            endState.OnStateEnter(ctx =>
            {
                gate.Set();
                return Task.CompletedTask;
            });

            var stateMachine = stateMachineBuilder.Compile();
            var context = stateMachine.StartStateMachine(TestEnum.Start, 5, executeStageEntry: true);

            context.Post(TestTransitionEnum.Stopping);

            Assert.IsTrue(gate.Wait(StateMachineTests.gateTimeout));
        }
    }
}
