// <copyright file="IStateTransitionBuilder.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Builder
{
    using static Sulucz.StateMachine.StateMachineDelegates;

    public interface IStateTransitionBuilder<TState, TTransition, TPayload> : IStateBuilder<TState, TTransition, TPayload>
#if OLD_VERSION
        where TState : struct
        where TTransition : struct
#else
        where TState : System.Enum
        where TTransition : System.Enum
#endif
    {
        /// <summary>
        /// Add a function to call on transition.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        /// <returns>this.</returns>
        IStateTransitionBuilder<TState, TTransition, TPayload> AddTransitionFunction(StateMachineTransitionDel<TState, TTransition, TPayload> func);
    }
}
