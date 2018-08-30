// <copyright file="IStateMachineController.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine
{
    /// <summary>
    /// Controls the state machine.
    /// </summary>
    /// <typeparam name="TTransition">The transtion.</typeparam>
    public interface IStateMachineController<TTransition>
        where TTransition : System.Enum
    {
        /// <summary>
        /// Post a transition request.
        /// </summary>
        /// <param name="transition">The transition.</param>
        void Post(TTransition transition);
    }
}
