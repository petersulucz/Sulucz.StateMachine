// <copyright file="StateMachineTransition.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Sulucz.StateMachine.StateMachineDelegates;

    /// <summary>
    /// The state machine transition class.
    /// </summary>
    /// <typeparam name="TState">The state.</typeparam>
    /// <typeparam name="TTransition">The transition.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    internal class StateMachineTransition<TState, TTransition, TPayload>
#if OLD_VERSION
        where TState : struct
        where TTransition : struct
#else
        where TState : System.Enum
        where TTransition : System.Enum
#endif
    {
        /// <summary>
        /// The state machine.
        /// </summary>
        private readonly StateMachineBase<TState, TTransition, TPayload> stateMachine;

        /// <summary>
        /// The initial state.
        /// </summary>
        private readonly StateMachineState<TState, TTransition, TPayload> startState;

        /// <summary>
        /// The end state.
        /// </summary>
        private readonly StateMachineState<TState, TTransition, TPayload> endState;

        /// <summary>
        /// The state machine transition delegates.
        /// </summary>
        private readonly IList<StateMachineTransitionDel<TState, TTransition, TPayload>> onTransitionDelegate;

        public StateMachineTransition(
            TTransition current,
            StateMachineBase<TState, TTransition, TPayload> stateMachine,
            StateMachineState<TState, TTransition, TPayload> startState,
            StateMachineState<TState, TTransition, TPayload> endState,
            IEnumerable<StateMachineTransitionDel<TState, TTransition, TPayload>> onTransitionDelegate)
        {
            this.stateMachine = stateMachine;
            this.startState = startState;
            this.endState = endState;
            this.onTransitionDelegate = onTransitionDelegate.ToList();
            this.Message = current;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public TTransition Message { get; }

        /// <summary>
        /// Execute the inter-transition interceptor.
        /// </summary>
        /// <param name="stateMachineContext">The state machine context.</param>
        /// <returns>An async task.</returns>
        internal async Task Execute(StateMachineContextBase<TState, TTransition, TPayload> stateMachineContext)
        {
            if (true == this.onTransitionDelegate.Any())
            {
                var context = StateMachineTransitionContext.Create(this.startState, this.endState.State, this.Message, this.stateMachine, stateMachineContext.Payload);
                await Task.WhenAll(this.onTransitionDelegate.Select(d => this.HandlerExecutor(d, context)));
            }

            // We can continue on the same thread for the executing the next state.
            stateMachineContext.SetState(this.endState);

            // Fire of the state entrance.
            this.endState.InvokeEnter(stateMachineContext);
        }

        /// <summary>
        /// The method handler exector.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="context">The transition context.</param>
        /// <returns>An async task.</returns>
        private async Task HandlerExecutor(StateMachineTransitionDel<TState, TTransition, TPayload> action, StateMachineTransitionContext<TState, TTransition, TPayload> context)
        {
            try
            {
                await action(context);
            }
            catch (Exception ex)
            {
                // Do nothing for now.
            }
        }
    }
}
