// <copyright file="StateMachineContextTests.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sulucz.StateMachine.Internal;

    /// <summary>
    /// The state machine context tests.
    /// </summary>
    [TestClass]
    public class StateMachineContextTests
    {
        /// <summary>
        /// Tests a basic new context.
        /// </summary>
        /// <returns>An async task.</returns>
        [TestMethod]
        public async Task TestNewContext()
        {
            var context = StateMachineContext.Create<BasicEnum, TestTransitionEnum, object>(null, new StateMachineState<BasicEnum, TestTransitionEnum, object>(BasicEnum.Start, null, null, null), null);
            Assert.IsNotNull(context);
            Assert.AreEqual(BasicEnum.Start, context.CurrentState.State);
            await Task.Delay(100);

            Assert.IsTrue(context.StateElapsedTime > TimeSpan.Zero);
            Assert.IsTrue(context.TotalElapsedTime > TimeSpan.Zero);
        }

        /// <summary>
        /// Tests setting the next state.
        /// </summary>
        /// <returns>An async task.</returns>
        [TestMethod]
        public async Task TestNextState()
        {
            var context = StateMachineContext.Create<BasicEnum, TestTransitionEnum, object>(null, new StateMachineState<BasicEnum, TestTransitionEnum, object>(BasicEnum.Start, null, null, null), null);
            await Task.Delay(100);
            context.SetState(new StateMachineState<BasicEnum, TestTransitionEnum, object>(BasicEnum.Stop, null, null, null));
            Assert.IsTrue(context.TotalElapsedTime > context.StateElapsedTime);
            Assert.AreEqual(BasicEnum.Stop, context.CurrentState.State);
        }

        /// <summary>
        /// Tests setting the next state.
        /// </summary>
        [TestMethod]
        public void TestNextState_Throws()
        {
            var context = StateMachineContext.Create<BasicEnum, TestTransitionEnum, object>(null, new StateMachineState<BasicEnum, TestTransitionEnum, object>(BasicEnum.Start, null, null, null), null);
            Assert.ThrowsException<ArgumentException>(() => context.SetState(new StateMachineState<BasicEnum, TestTransitionEnum, object>(BasicEnum.Start, null, null, null)));
        }

        /// <summary>
        /// A dummy enum.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "For testing.")]
        private enum BasicEnum
        {
            Start,
            Stop
        }
    }
}
