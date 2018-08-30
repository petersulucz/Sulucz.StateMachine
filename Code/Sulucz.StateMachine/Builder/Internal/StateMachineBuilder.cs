// <copyright file="StateMachineBuilder.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Builder.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sulucz.StateMachine.Internal;

    /// <summary>
    /// The state machine builder.
    /// </summary>
    /// <typeparam name="TState">The type of the state machine.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    internal class StateMachineBuilder<TState, TTransition, TPayload> : IStateMachineBuilder<TState, TTransition, TPayload>
        where TState : System.Enum
        where TTransition : System.Enum
    {
        /// <summary>
        /// The set of states.
        /// </summary>
        private readonly IDictionary<TState, StateBuilder<TState, TTransition, TPayload>> states = new Dictionary<TState, StateBuilder<TState, TTransition, TPayload>>();

        /// <summary>
        /// Adds a new state to the state machine.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The state builder.</returns>
        public IStateBuilder<TState, TTransition, TPayload> AddState(TState state)
        {
            var newState = new StateBuilder<TState, TTransition, TPayload>(state);

            if (true == this.states.ContainsKey(state))
            {
                throw new ArgumentException($"The state {state} is already a member of this state machine.");
            }

            this.states[state] = newState;

            return newState;
        }

        /// <summary>
        /// Builds the state machine.
        /// </summary>
        /// <returns>The state machine.</returns>
        public IStateMachine<TState, TTransition, TPayload> Compile()
        {
            var stateMachine = new StateMachineBase<TState, TTransition, TPayload>();

            foreach (var state in this.states.Values)
            {
                stateMachine.AddState(new StateMachineState<TState, TTransition, TPayload>(state.State, state.OnEnter));
            }

            foreach (var transition in this.states.Values.SelectMany(state => state.Transitions))
            {
                stateMachine.AddTransition(transition.Message, transition.StartState.State, transition.EndState.State, transition.TransitionIntercepts);
            }

            return stateMachine;
        }
    }
}
