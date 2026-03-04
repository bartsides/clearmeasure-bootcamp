## 1. Core Domain: State Command

- [ ] 1.1 Create `src/Core/Model/StateCommands/CompleteToInProgressCommand.cs` extending `StateCommandBase` with begin status `Complete`, end status `InProgress`, verb "Reopen", and `UserCanExecute` checking `currentUser == WorkOrder.Assignee`
- [ ] 1.2 Override `Execute()` to clear `WorkOrder.CompletedDate` before calling `base.Execute(context)`
- [ ] 1.3 Add `CompleteToInProgressCommand` to `StateCommandList.GetAllStateCommands()` in `src/Core/Services/Impl/StateCommandList.cs`

## 2. Unit Tests

- [ ] 2.1 Create `src/UnitTests/Core/Model/StateCommands/CompleteToInProgressCommandTests.cs` extending `StateCommandBaseTests` with tests: `ShouldNotBeValidInWrongStatus`, `ShouldNotBeValidWithWrongEmployee`, `ShouldBeValid`, `ShouldTransitionStateProperly`
- [ ] 2.2 Add test `ShouldClearCompletedDateOnExecute` verifying `CompletedDate` is set to `null` after execution
- [ ] 2.3 Update `StateCommandListTests` to expect 7 commands (was 6) and verify the new command is included

## 3. Integration Tests

- [ ] 3.1 Add integration test verifying a Complete work order can be reopened and persisted with status InProgress and null CompletedDate

## 4. Verification

- [ ] 4.1 Run full build (`PrivateBuild.ps1`) and verify all tests pass
