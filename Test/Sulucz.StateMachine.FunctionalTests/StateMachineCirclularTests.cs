// <copyright file="StateMachineCirclularTests.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.FunctionalTests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sulucz.StateMachine.Builder;

    /// <summary>
    /// Tests different state machine circles.
    /// </summary>
    [TestClass]
    public class StateMachineCirclularTests
    {
        /// <summary>
        /// The time to wait at a gate.
        /// </summary>
        private static readonly TimeSpan GateWait = TimeSpan.FromSeconds(3);

        /// <summary>
        /// Test starting at one. Going to two. Then going back to one.
        /// </summary>
        [TestMethod]
        public void TestBackAndForth()
        {
            var oneToTwo = false;
            var twoToOne = false;
            var two = false;

            var factory = StateMachineFactory.CreateBuilder<NumberedTestEnum, TransitionEnum, int>();

            var finishGate = new ManualResetEventSlim(false);

            var first = factory.AddState(NumberedTestEnum.One);
            first.OnStateEnter(ctx =>
            {
                finishGate.Set();
                return Task.CompletedTask;
            });
            var second = factory.AddState(NumberedTestEnum.Two);
            second.OnStateEnter(ctx =>
            {
                two = true;
                ctx.Post(TransitionEnum.T2);
                return Task.CompletedTask;
            });

            first.AddValidTransition(TransitionEnum.T1, second)
                .AddTransitionFunction(ctx =>
                {
                    oneToTwo = true;
                    return Task.CompletedTask;
                });
            second.AddValidTransition(TransitionEnum.T2, first)
                .AddTransitionFunction(ctx =>
                {
                    twoToOne = true;
                    return Task.CompletedTask;
                });

            var stateMachine = factory.Compile();
            var session = stateMachine.StartStateMachine(NumberedTestEnum.One, 5);
            session.Post(TransitionEnum.T1);

            var one = finishGate.Wait(StateMachineCirclularTests.GateWait);
            Thread.MemoryBarrier();
            Assert.IsTrue(oneToTwo);
            Assert.IsTrue(two);
            Assert.IsTrue(twoToOne);
            Assert.IsTrue(one);
        }

        /// <summary>
        /// Test doing a full loop.
        /// </summary>
        [TestMethod]
        public void TestFullLoop()
        {
            var factory = StateMachineFactory.CreateBuilder<NumberedTestEnum, TransitionEnum, int>();

            var finishGate = new ManualResetEventSlim(false);
            var states = Enum.GetValues(typeof(NumberedTestEnum)).Cast<NumberedTestEnum>().Select(s => factory.AddState(s)).ToArray();

            var statecounts = new int[states.Length];

            for (var i = 0; i < states.Length; i++)
            {
                states[i].AddValidTransition(TransitionEnum.T1, states[(i + 1) % states.Length]);

                if (i != 0)
                {
                    var locali = i;
                    states[i].OnStateEnter(ctx =>
                    {
                        statecounts[locali]++;
                        ctx.Post(TransitionEnum.T1);
                        return Task.CompletedTask;
                    });
                }
            }

            states[0].OnStateEnter(ctx =>
            {
                if (++statecounts[0] < 5)
                {
                    ctx.Post(TransitionEnum.T1);
                }
                else
                {
                    finishGate.Set();
                }

                return Task.CompletedTask;
            });

            var stateMachine = factory.Compile();
            var context = stateMachine.StartStateMachine(NumberedTestEnum.One, 5);
            context.Post(TransitionEnum.T1);

            Assert.IsTrue(finishGate.Wait(StateMachineCirclularTests.GateWait));
            Assert.AreEqual(5, statecounts[0]);

            context.Dispose();
        }
    }
}
