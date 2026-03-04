## ADDED Requirements

### Requirement: AssignedToDraftCommand state command
The system SHALL provide a new state command `AssignedToDraftCommand` that transitions a work order from Assigned status to Draft status, clearing the assignment.

#### Scenario: Command is valid when status is Assigned and user is Creator
- **GIVEN** a work order with status **Assigned**
- **AND** the current user is the **Creator** of the work order
- **WHEN** `IsValid()` is called on the `AssignedToDraftCommand`
- **THEN** it SHALL return `true`

#### Scenario: Command is not valid when status is not Assigned
- **GIVEN** a work order with status **Draft** (or any status other than Assigned)
- **WHEN** `IsValid()` is called on the `AssignedToDraftCommand`
- **THEN** it SHALL return `false`

#### Scenario: Command is not valid when user is not Creator
- **GIVEN** a work order with status **Assigned**
- **AND** the current user is NOT the Creator of the work order
- **WHEN** `IsValid()` is called on the `AssignedToDraftCommand`
- **THEN** it SHALL return `false`

#### Scenario: Execute transitions status to Draft and clears assignment
- **GIVEN** a work order with status **Assigned**, an Assignee, and an AssignedDate
- **WHEN** `Execute()` is called on the `AssignedToDraftCommand`
- **THEN** the work order status SHALL be **Draft**
- **AND** the work order Assignee SHALL be **null**
- **AND** the work order AssignedDate SHALL be **null**

### Requirement: Command metadata
The `AssignedToDraftCommand` SHALL have `TransitionVerbPresentTense` equal to `"Unassign"` and `TransitionVerbPastTense` equal to `"Unassigned"`. The command SHALL match when `Matches("Unassign")` is called.

#### Scenario: Command matches by verb
- **WHEN** `Matches("Unassign")` is called on the `AssignedToDraftCommand`
- **THEN** it SHALL return `true`

### Requirement: Command registered in StateCommandList
The `StateCommandList.GetAllStateCommands()` method SHALL include `AssignedToDraftCommand` in the list of all state commands.

#### Scenario: All state commands include AssignedToDraftCommand
- **WHEN** `GetAllStateCommands()` is called
- **THEN** the returned array SHALL contain an instance of `AssignedToDraftCommand`

#### Scenario: Valid commands include Unassign for Assigned work order with Creator
- **GIVEN** a work order with status **Assigned**
- **AND** the current user is the Creator
- **WHEN** `GetValidStateCommands()` is called
- **THEN** the returned array SHALL contain the `AssignedToDraftCommand`

### Requirement: Persistence through StateCommandHandler
When the `AssignedToDraftCommand` is handled by the `StateCommandHandler`, the work order SHALL be persisted to the database with status Draft, null Assignee, and null AssignedDate.

#### Scenario: Persisted unassignment
- **GIVEN** a work order in Assigned status with a Creator and Assignee saved in the database
- **WHEN** the `AssignedToDraftCommand` is sent through `StateCommandHandler`
- **THEN** the persisted work order SHALL have status **Draft**
- **AND** the persisted work order Assignee SHALL be **null**
- **AND** the persisted work order AssignedDate SHALL be **null**
- **AND** the persisted work order Creator SHALL be unchanged

### Requirement: Unit tests
Unit tests SHALL be added in `src/UnitTests/Core/Model/StateCommands/AssignedToDraftCommandTests.cs` extending `StateCommandBaseTests`. Tests SHALL use NUnit `[TestFixture]` and `[Test]` attributes with `Assert.That` assertions.

#### Scenario: ShouldNotBeValidInWrongStatus
- **GIVEN** test method `ShouldNotBeValidInWrongStatus` exists
- **WHEN** a work order has status **Draft** and the command is created
- **THEN** `IsValid()` SHALL return `false`

#### Scenario: ShouldNotBeValidWithWrongEmployee
- **GIVEN** test method `ShouldNotBeValidWithWrongEmployee` exists
- **WHEN** a work order has status **Assigned** but the current user is not the Creator
- **THEN** `IsValid()` SHALL return `false`

#### Scenario: ShouldBeValid
- **GIVEN** test method `ShouldBeValid` exists
- **WHEN** a work order has status **Assigned** and the current user is the Creator
- **THEN** `IsValid()` SHALL return `true`

#### Scenario: ShouldTransitionStateProperly
- **GIVEN** test method `ShouldTransitionStateProperly` exists
- **WHEN** `Execute()` is called on a valid command
- **THEN** the work order status SHALL be **Draft**
- **AND** the Assignee SHALL be **null**
- **AND** the AssignedDate SHALL be **null**

### Requirement: Integration tests
Integration tests SHALL be added in `src/IntegrationTests/DataAccess/Handlers/StateCommandHandlerForUnassignTests.cs` extending `IntegratedTestBase`. Tests SHALL use Shouldly assertions and follow the existing `StateCommandHandlerFor*Tests` pattern.

#### Scenario: ShouldSaveWorkOrderWithNoAssigneeAndDraftStatus
- **GIVEN** a work order in Assigned status with Creator and Assignee persisted to the database
- **WHEN** the `AssignedToDraftCommand` is handled by `StateCommandHandler`
- **THEN** the reloaded work order SHALL have status Draft, null Assignee, null AssignedDate, and the original Creator

#### Scenario: ShouldSaveWorkOrderWithRemotedCommand
- **GIVEN** a work order in Assigned status persisted to the database
- **WHEN** the command is serialized/deserialized via `RemotableRequestTests.SimulateRemoteObject` and handled
- **THEN** the reloaded work order SHALL have status Draft and null Assignee

### Constraints
- The `AssignedToDraftCommand` SHALL be defined in `src/Core/Model/StateCommands/` (Core project)
- The command SHALL inherit from `StateCommandBase` and follow the same pattern as `AssignedToCancelledCommand`
- No new NuGet packages required
- No database schema changes required
- No new project references required
