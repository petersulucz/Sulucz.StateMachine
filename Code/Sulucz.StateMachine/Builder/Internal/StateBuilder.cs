// <copyright file="StateBuilder.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Builder.Internal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The state builder.
    /// </summary>
    /// <typeparam name="TState">The state builder type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    internal class StateBuilder<TState, TTransition, TPayload> : IStateBuilder<TState, TTransition, TPayload>
#if OLD_VERSION
        where TState : struct
        where TTransition : struct
#else
        where TState : System.Enum
        where TTransition : System.Enum
#endif
    {
        /// <summary>
        /// The valid next states.
        /// </summary>
        private readonly Dictionary<TTransition, StateTransitionBuilder<TState, TTransition, TPayload>> nextStates;

        /// <summary>
        /// A reference to the state machine.
        /// </summary>
        private readonly StateMachineBuilder<TState, TTransition, TPayload> stateMachine;

        public StateBuilder(TState state, StateMachineBuilder<TState, TTransition, TPayload> stateMachine)
        {
            this.State = state;
            this.stateMachine = stateMachine;
            this.nextStates = new Dictionary<TTransition, StateTransitionBuilder<TState, TTransition, TPayload>>();
        }

        /// <summary>
        /// Gets the state represented by this builder.
        /// </summary>
        public TState State { get; }

        /// <summary>
        /// Gets the onenter delegate.
        /// </summary>
        public StateMachineDelegates.StateMachineEnterDel<TState, TTransition, TPayload> OnEnter { get; private set; }

        /// <summary>
        /// Gets the valid next states.
        /// </summary>
        internal IReadOnlyCollection<StateTransitionBuilder<TState, TTransition, TPayload>> Transitions => this.nextStates.Values;

        /// <summary>
        /// Gets the fault handler for enter.
        /// </summary>
        internal Action<StateMachineContextBase<TState, TTransition, TPayload>, Exception> OnEnterFaultHandler { get; private set; }

        /// <summary>
        /// Adds a valid next state to this state.
        /// </summary>
        /// <param name="message">The transition message.</param>
        /// <param name="nextState">The valid next state.</param>
        /// <returns>The transition builder.</returns>
        public IStateTransitionBuilder<TState, TTransition, TPayload> AddValidTransition(TTransition message, IStateBuilder<TState, TTransition, TPayload> nextState)
        {
            if (this.nextStates.ContainsKey(message))
            {
                throw new ArgumentException($"Cannot add duplicate transition from {this.State} -> {((StateBuilder<TState, TTransition, TPayload>)nextState).State}. with transition {message}.");
            }

            var transition = new StateTransitionBuilder<TState, TTransition, TPayload>(message, this.stateMachine, this, (StateBuilder<TState, TTransition, TPayload>)nextState);
            this.nextStates.Add(message, transition);
            return transition;
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        /// <param name="onEnter">When the state is entered.</param>
        /// <returns>The state builder.</returns>
        public IStateBuilder<TState, TTransition, TPayload> OnStateEnter(StateMachineDelegates.StateMachineEnterDel<TState, TTransition, TPayload> onEnter)
        {
            if (null != this.OnEnter)
            {
                throw new ArgumentException("Cannot set the OnEnter method twice");
            }

            this.OnEnter = onEnter;
            return this;
        }

        /// <summary>
        /// Sets the fault handler for On-Enter.
        /// </summary>
        /// <param name="handler">The fault handler.</param>
        /// <returns>The state builder.</returns>
        public IStateBuilder<TState, TTransition, TPayload> SetOnEnterFaultHandler(Action<StateMachineContextBase<TState, TTransition, TPayload>, Exception> handler)
        {
            this.OnEnterFaultHandler = handler;
            return this;
        }

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
