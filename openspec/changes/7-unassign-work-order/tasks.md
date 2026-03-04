## 1. Core Domain

- [ ] 1.1 Create `AssignedToDraftCommand` record in `src/Core/Model/StateCommands/` inheriting from `StateCommandBase`
- [ ] 1.2 Implement begin status (Assigned), end status (Draft), verb ("Unassign"), authorization (Creator only)
- [ ] 1.3 Override `Execute()` to clear `WorkOrder.Assignee` and `WorkOrder.AssignedDate` before calling `base.Execute()`
- [ ] 1.4 Register `AssignedToDraftCommand` in `StateCommandList.GetAllStateCommands()`

## 2. Unit Tests

- [ ] 2.1 Create `AssignedToDraftCommandTests` in `src/UnitTests/Core/Model/StateCommands/` extending `StateCommandBaseTests`
- [ ] 2.2 Test: `ShouldNotBeValidInWrongStatus` — command with Draft status work order is not valid
- [ ] 2.3 Test: `ShouldNotBeValidWithWrongEmployee` — command with non-Creator user is not valid
- [ ] 2.4 Test: `ShouldBeValid` — command with Assigned status and Creator user is valid
- [ ] 2.5 Test: `ShouldTransitionStateProperly` — execute transitions to Draft, clears Assignee and AssignedDate
- [ ] 2.6 Update `StateCommandListTests.ShouldReturnAllStateCommandsInCorrectOrder` to expect the new command count and type

## 3. Integration Tests

- [ ] 3.1 Create `StateCommandHandlerForUnassignTests` in `src/IntegrationTests/DataAccess/Handlers/`
- [ ] 3.2 Test: `ShouldSaveWorkOrderWithNoAssigneeAndDraftStatus` — persist unassign and verify Assignee is null, status is Draft
- [ ] 3.3 Test: `ShouldSaveWorkOrderWithRemotedCommand` — verify serialization/deserialization round-trip

## 4. Build Verification

- [ ] 4.1 Verify solution builds with `dotnet build src/ChurchBulletin.sln`
- [ ] 4.2 Verify unit tests pass with `dotnet test src/UnitTests/UnitTests.csproj`
