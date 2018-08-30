// <copyright file="IStateMachineState.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine
{
    public interface IStateMachineState<TState>
        where TState : System.Enum
    {
        /// <summary>
        /// Gets the state.
        /// </summary>
        TState State { get; }
    }
}
