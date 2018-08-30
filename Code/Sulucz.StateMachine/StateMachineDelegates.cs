// <copyright file="StateMachineDelegates.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine
{
    using System.Threading.Tasks;

    /// <summary>
    /// The state machine delegate definitions.
    /// </summary>
    public static class StateMachineDelegates
    {
        public delegate Task StateMachineTransitionDel<TState, TTransition, TPayload>(StateMachineTransitionContext<TState, TTransition, TPayload> context)
            where TState : System.Enum
            where TTransition : System.Enum;

        public delegate Task StateMachineEnterDel<TState, TTransition, TPayload>(StateMachineContextBase<TState, TTransition, TPayload> context)
            where TState : System.Enum
            where TTransition : System.Enum;
    }
}
