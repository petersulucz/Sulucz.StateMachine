// <copyright file="StateTransitionBuilder.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Builder.Internal
{
    using System.Collections.Generic;
    using Sulucz.Common;
    using static Sulucz.StateMachine.StateMachineDelegates;

    /// <summary>
    /// The state machine transition builder.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    internal class StateTransitionBuilder<TState, TTransition, TPayload> : IStateTransitionBuilder<TState, TTransition, TPayload>
        where TState : System.Enum
        where TTransition : System.Enum
    {
        /// <summary>
        /// The functions to call on transition.
        /// </summary>
        private readonly IList<StateMachineTransitionDel<TState, TTransition, TPayload>> onTransitionFunctions = new List<StateMachineTransitionDel<TState, TTransition, TPayload>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StateTransitionBuilder{TState, TTransition, TPayload}"/> class.
        /// </summary>
        /// <param name="transitionMessage">The transition message.</param>
        /// <param name="startState">The start state.</param>
        /// <param name="endState">The end state.</param>
        public StateTransitionBuilder(TTransition transitionMessage, StateBuilder<TState, TTransition, TPayload> startState, StateBuilder<TState, TTransition, TPayload> endState)
        {
            this.Message = transitionMessage;
            this.StartState = startState;
            this.EndState = endState;
        }

        /// <summary>
        /// Gets the transition message.
        /// </summary>
        public TTransition Message { get; }

        /// <summary>
        /// Gets the transition interceptor fuctions.
        /// </summary>
        public IReadOnlyCollection<StateMachineTransitionDel<TState, TTransition, TPayload>> TransitionIntercepts => this.onTransitionFunctions.ToReadOnlyCollection();

        /// <summary>
        /// Gets the start state.
        /// </summary>
        public StateBuilder<TState, TTransition, TPayload> StartState { get; }

        /// <summary>
        /// Gets the end state.
        /// </summary>
        public StateBuilder<TState, TTransition, TPayload> EndState { get; }

        /// <summary>
        /// Add a function to call on transition.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        /// <returns>this.</returns>
        IStateTransitionBuilder<TState, TTransition, TPayload> IStateTransitionBuilder<TState, TTransition, TPayload>.AddTransitionFunction(StateMachineTransitionDel<TState, TTransition, TPayload> func)
        {
            this.onTransitionFunctions.Add(func);
            return this;
        }
    }
}
