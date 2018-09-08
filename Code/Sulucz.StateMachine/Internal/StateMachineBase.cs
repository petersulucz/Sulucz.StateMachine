// <copyright file="StateMachineBase.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sulucz.Common;

    /// <summary>
    /// The state machine base class.
    /// </summary>
    /// <typeparam name="TState">The machine type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="TPayload">The transition payload.</typeparam>
    internal class StateMachineBase<TState, TTransition, TPayload> : IStateMachine<TState, TTransition, TPayload>
#if OLD_VERSION
        where TState : struct
        where TTransition : struct
#else
        where TState : System.Enum
        where TTransition : System.Enum
#endif
    {
        /// <summary>
        /// The states collection.
        /// </summary>
        private readonly IDictionary<TState, StateMachineState<TState, TTransition, TPayload>> states;

        /// <summary>
        /// The fault handler.
        /// </summary>
        private readonly Action<StateMachineContextBase<TState, TTransition, TPayload>, Exception> faultHandler;

        /// <summary>
        /// The context set.
        /// </summary>
        private readonly HashSet<StateMachineContextBase<TState, TTransition, TPayload>> contexts;

        public StateMachineBase(Action<StateMachineContextBase<TState, TTransition, TPayload>, Exception> faultHandler)
        {
            this.states = new Dictionary<TState, StateMachineState<TState, TTransition, TPayload>>();
            this.contexts = new HashSet<StateMachineContextBase<TState, TTransition, TPayload>>();
            this.faultHandler = faultHandler;
        }

        /// <summary>
        /// Gets all of the states.
        /// </summary>
        IReadOnlyCollection<IStateMachineState<TState>> IStateMachine<TState, TTransition, TPayload>.States => this.states.Values.Cast<IStateMachineState<TState>>().ToReadOnlyCollection();

        /// <summary>
        /// Gets the contexts.
        /// </summary>
        public IReadOnlyCollection<StateMachineContextBase<TState, TTransition, TPayload>> Contexts
        {
            get
            {
                lock (this.contexts)
                {
                    return this.contexts.ToReadOnlyCollection();
                }
            }
        }

        /// <summary>
        /// Gets the states.
        /// </summary>
        internal IReadOnlyCollection<StateMachineState<TState, TTransition, TPayload>> States
        {
            get
            {
                lock (this.states)
                {
                    return this.states.Values.ToReadOnlyCollection();
                }
            }
        }

        /// <summary>
        /// Starts a state machine.
        /// </summary>
        /// <param name="startState">The start state.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="executeStageEntry">True executes the OnEnter function of the first state, if it exists.</param>
        /// <returns>The state machine context.</returns>
        public StateMachineContext<TState, TTransition, TPayload> StartStateMachine(TState startState, TPayload payload, bool executeStageEntry)
        {
            var currentState = this.states[startState];
            var context = StateMachineContext.Create(this, currentState, payload);

            lock (this.contexts)
            {
                this.contexts.Add(context);
            }

            // If true, fire off the first stage.
            if (true == executeStageEntry)
            {
                currentState.InvokeEnter(context);
            }

            return context;
        }

        /// <summary>
        /// Remove the context.
        /// </summary>
        /// <param name="context">The context.</param>
        internal void RemoveContext(StateMachineContextBase<TState, TTransition, TPayload> context)
        {
            lock (this.contexts)
            {
                this.contexts.Remove(context);
            }
        }

        /// <summary>
        /// Adds a state to the state machine.
        /// </summary>
        /// <param name="state">The state.</param>
        internal void AddState(StateMachineState<TState, TTransition, TPayload> state)
        {
            if (true == this.states.ContainsKey(state.State))
            {
                throw new ArgumentException($"The state machine has duplicate state '{state.State}' defined.");
            }

            this.states.Add(state.State, state);
        }

        /// <summary>
        /// Adds a transition.
        /// </summary>
        /// <param name="transition">The transition.</param>
        /// <param name="start">The start state.</param>
        /// <param name="end">The end state.</param>
        /// <param name="intercepts">The interceptors.</param>
        internal void AddTransition(TTransition transition, TState start, TState end, IEnumerable<StateMachineDelegates.StateMachineTransitionDel<TState, TTransition, TPayload>> intercepts)
        {
            if (false == this.states.TryGetValue(start, out var startState) || false == this.states.TryGetValue(end, out var endState))
            {
                throw new ArgumentException($"Clould not create transition {transition} from {start} => {end} because one of the states is missing.");
            }

            startState.AddTransition(new StateMachineTransition<TState, TTransition, TPayload>(transition, this, startState, endState, intercepts));
        }
    }
}
