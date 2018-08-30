﻿// <copyright file="StateMachineState.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The state machinen state.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    internal class StateMachineState<TState, TTransition, TPayload> : IStateMachineState<TState>
        where TState : System.Enum
        where TTransition : System.Enum
    {
        /// <summary>
        /// The transitions.
        /// </summary>
        private readonly IDictionary<TTransition, StateMachineTransition<TState, TTransition, TPayload>> transitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineState{TState, TTransition, TPayload}"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="onEnter">The on enter method.</param>
        public StateMachineState(TState state, StateMachineDelegates.StateMachineEnterDel<TState, TTransition, TPayload> onEnter)
        {
            this.State = state;
            this.transitions = new Dictionary<TTransition, StateMachineTransition<TState, TTransition, TPayload>>();
            this.OnEnter = onEnter;
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public TState State { get; }

        /// <summary>
        /// Gets the onEnter method.
        /// </summary>
        public StateMachineDelegates.StateMachineEnterDel<TState, TTransition, TPayload> OnEnter { get; }

        /// <summary>
        /// Adds a new transition.
        /// </summary>
        /// <param name="transition">The transition.</param>
        public void AddTransition(StateMachineTransition<TState, TTransition, TPayload> transition)
        {
            if (true == this.transitions.ContainsKey(transition.Message))
            {
                throw new ArgumentException($"Cannot add duplicate transition {transition.Message} from state {this.State}.");
            }

            this.transitions.Add(transition.Message, transition);
        }

        /// <summary>
        /// Begin invoking the transition on the context.
        /// </summary>
        /// <param name="transition">The transition.</param>
        /// <param name="context">The context.</param>
        public void InvokeTransition(TTransition transition, StateMachineContextBase<TState, TTransition, TPayload> context)
        {
            if (false == this.transitions.TryGetValue(transition, out var tranny))
            {
                throw new InvalidOperationException($"The transition {transition} from the state {this.State} does not exist.");
            }

            // We are now transferring. Tranfer responsibility to the transition object.
            Task.Run(() => tranny.Execute(context));
        }
    }
}
