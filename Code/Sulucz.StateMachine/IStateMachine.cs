// <copyright file="IStateMachine.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine
{
    using System.Collections.Generic;

    public interface IStateMachine<TState, TTransition, TPayload>
#if OLD_VERSION
        where TState : struct
        where TTransition : struct
#else
        where TState : System.Enum
        where TTransition : System.Enum
#endif
    {
        /// <summary>
        /// Gets all of the states.
        /// </summary>
        IReadOnlyCollection<IStateMachineState<TState>> States { get; }

        /// <summary>
        /// Gets the contexts.
        /// </summary>
        IReadOnlyCollection<StateMachineContextBase<TState, TTransition, TPayload>> Contexts { get; }

        /// <summary>
        /// Starts a state machine.
        /// </summary>
        /// <param name="startState">The start state.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="executeStageEntry">True executes the OnEnter function of the first state, if it exists.</param>
        /// <returns>The state machine context.</returns>
        StateMachineContext<TState, TTransition, TPayload> StartStateMachine(TState startState, TPayload payload, bool executeStageEntry = false);
    }
}
