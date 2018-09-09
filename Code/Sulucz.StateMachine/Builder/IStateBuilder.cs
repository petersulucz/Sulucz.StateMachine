// <copyright file="IStateBuilder.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Builder
{
    using System;

    /// <summary>
    /// A state builder.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TTransition">The type of the transition.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    public interface IStateBuilder<TState, TTransition, TPayload> : IStateMachineBuilder<TState, TTransition, TPayload>
#if OLD_VERSION
        where TState : struct
        where TTransition : struct
#else
        where TState : System.Enum
        where TTransition : System.Enum
#endif
    {
        /// <summary>
        /// Adds a valid next state to this state.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="nextState">The valid next state.</param>
        /// <returns>This.</returns>
        IStateTransitionBuilder<TState, TTransition, TPayload> AddValidTransition(TTransition message, IStateBuilder<TState, TTransition, TPayload> nextState);

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        /// <param name="onEnter">When the state is entered.</param>
        /// <returns>The state builder.</returns>
        IStateBuilder<TState, TTransition, TPayload> OnStateEnter(StateMachineDelegates.StateMachineEnterDel<TState, TTransition, TPayload> onEnter);

        /// <summary>
        /// Sets the fault handler for On-Enter.
        /// </summary>
        /// <param name="handler">The fault handler.</param>
        /// <returns>The state builder.</returns>
        IStateBuilder<TState, TTransition, TPayload> SetOnEnterFaultHandler(Action<StateMachineContextBase<TState, TTransition, TPayload>, Exception> handler);
    }
}
