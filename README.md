# Sulucz.StateMachine
A simple state machine.

## How to use

```csharp
    // Create a state machine builder
    // Each state is a member of the "TestEnum" enum
    // Each transition is a member of the "TestTransitionEnum" enum
    // The payload is an int
    var stateMachineBuilder = StateMachineFactory.CreateBuilder<TestEnum, TestTransitionEnum, int>();

    // Add three states
    var startState = stateMachineBuilder.AddState(TestEnum.Start);
    var middleState = stateMachineBuilder.AddState(TestEnum.Middle);
    var endState = stateMachineBuilder.AddState(TestEnum.End);

    // Add a valid transition from Start -> Middle
    startState.AddValidTransition(TestTransitionEnum.Middling, middleState);

    // Add a valid transition from Middle -> End
    middleState.AddValidTransition(TestTransitionEnum.Stopping, endState);

    // Called when enterning the middle state.
    middleState.OnStateEnter(ctx =>
        {
            ctx.Post(TestTransitionEnum.Stopping);
            return Task.CompletedTask;
        });

    // Called when entering the end state.
    endState.OnStateEnter(ctx =>
    {
        return Task.CompletedTask;
    });

    var stateMachine = stateMachineBuilder.Compile();

    // Start the state machine at state "Start" for the payload (4)
    var context = stateMachine.StartStateMachine(TestEnum.Start, 4);

    // Will post an event to the state machine, to kick off the "middleing" transition from the current state
    // In this case, will kick off a transition from Start -> Middle
    context.Post(TestTransitionEnum.Middling);
```