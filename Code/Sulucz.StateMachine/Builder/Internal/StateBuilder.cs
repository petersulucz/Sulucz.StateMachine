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

        public StateBuilder(TState state)
        {
            this.State = state;
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
        /// Adds a valid next state to this state.
        /// </summary>
        /// <param name="message">The transition message.</param>
        /// <param name="nextState">The valid next state.</param>
        /// <returns>The transition builder.</returns>
        public IStateTransitionBuilder<TState, TTransition, TPayload> AddValidTransition(TTransition message, IStateBuilder<TState, TTransition, TPayload> nextState)
        {
            if (this.nextStates.ContainsKey(message))
            {
                throw new ArgumentException($"Cannot add duplicate transition from {this.State} -> {nextState.State}. with transition {message}.");
            }

            var transition = new StateTransitionBuilder<TState, TTransition, TPayload>(message, this, (StateBuilder<TState, TTransition, TPayload>)nextState);
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
    }
}
