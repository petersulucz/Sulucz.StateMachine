// <copyright file="StateMachineFactory.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Builder
{
    using Sulucz.StateMachine.Builder.Internal;

    /// <summary>
    /// Creates a state machine builder.
    /// </summary>
    public static class StateMachineFactory
    {
        /// <summary>
        /// Creates the state machine builder.
        /// </summary>
        /// <typeparam name="TState">The type of the state machine.</typeparam>
        /// <typeparam name="TTransition">The transition type of the state machine</typeparam>
        /// <typeparam name="TPayload">The payload type.</typeparam>
        /// <returns>The builder.</returns>
        public static IStateMachineBuilder<TState, TTransition, TPayload> CreateBuilder<TState, TTransition, TPayload>()
        where TState : System.Enum
        where TTransition : System.Enum
        {
            return new StateMachineBuilder<TState, TTransition, TPayload>();
        }
    }
}
