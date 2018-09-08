// <copyright file="IStateMachineBuilder.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Builder
{
    using System;

    /// <summary>
    /// The state machine builder.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    public interface IStateMachineBuilder<TState, TTransition, TPayload>
#if OLD_VERSION
        where TState : struct
        where TTransition : struct
#else
        where TState : System.Enum
        where TTransition : System.Enum
#endif
    {
        /// <summary>
        /// Adds a new state to the state machine.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The state builder.</returns>
        IStateBuilder<TState, TTransition, TPayload> AddState(TState state);

        /// <summary>
        /// Sets the fault handler.
        /// </summary>
        /// <param name="handler">The fault handler.</param>
        /// <returns>The state machine builder.</returns>
        IStateMachineBuilder<TState, TTransition, TPayload> SetGlobalFaultHandler(Action<StateMachineContextBase<TState, TTransition, TPayload>, Exception> handler);

        /// <summary>
        /// Builds the state machine.
        /// </summary>
        /// <returns>The state machine.</returns>
        IStateMachine<TState, TTransition, TPayload> Compile();
    }
}
