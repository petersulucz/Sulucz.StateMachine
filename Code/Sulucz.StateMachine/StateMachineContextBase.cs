﻿// <copyright file="StateMachineContextBase.cs" company="Peter Sulucz">
// Copyright (c) Peter Sulucz. All rights reserved.
// </copyright>

namespace Sulucz.StateMachine
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Sulucz.StateMachine.Internal;

    /// <summary>
    /// The base state machine context.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    public abstract class StateMachineContextBase<TState, TTransition, TPayload> : IStateMachineController<TTransition>, IDisposable
        where TState : System.Enum
        where TTransition : System.Enum
    {
        /// <summary>
        /// The total elapsed time.
        /// </summary>
        private readonly Stopwatch totalElapsedTime;

        /// <summary>
        /// The state elapsed time.
        /// </summary>
        private readonly Stopwatch stateElapsedTime;

        /// <summary>
        /// The cancellation token source.
        /// </summary>
        private readonly CancellationTokenSource tokenSource;

        /// <summary>
        /// The state machine.
        /// </summary>
        private readonly StateMachineBase<TState, TTransition, TPayload> stateMachine;

        /// <summary>
        /// The transition lock.
        /// </summary>
        private readonly object transitionLock = new object();

        /// <summary>
        /// True if the state is in transition.
        /// </summary>
        private bool inTransition = false;

        internal StateMachineContextBase(StateMachineBase<TState, TTransition, TPayload> stateMachine, StateMachineState<TState, TTransition, TPayload> currentState, TPayload payload)
        {
            this.stateMachine = stateMachine;
            this.Payload = payload;
            this.CurrentState = currentState;
            this.stateElapsedTime = Stopwatch.StartNew();
            this.totalElapsedTime = Stopwatch.StartNew();
            this.tokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Gets the amount of time spent in the state machine.
        /// </summary>
        public TimeSpan TotalElapsedTime => this.totalElapsedTime.Elapsed;

        /// <summary>
        /// Gets the amount of time spent in this current state.
        /// </summary>
        public TimeSpan StateElapsedTime => this.stateElapsedTime.Elapsed;

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public TState Current => this.CurrentState.State;

        /// <summary>
        /// Gets the payload.
        /// </summary>
        public TPayload Payload { get; }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        internal StateMachineState<TState, TTransition, TPayload> CurrentState { get; private set; }

        /// <summary>
        /// Cleanup.
        /// </summary>
        public void Dispose()
        {
            this.tokenSource.Cancel();
            this.tokenSource.Dispose();
        }

        /// <summary>
        /// Post a transition request.
        /// </summary>
        /// <param name="transition">The transition.</param>
        public void Post(TTransition transition)
        {
            lock (this.transitionLock)
            {
                if (true == this.inTransition)
                {
                    throw new Exception("Cannot switch state while in transition");
                }

                this.inTransition = true;
            }

            this.CurrentState.InvokeTransition(transition, this);
        }

        /// <summary>
        /// Sets the next state.
        /// </summary>
        /// <param name="state">The next state.</param>
        internal void SetState(StateMachineState<TState, TTransition, TPayload> state)
        {
            lock (this.transitionLock)
            {
                if (this.CurrentState.State.Equals(state.State))
                {
                    throw new ArgumentException($"Attempt to set state: {state.State} to itself.");
                }

                this.CurrentState = state;
                this.stateElapsedTime.Restart();
                this.inTransition = false;
            }
        }
    }
}
