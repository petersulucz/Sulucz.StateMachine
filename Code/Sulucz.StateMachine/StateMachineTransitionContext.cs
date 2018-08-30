// <copyright file="StateMachineTransitionContext.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine
{
    using System.Diagnostics.CodeAnalysis;
    using Sulucz.StateMachine.Internal;

    /// <summary>
    /// The state machine transition context.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Reviewed.")]
    public sealed class StateMachineTransitionContext<TState, TTransition, TPayload> : StateMachineContextBase<TState, TTransition, TPayload>
        where TState : System.Enum
        where TTransition : System.Enum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineTransitionContext{TState, TTransition, TPayload}"/> class.
        /// </summary>
        /// <param name="startState">The start state.</param>
        /// <param name="endState">The end state.</param>
        /// <param name="transition">The transition.</param>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="payload">The payload.</param>
        internal StateMachineTransitionContext(StateMachineState<TState, TTransition, TPayload> startState, TState endState, TTransition transition, StateMachineBase<TState, TTransition, TPayload> stateMachine, TPayload payload)
            : base(stateMachine, startState, payload)
        {
            this.StartState = startState.State;
            this.EndState = endState;
            this.Transition = transition;
        }

        /// <summary>
        /// Gets the start state.
        /// </summary>
        public TState StartState { get; }

        /// <summary>
        /// Gets the end state.
        /// </summary>
        public TState EndState { get; }

        /// <summary>
        /// Gets the transition type.
        /// </summary>
        public TTransition Transition { get; }
    }

    /// <summary>
    /// The state machine transition context creator class.
    /// </summary>
    internal static class StateMachineTransitionContext
    {
        public static StateMachineTransitionContext<TState, TTransition, TPayload> Create<TState, TTransition, TPayload>(StateMachineState<TState, TTransition, TPayload> startState, TState endState, TTransition transition, StateMachineBase<TState, TTransition, TPayload> stateMachine, TPayload payload)
            where TState : System.Enum
            where TTransition : System.Enum
        {
            return new StateMachineTransitionContext<TState, TTransition, TPayload>(startState, endState, transition, stateMachine, payload);
        }
    }
}
