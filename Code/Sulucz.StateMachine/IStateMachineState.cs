// <copyright file="IStateMachineState.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine
{
    public interface IStateMachineState<TState>
#if OLD_VERSION
            where TState : struct
#else
            where TState : System.Enum
#endif
    {
        /// <summary>
        /// Gets the state.
        /// </summary>
        TState State { get; }
    }
}
