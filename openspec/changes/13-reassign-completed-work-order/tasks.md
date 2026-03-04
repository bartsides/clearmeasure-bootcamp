## 1. Core Domain

- [ ] 1.1 Create `CompleteToAssignedCommand` record in `src/Core/Model/StateCommands/` inheriting from `StateCommandBase`
- [ ] 1.2 Implement begin status (Complete), end status (Assigned), verb ("Reassign"), authorization (Creator only)
- [ ] 1.3 Override `Execute()` to clear `WorkOrder.CompletedDate` before calling `base.Execute()`
- [ ] 1.4 Register `CompleteToAssignedCommand` in `StateCommandList.GetAllStateCommands()`

## 2. Unit Tests

- [ ] 2.1 Create `CompleteToAssignedCommandTests` in `src/UnitTests/Core/Model/StateCommands/` extending `StateCommandBaseTests`
- [ ] 2.2 Test: `ShouldNotBeValidInWrongStatus` — command with Draft status work order is not valid
- [ ] 2.3 Test: `ShouldNotBeValidWithWrongEmployee` — command with non-Creator user is not valid
- [ ] 2.4 Test: `ShouldBeValid` — command with Complete status and Creator user is valid
- [ ] 2.5 Test: `ShouldTransitionStateProperly` — execute transitions to Assigned and clears CompletedDate
- [ ] 2.6 Update `StateCommandListTests.ShouldReturnAllStateCommandsInCorrectOrder` to expect the new command count and type

## 3. Integration Tests

- [ ] 3.1 Create `StateCommandHandlerForReassignTests` in `src/IntegrationTests/DataAccess/Handlers/`
- [ ] 3.2 Test: `ShouldSaveWorkOrderWithAssignedStatusAfterReassign` — persist reassign and verify status is Assigned, CompletedDate is null
- [ ] 3.3 Test: `ShouldSaveWorkOrderWithRemotedCommand` — verify serialization/deserialization round-trip

## 4. Build Verification

- [ ] 4.1 Verify solution builds with `dotnet build src/ChurchBulletin.sln`
- [ ] 4.2 Verify unit tests pass with `dotnet test src/UnitTests/UnitTests.csproj`
