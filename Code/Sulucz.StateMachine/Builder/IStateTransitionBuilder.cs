// <copyright file="IStateTransitionBuilder.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Builder
{
    using static Sulucz.StateMachine.StateMachineDelegates;

    public interface IStateTransitionBuilder<TState, TTransition, TPayload>
        where TState : System.Enum
        where TTransition : System.Enum
    {
        /// <summary>
        /// Add a function to call on transition.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        /// <returns>this.</returns>
        IStateTransitionBuilder<TState, TTransition, TPayload> AddTransitionFunction(StateMachineTransitionDel<TState, TTransition, TPayload> func);
    }
}
