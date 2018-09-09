// <copyright file="StateTransitionBuilder.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Builder.Internal
{
    using System;
    using System.Collections.Generic;
    using Sulucz.Common;
    using static Sulucz.StateMachine.StateMachineDelegates;

    /// <summary>
    /// The state machine transition builder.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    internal class StateTransitionBuilder<TState, TTransition, TPayload> : IStateTransitionBuilder<TState, TTransition, TPayload>
#if OLD_VERSION
        where TState : struct
        where TTransition : struct
#else
        where TState : System.Enum
        where TTransition : System.Enum
#endif
    {
        /// <summary>
        /// The functions to call on transition.
        /// </summary>
        private readonly IList<StateMachineTransitionDel<TState, TTransition, TPayload>> onTransitionFunctions = new List<StateMachineTransitionDel<TState, TTransition, TPayload>>();

        /// <summary>
        /// The state machine.
        /// </summary>
        private readonly StateMachineBuilder<TState, TTransition, TPayload> stateMachine;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateTransitionBuilder{TState, TTransition, TPayload}"/> class.
        /// </summary>
        /// <param name="transitionMessage">The transition message.</param>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="startState">The start state.</param>
        /// <param name="endState">The end state.</param>
        public StateTransitionBuilder(TTransition transitionMessage, StateMachineBuilder<TState, TTransition,  TPayload> stateMachine, StateBuilder<TState, TTransition, TPayload> startState, StateBuilder<TState, TTransition, TPayload> endState)
        {
            this.Message = transitionMessage;
            this.stateMachine = stateMachine;
            this.StartState = startState;
            this.EndState = endState;
        }

        /// <summary>
        /// Gets the transition message.
        /// </summary>
        public TTransition Message { get; }

        /// <summary>
        /// Gets the transition interceptor fuctions.
        /// </summary>
        public IReadOnlyCollection<StateMachineTransitionDel<TState, TTransition, TPayload>> TransitionIntercepts => this.onTransitionFunctions.ToReadOnlyCollection();

        /// <summary>
        /// Gets the start state.
        /// </summary>
        public StateBuilder<TState, TTransition, TPayload> StartState { get; }

        /// <summary>
        /// Gets the end state.
        /// </summary>
        public StateBuilder<TState, TTransition, TPayload> EndState { get; }

        /// <summary>
        /// Add a function to call on transition.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        /// <returns>this.</returns>
        public IStateTransitionBuilder<TState, TTransition, TPayload> AddTransitionFunction(StateMachineTransitionDel<TState, TTransition, TPayload> func)
        {
            this.onTransitionFunctions.Add(func);
            return this;
        }

        #region IStateBuilder

        /// <summary>
        /// Adds a valid next state to this state.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="nextState">The valid next state.</param>
        /// <returns>This.</returns>
        public IStateTransitionBuilder<TState, TTransition, TPayload> AddValidTransition(TTransition message, IStateBuilder<TState, TTransition, TPayload> nextState)
        {
            return this.StartState.AddValidTransition(message, nextState);
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        /// <param name="onEnter">When the state is entered.</param>
        /// <returns>The state builder.</returns>
        public IStateBuilder<TState, TTransition, TPayload> OnStateEnter(StateMachineEnterDel<TState, TTransition, TPayload> onEnter)
        {
            return this.StartState.OnStateEnter(onEnter);
        }

        /// <summary>
        /// Sets the fault handler for On-Enter.
        /// </summary>
        /// <param name="handler">The fault handler.</param>
        /// <returns>The state builder.</returns>
        public IStateBuilder<TState, TTransition, TPayload> SetOnEnterFaultHandler(Action<StateMachineContextBase<TState, TTransition, TPayload>, Exception> handler)
        {
            return this.StartState.SetOnEnterFaultHandler(handler);
        }

        #endregion

        #region IStateMachineBuilder

        /// <summary>
        /// Builds the state machine.
        /// </summary>
        /// <returns>The state machine.</returns>
        public IStateMachine<TState, TTransition, TPayload> Compile()
        {
            return this.stateMachine.Compile();
        }

        /// <summary>
        /// Adds a new state to the state machine.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The state builder.</returns>
        public IStateBuilder<TState, TTransition, TPayload> AddState(TState state)
        {
            return this.stateMachine.AddState(state);
        }

        /// <summary>
        /// Sets the fault handler.
        /// </summary>
        /// <param name="handler">The fault handler.</param>
        /// <returns>The state machine builder.</returns>
        public IStateMachineBuilder<TState, TTransition, TPayload> SetGlobalFaultHandler(Action<StateMachineContextBase<TState, TTransition, TPayload>, Exception> handler)
        {
            return this.stateMachine.SetGlobalFaultHandler(handler);
        }

        #endregion
    }
}
