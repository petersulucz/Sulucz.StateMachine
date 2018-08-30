// <copyright file="StateMachineContext.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Sulucz.StateMachine.Internal;

    /// <summary>
    /// The state machine context.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Reviewed.")]
    public sealed class StateMachineContext<TState, TTransition, TPayload> : StateMachineContextBase<TState, TTransition, TPayload>
#if OLD_VERSION
        where TState : struct
        where TTransition : struct
#else
        where TState : System.Enum
        where TTransition : System.Enum
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineContext{TState,TTransition,TPayload}"/> class.
        /// </summary>
        /// <param name="currentState">The current state.</param>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="payload">THe payload.</param>
        internal StateMachineContext(StateMachineBase<TState, TTransition, TPayload> stateMachine, StateMachineState<TState, TTransition, TPayload> currentState, TPayload payload)
            : base(stateMachine, currentState, payload)
        {
        }
    }

    /// <summary>
    /// An abstract state machine context.
    /// </summary>
    internal abstract class StateMachineContext
    {
        /// <summary>
        /// Creates a new state machine context.
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <typeparam name="TTransition">The transition type.</typeparam>
        /// <typeparam name="TPayload">The payload type.</typeparam>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="currentState">The current state.</param>
        /// <param name="payload">The payload.</param>
        /// <returns>The context.</returns>
        public static StateMachineContext<TState, TTransition, TPayload> Create<TState, TTransition, TPayload>(StateMachineBase<TState, TTransition, TPayload> stateMachine, StateMachineState<TState, TTransition, TPayload> currentState, TPayload payload)
#if OLD_VERSION
            where TState : struct
            where TTransition : struct
#else
            where TState : System.Enum
            where TTransition : System.Enum
#endif
        {
            return new StateMachineContext<TState, TTransition, TPayload>(stateMachine, currentState, payload);
        }
    }
}
