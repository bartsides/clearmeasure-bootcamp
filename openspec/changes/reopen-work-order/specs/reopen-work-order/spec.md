## ADDED Requirements

### Requirement: CompleteToInProgressCommand exists and follows the state command pattern
The system SHALL include a `CompleteToInProgressCommand` record in `src/Core/Model/StateCommands/` that extends `StateCommandBase`. The command SHALL implement the Complete-to-InProgress state transition with the verb "Reopen".

#### Scenario: Command class structure
- **WHEN** the `CompleteToInProgressCommand` class is examined
- **THEN** it SHALL be a `record` extending `StateCommandBase(WorkOrder, Employee)`
- **AND** `GetBeginStatus()` SHALL return `WorkOrderStatus.Complete`
- **AND** `GetEndStatus()` SHALL return `WorkOrderStatus.InProgress`
- **AND** `TransitionVerbPresentTense` SHALL return `"Reopen"`
- **AND** `TransitionVerbPastTense` SHALL return `"Reopened"`
- **AND** `Name` SHALL be the const string `"Reopen"`

### Requirement: Only the Assignee can reopen a work order
The `CompleteToInProgressCommand` SHALL only be valid when the current user is the work order's Assignee.

#### Scenario: Assignee can reopen
- **GIVEN** a work order with status `Complete`
- **AND** the current user is the work order's `Assignee`
- **WHEN** `IsValid()` is called
- **THEN** it SHALL return `true`

#### Scenario: Non-assignee cannot reopen
- **GIVEN** a work order with status `Complete`
- **AND** the current user is NOT the work order's `Assignee`
- **WHEN** `IsValid()` is called
- **THEN** it SHALL return `false`

#### Scenario: Wrong status prevents reopen
- **GIVEN** a work order with status other than `Complete` (e.g., `Draft`, `Assigned`, `InProgress`, `Cancelled`)
- **AND** the current user is the work order's `Assignee`
- **WHEN** `IsValid()` is called
- **THEN** it SHALL return `false`

### Requirement: Reopening changes status to InProgress and clears CompletedDate
When the `CompleteToInProgressCommand` is executed, the work order's status SHALL change to `InProgress` and the `CompletedDate` SHALL be cleared.

#### Scenario: Status transitions to InProgress
- **GIVEN** a work order with status `Complete` and a non-null `CompletedDate`
- **AND** the current user is the Assignee
- **WHEN** `Execute(context)` is called
- **THEN** `WorkOrder.Status` SHALL be `WorkOrderStatus.InProgress`
- **AND** `WorkOrder.CompletedDate` SHALL be `null`

### Requirement: StateCommandList includes the reopen command
The `StateCommandList.GetAllStateCommands()` method SHALL include `CompleteToInProgressCommand` in the list of available commands.

#### Scenario: Command is registered
- **WHEN** `GetAllStateCommands(workOrder, currentUser)` is called
- **THEN** the returned array SHALL contain an instance of `CompleteToInProgressCommand`
- **AND** the total command count SHALL be 7

#### Scenario: Command appears in valid commands for Complete work order with Assignee
- **GIVEN** a work order with status `Complete`
- **AND** the current user is the Assignee
- **WHEN** `GetValidStateCommands(workOrder, currentUser)` is called
- **THEN** the returned array SHALL contain a command with `TransitionVerbPresentTense == "Reopen"`

#### Scenario: Command does not appear for non-Complete work orders
- **GIVEN** a work order with status `InProgress`
- **WHEN** `GetValidStateCommands(workOrder, currentUser)` is called
- **THEN** the returned array SHALL NOT contain a command with `TransitionVerbPresentTense == "Reopen"`

### Requirement: UI automatically renders Reopen button
The existing `WorkOrderManage.razor` renders buttons for all valid commands via `@foreach (var command in ValidCommands)`. No UI changes are required — the Reopen button SHALL appear automatically when viewing a Complete work order as the Assignee.

#### Scenario: Reopen button visible for Assignee on Complete work order
- **GIVEN** a work order with status `Complete`
- **AND** the logged-in user is the Assignee
- **WHEN** the work order manage page is rendered
- **THEN** a button with text "Reopen" SHALL be visible
- **AND** the button SHALL have `data-testid` of `CommandButtonReopen`

#### Scenario: Reopen button not visible for non-Assignee
- **GIVEN** a work order with status `Complete`
- **AND** the logged-in user is NOT the Assignee
- **WHEN** the work order manage page is rendered
- **THEN** no "Reopen" button SHALL be visible
- **AND** the page SHALL display the read-only message

### Requirement: Unit tests cover the reopen command
Unit tests SHALL be added in `src/UnitTests/Core/Model/StateCommands/CompleteToInProgressCommandTests.cs` extending `StateCommandBaseTests`.

#### Scenario: ShouldNotBeValidInWrongStatus
- **GIVEN** a work order with status `Draft` (or any non-Complete status)
- **AND** the current user is the Assignee
- **WHEN** `IsValid()` is called on `CompleteToInProgressCommand`
- **THEN** it SHALL return `false`

#### Scenario: ShouldNotBeValidWithWrongEmployee
- **GIVEN** a work order with status `Complete`
- **AND** the current user is NOT the Assignee
- **WHEN** `IsValid()` is called on `CompleteToInProgressCommand`
- **THEN** it SHALL return `false`

#### Scenario: ShouldBeValid
- **GIVEN** a work order with status `Complete`
- **AND** the current user is the Assignee
- **WHEN** `IsValid()` is called on `CompleteToInProgressCommand`
- **THEN** it SHALL return `true`

#### Scenario: ShouldTransitionStateProperly
- **GIVEN** a work order with status `Complete`
- **WHEN** `Execute(context)` is called on `CompleteToInProgressCommand`
- **THEN** `WorkOrder.Status` SHALL be `WorkOrderStatus.InProgress`

#### Scenario: ShouldClearCompletedDateOnExecute
- **GIVEN** a work order with status `Complete` and `CompletedDate` set to a non-null value
- **WHEN** `Execute(context)` is called on `CompleteToInProgressCommand`
- **THEN** `WorkOrder.CompletedDate` SHALL be `null`

## Constraints

- The `CompleteToInProgressCommand` SHALL follow the exact same record pattern as `InProgressToCompleteCommand` and `AssignedToCancelledCommand`
- The command SHALL NOT raise any `IStateTransitionEvent` (matching the pattern of most existing commands)
- The `StateCommandHandler` in DataAccess SHALL NOT be modified — it already handles any `StateCommandBase` generically
- The Blazor UI template SHALL NOT be modified — it already renders buttons for all valid commands dynamically
- The `WorkOrder` entity SHALL NOT be modified — it already has `CompletedDate` and `ChangeStatus()` methods
