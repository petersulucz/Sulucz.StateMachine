// <copyright file="IStateMachineBuilder.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Builder
{
    /// <summary>
    /// The state machine builder.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    public interface IStateMachineBuilder<TState, TTransition, TPayload>
        where TState : System.Enum
        where TTransition : System.Enum
    {
        /// <summary>
        /// Adds a new state to the state machine.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The state builder.</returns>
        IStateBuilder<TState, TTransition, TPayload> AddState(TState state);

        /// <summary>
        /// Builds the state machine.
        /// </summary>
        /// <returns>The state machine.</returns>
        IStateMachine<TState, TTransition, TPayload> Compile();
    }
}
