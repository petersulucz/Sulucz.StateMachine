// <copyright file="StateMachineBuilderTests.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Tests
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sulucz.StateMachine.Builder;
    using Sulucz.StateMachine.Builder.Internal;

    /// <summary>
    /// Tests the SM builder.
    /// </summary>
    [TestClass]
    public class StateMachineBuilderTests
    {
        /// <summary>
        /// Test adding a new state.
        /// </summary>
        [TestMethod]
        public void TestAddState()
        {
            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, object>();
            var newState = builder.AddState(TestEnum.Start);

            Assert.IsNotNull(newState);
            Assert.AreEqual(TestEnum.Start, ((StateBuilder<TestEnum, TestTransitionEnum, object>)newState).State);
        }

        /// <summary>
        /// Test adding a state twice.
        /// </summary>
        [TestMethod]
        public void TestAddStateDuplicate()
        {
            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, object>();
            var newState = builder.AddState(TestEnum.Start);

            Assert.ThrowsException<ArgumentException>(() => builder.AddState(TestEnum.Start));
        }

        /// <summary>
        /// Tests adding a next state.
        /// </summary>
        [TestMethod]
        public void TestAddNextState()
        {
            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, object>();
            var startState = builder.AddState(TestEnum.Start);
            var nextState = builder.AddState(TestEnum.End);

            Assert.IsFalse(((StateBuilder<TestEnum, TestTransitionEnum, object>)startState).Transitions.Any());
            Assert.IsFalse(((StateBuilder<TestEnum, TestTransitionEnum, object>)nextState).Transitions.Any());
            startState.AddValidTransition(TestTransitionEnum.Stopping, nextState);

            Assert.AreSame(nextState, ((StateBuilder<TestEnum, TestTransitionEnum, object>)startState).Transitions.Single().EndState);
            Assert.IsFalse(((StateBuilder<TestEnum, TestTransitionEnum, object>)nextState).Transitions.Any());
        }

        /// <summary>
        /// Tests adding a next state duplicate.
        /// </summary>
        [TestMethod]
        public void TestAddNextStateDuplicate()
        {
            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, object>();
            var startState = builder.AddState(TestEnum.Start);

            var endState = builder.AddState(TestEnum.End);
            startState.AddValidTransition(TestTransitionEnum.Stopping, endState);

            Assert.ThrowsException<ArgumentException>(() => startState.AddValidTransition(TestTransitionEnum.Stopping, endState));
        }

        /// <summary>
        /// Tests an empty state machine.
        /// </summary>
        [TestMethod]
        public void TestStateMachineCompileEmpty()
        {
            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, object>();

            var stateMachine = builder.Compile();
            Assert.IsFalse(stateMachine.States.Any());
        }

        /// <summary>
        /// Tests a state machine with one state.
        /// </summary>
        [TestMethod]
        public void TestStateMachineOneState()
        {
            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, object>();
            builder.AddState(TestEnum.Start);

            var stateMachine = builder.Compile();
            Assert.AreEqual(TestEnum.Start, stateMachine.States.First().State);
        }

        /// <summary>
        /// Tests a state machine with one transition.
        /// </summary>
        [TestMethod]
        public void TestStateMachineTransition()
        {
            var builder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, object>();
            var start = builder.AddState(TestEnum.Start);
            var end = builder.AddState(TestEnum.End);

            start.AddValidTransition(TestTransitionEnum.Stopping, end);

            var stateMachine = builder.Compile();
            Assert.AreEqual(2, stateMachine.States.Count);
        }
    }
}
