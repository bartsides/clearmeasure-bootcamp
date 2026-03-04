## ADDED Requirements

### Requirement: CompleteToAssignedCommand state command
The system SHALL provide a new state command `CompleteToAssignedCommand` that transitions a work order from Complete status to Assigned status, allowing the Creator to reassign completed work.

#### Scenario: Command is valid when status is Complete and user is Creator
- **GIVEN** a work order with status **Complete**
- **AND** the current user is the **Creator** of the work order
- **WHEN** `IsValid()` is called on the `CompleteToAssignedCommand`
- **THEN** it SHALL return `true`

#### Scenario: Command is not valid when status is not Complete
- **GIVEN** a work order with status **Draft** (or any status other than Complete)
- **WHEN** `IsValid()` is called on the `CompleteToAssignedCommand`
- **THEN** it SHALL return `false`

#### Scenario: Command is not valid when user is not Creator
- **GIVEN** a work order with status **Complete**
- **AND** the current user is NOT the Creator of the work order
- **WHEN** `IsValid()` is called on the `CompleteToAssignedCommand`
- **THEN** it SHALL return `false`

#### Scenario: Execute transitions status to Assigned and clears CompletedDate
- **GIVEN** a work order with status **Complete** and a CompletedDate
- **WHEN** `Execute()` is called on the `CompleteToAssignedCommand`
- **THEN** the work order status SHALL be **Assigned**
- **AND** the work order CompletedDate SHALL be **null**

### Requirement: Command metadata
The `CompleteToAssignedCommand` SHALL have `TransitionVerbPresentTense` equal to `"Reassign"` and `TransitionVerbPastTense` equal to `"Reassigned"`. The command SHALL match when `Matches("Reassign")` is called.

#### Scenario: Command matches by verb
- **WHEN** `Matches("Reassign")` is called on the `CompleteToAssignedCommand`
- **THEN** it SHALL return `true`

### Requirement: Command registered in StateCommandList
The `StateCommandList.GetAllStateCommands()` method SHALL include `CompleteToAssignedCommand` in the list of all state commands.

#### Scenario: All state commands include CompleteToAssignedCommand
- **WHEN** `GetAllStateCommands()` is called
- **THEN** the returned array SHALL contain an instance of `CompleteToAssignedCommand`

#### Scenario: Valid commands include Reassign for Complete work order with Creator
- **GIVEN** a work order with status **Complete**
- **AND** the current user is the Creator
- **WHEN** `GetValidStateCommands()` is called
- **THEN** the returned array SHALL contain the `CompleteToAssignedCommand`

### Requirement: Persistence through StateCommandHandler
When the `CompleteToAssignedCommand` is handled by the `StateCommandHandler`, the work order SHALL be persisted to the database with status Assigned, null CompletedDate, and the original Creator and Assignee preserved.

#### Scenario: Persisted reassignment
- **GIVEN** a work order in Complete status with a Creator and Assignee saved in the database
- **WHEN** the `CompleteToAssignedCommand` is sent through `StateCommandHandler`
- **THEN** the persisted work order SHALL have status **Assigned**
- **AND** the persisted work order CompletedDate SHALL be **null**
- **AND** the persisted work order Creator SHALL be unchanged
- **AND** the persisted work order Assignee SHALL be unchanged

### Requirement: Unit tests
Unit tests SHALL be added in `src/UnitTests/Core/Model/StateCommands/CompleteToAssignedCommandTests.cs` extending `StateCommandBaseTests`. Tests SHALL use NUnit `[TestFixture]` and `[Test]` attributes with `Assert.That` assertions.

#### Scenario: ShouldNotBeValidInWrongStatus
- **GIVEN** test method `ShouldNotBeValidInWrongStatus` exists
- **WHEN** a work order has status **Draft** and the command is created
- **THEN** `IsValid()` SHALL return `false`

#### Scenario: ShouldNotBeValidWithWrongEmployee
- **GIVEN** test method `ShouldNotBeValidWithWrongEmployee` exists
- **WHEN** a work order has status **Complete** but the current user is not the Creator
- **THEN** `IsValid()` SHALL return `false`

#### Scenario: ShouldBeValid
- **GIVEN** test method `ShouldBeValid` exists
- **WHEN** a work order has status **Complete** and the current user is the Creator
- **THEN** `IsValid()` SHALL return `true`

#### Scenario: ShouldTransitionStateProperly
- **GIVEN** test method `ShouldTransitionStateProperly` exists
- **WHEN** `Execute()` is called on a valid command
- **THEN** the work order status SHALL be **Assigned**
- **AND** the CompletedDate SHALL be **null**

### Requirement: Integration tests
Integration tests SHALL be added in `src/IntegrationTests/DataAccess/Handlers/StateCommandHandlerForReassignTests.cs` extending `IntegratedTestBase`. Tests SHALL use Shouldly assertions and follow the existing `StateCommandHandlerFor*Tests` pattern.

#### Scenario: ShouldSaveWorkOrderWithAssignedStatusAfterReassign
- **GIVEN** a work order in Complete status with Creator and Assignee persisted to the database
- **WHEN** the `CompleteToAssignedCommand` is handled by `StateCommandHandler`
- **THEN** the reloaded work order SHALL have status Assigned, null CompletedDate, and the original Creator and Assignee

#### Scenario: ShouldSaveWorkOrderWithRemotedCommand
- **GIVEN** a work order in Complete status persisted to the database
- **WHEN** the command is serialized/deserialized via `RemotableRequestTests.SimulateRemoteObject` and handled
- **THEN** the reloaded work order SHALL have status Assigned and the original Creator

### Constraints
- The `CompleteToAssignedCommand` SHALL be defined in `src/Core/Model/StateCommands/` (Core project)
- The command SHALL inherit from `StateCommandBase` and follow the same pattern as `CompleteToInProgressCommand`
- No new NuGet packages required
- No database schema changes required
- No new project references required
