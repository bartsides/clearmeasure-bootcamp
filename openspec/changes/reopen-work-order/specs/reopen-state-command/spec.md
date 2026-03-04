## ADDED Requirements

### Requirement: CompleteToAssignedCommand state command exists
The system SHALL include a state command `CompleteToAssignedCommand` in `src/Core/Model/StateCommands/` that transitions a work order from Complete status to Assigned status. The command SHALL extend `StateCommandBase` and follow the same record pattern as existing state commands.

#### Scenario: Command transitions status from Complete to Assigned
- **GIVEN** a work order with status Complete
- **AND** the current user is the Creator of the work order
- **WHEN** the `CompleteToAssignedCommand` is executed
- **THEN** the work order status SHALL be Assigned

#### Scenario: Command is valid when status is Complete and user is Creator
- **GIVEN** a work order with status Complete
- **AND** the current user is the Creator of the work order
- **WHEN** `IsValid()` is called on the command
- **THEN** it SHALL return true

#### Scenario: Command is invalid when status is not Complete
- **GIVEN** a work order with status other than Complete (e.g., Draft, Assigned, InProgress)
- **WHEN** `IsValid()` is called on the command
- **THEN** it SHALL return false

#### Scenario: Command is invalid when user is not the Creator
- **GIVEN** a work order with status Complete
- **AND** the current user is NOT the Creator of the work order
- **WHEN** `IsValid()` is called on the command
- **THEN** it SHALL return false

### Requirement: CompleteToAssignedCommand uses transition verb "Reopen"
The command SHALL have `TransitionVerbPresentTense` equal to `"Reopen"` and `TransitionVerbPastTense` equal to `"Reopened"`. The UI SHALL render a "Reopen" button for the Creator when viewing a completed work order.

### Requirement: CompleteToAssignedCommand is registered in StateCommandList
The `StateCommandList.GetAllStateCommands()` method SHALL include `CompleteToAssignedCommand` in the list of all state commands. The command SHALL appear after `AssignedToCancelledCommand` in the command list.

#### Scenario: StateCommandList includes 7 commands
- **WHEN** `GetAllStateCommands()` is called
- **THEN** it SHALL return 7 commands
- **AND** the last command SHALL be an instance of `CompleteToAssignedCommand`

### Constraints
- The command class SHALL be a `record` extending `StateCommandBase(WorkOrder, CurrentUser)`
- The command SHALL have `public const string Name = "Reopen"`
- `GetBeginStatus()` SHALL return `WorkOrderStatus.Complete`
- `GetEndStatus()` SHALL return `WorkOrderStatus.Assigned`
- `UserCanExecute()` SHALL return `true` only when `currentUser == WorkOrder.Creator`
- The command SHALL NOT modify `CompletedDate`, `AssignedDate`, or `Assignee`
- The command file SHALL be located at `src/Core/Model/StateCommands/CompleteToAssignedCommand.cs`
