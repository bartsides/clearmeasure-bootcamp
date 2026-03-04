## Why

Work orders that have been marked Complete cannot currently be revisited. In practice, completed work may need to be reopened — the original issue was not fully resolved, new information surfaced, or the work was marked complete prematurely. Without a reopen path, the only option is to create an entirely new work order, losing the history, context, and audit trail of the original. Adding a Reopen transition allows the Assignee to move a Complete work order back to In Progress, maintaining traceability and reducing administrative overhead.

GitHub Issue: #6

## What Changes

- New `CompleteToInProgressCommand` state command in `src/Core/Model/StateCommands/` implementing the Complete-to-InProgress transition
- The command clears `CompletedDate` when reopening
- `StateCommandList` updated to include the new command in the available commands list
- Unit tests for the new command following the existing `StateCommandBaseTests` pattern
- Integration tests covering persistence of the reopened work order
- The Blazor UI automatically picks up the new command via the existing `ValidCommands` rendering loop — no UI changes required

## Capabilities

### New Capabilities
- `reopen-work-order`: State command enabling the transition from Complete to InProgress status when executed by the Assignee

### Modified Capabilities
<!-- No existing spec-level behavior changes beyond adding the new command to StateCommandList -->

## Impact

- **Core project**: New `CompleteToInProgressCommand.cs` file added to `src/Core/Model/StateCommands/`
- **Core project**: `StateCommandList.GetAllStateCommands()` updated to include the new command
- **UnitTests project**: New `CompleteToInProgressCommandTests.cs` and updated `StateCommandListTests` to account for 7 commands
- **IntegrationTests project**: New integration test for persisting a reopened work order
- **Database**: No schema changes — reuses existing `Status` and `CompletedDate` columns
- **UI**: No template changes — the existing `@foreach (var command in ValidCommands)` loop will automatically render a "Reopen" button when the command is valid
- **DataAccess**: No handler changes — `StateCommandHandler` already handles any `StateCommandBase`-derived command generically
