// <copyright file="StateMachineConcurrencyTests.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.FunctionalTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sulucz.StateMachine.Builder;

    /// <summary>
    /// The state machine concurrency tests.
    /// </summary>
    [TestClass]
    public class StateMachineConcurrencyTests
    {
        /// <summary>
        /// The time to wait at a gate.
        /// </summary>
        private static readonly TimeSpan GateWait = TimeSpan.FromSeconds(3);

        /// <summary>
        /// Test posting a transition while one is happening.
        /// </summary>
        [TestMethod]
        public void TestPendingTransition()
        {
            var factory = StateMachineFactory.CreateBuilder<NumberedTestEnum, TransitionEnum, int>();

            var leftState2 = false;

            var finishGate = new ManualResetEventSlim(false);

            var first = factory.AddState(NumberedTestEnum.One);
            var second = factory.AddState(NumberedTestEnum.Two);
            second.OnStateEnter(async ctx =>
            {
                ctx.Post(TransitionEnum.T2);
                await Task.Delay(1000);
                leftState2 = true;
            });
            var third = factory.AddState(NumberedTestEnum.Three);
            third.OnStateEnter(ctx =>
            {
                Assert.IsTrue(leftState2);
                finishGate.Set();
                return Task.CompletedTask;
            });

            first.AddValidTransition(TransitionEnum.T1, second);
            second.AddValidTransition(TransitionEnum.T2, third);

            var stateMachine = factory.Compile();
            var session = stateMachine.StartStateMachine(NumberedTestEnum.One, 5);
            session.Post(TransitionEnum.T1);

            Assert.IsTrue(finishGate.Wait(StateMachineConcurrencyTests.GateWait));
        }
    }
}
