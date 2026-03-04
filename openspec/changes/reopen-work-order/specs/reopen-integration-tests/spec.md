## ADDED Requirements

### Requirement: Integration test for CompleteToAssignedCommand handler
An integration test SHALL be added in `src/IntegrationTests/DataAccess/Handlers/StateCommandHandlerForReopenTests.cs`. The test class SHALL extend `IntegratedTestBase` and follow the same pattern as `StateCommandHandlerForCompleteTests`.

#### Scenario: ShouldReopenWorkOrder
- **GIVEN** a work order persisted in the database with status Complete
- **AND** the work order has a Creator and an Assignee
- **WHEN** a `CompleteToAssignedCommand` is created with the Creator as current user
- **AND** the command is serialized/deserialized via `RemotableRequestTests.SimulateRemoteObject()`
- **AND** the command is handled by `StateCommandHandler`
- **THEN** the work order loaded from the database SHALL have status Assigned
- **AND** the Creator SHALL be preserved
- **AND** the Assignee SHALL be preserved

### Constraints
- Test class SHALL extend `IntegratedTestBase`
- Test SHALL call `new DatabaseTests().Clean()` before persisting test data
- Test SHALL use `Faker<T>()` for generating test entities
- Test SHALL use `RemotableRequestTests.SimulateRemoteObject()` to verify JSON serialization round-trip
- Test SHALL use Shouldly assertions (`ShouldBe`)
- Test file SHALL be `src/IntegrationTests/DataAccess/Handlers/StateCommandHandlerForReopenTests.cs`
