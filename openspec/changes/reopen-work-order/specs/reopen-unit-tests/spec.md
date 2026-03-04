## ADDED Requirements

### Requirement: Unit tests for CompleteToAssignedCommand
Unit tests SHALL be added in `src/UnitTests/Core/Model/StateCommands/CompleteToAssignedCommandTests.cs`. The test class SHALL extend `StateCommandBaseTests` and follow the same pattern as `InProgressToCompleteCommandTests`.

#### Scenario: ShouldNotBeValidInWrongStatus
- **GIVEN** a work order with status Draft (wrong status)
- **AND** the current user is the Creator
- **WHEN** `IsValid()` is called on `CompleteToAssignedCommand`
- **THEN** it SHALL return false

#### Scenario: ShouldNotBeValidWithWrongEmployee
- **GIVEN** a work order with status Complete
- **AND** the current user is NOT the Creator
- **WHEN** `IsValid()` is called on `CompleteToAssignedCommand`
- **THEN** it SHALL return false

#### Scenario: ShouldBeValid
- **GIVEN** a work order with status Complete
- **AND** the current user is the Creator
- **WHEN** `IsValid()` is called on `CompleteToAssignedCommand`
- **THEN** it SHALL return true

#### Scenario: ShouldTransitionStateProperly
- **GIVEN** a work order with status Complete and the current user as Creator
- **WHEN** `Execute()` is called on `CompleteToAssignedCommand`
- **THEN** the work order status SHALL be Assigned

### Requirement: StateCommandList tests updated for 7 commands
The `StateCommandListTests.ShouldReturnAllStateCommandsInCorrectOrder` test SHALL be updated to expect 7 commands and verify that the 7th command (index 6) is an instance of `CompleteToAssignedCommand`.

### Constraints
- Test class SHALL extend `StateCommandBaseTests`
- Test class SHALL use `[TestFixture]` attribute
- Tests SHALL use NUnit `Assert.That` assertions
- Test file SHALL be `src/UnitTests/Core/Model/StateCommands/CompleteToAssignedCommandTests.cs`
