## Why

The ChurchBulletin work order state machine currently allows the Assignee to reopen a completed work order (Complete → InProgress via `CompleteToInProgressCommand`), but there is no way for the Creator to reassign a completed work order back to the Assigned state. If the Creator needs to reassign the work to a different person or simply restart the assignment process, they have no command available. A Reassign command allows the Creator to move the work order from Complete back to Assigned, keeping the existing assignee and making the work order ready for work again.

## What Changes

- New `CompleteToAssignedCommand` state command that transitions a work order from Complete to Assigned
- Only the Creator of the work order can execute the Reassign command
- The command clears the CompletedDate field (since the work order is no longer complete)
- The command is registered in `StateCommandList` alongside existing state commands
- Unit tests covering validation and state transition behavior
- Integration tests covering persistence through `StateCommandHandler`
- The UI automatically surfaces the Reassign button when the command is valid (existing `StateCommandList.GetValidStateCommands` pattern)

## Capabilities

### New Capabilities
- `reassign-state-command`: A new state command (`CompleteToAssignedCommand`) that transitions a work order from Complete back to Assigned. Only the Creator can execute it. The CompletedDate is cleared on execution.

### Modified Capabilities
- `state-command-list`: The `StateCommandList.GetAllStateCommands()` method is updated to include the new `CompleteToAssignedCommand`

## Impact

- **Core project**: New `CompleteToAssignedCommand` record in `src/Core/Model/StateCommands/`
- **StateCommandList**: One additional command registered in `GetAllStateCommands()`
- **Database**: No schema changes — reuses existing WorkOrder table columns (Status, CompletedDate)
- **UI**: No direct UI changes needed — the existing `WorkOrderManage` page dynamically renders buttons for all valid state commands via `StateCommandList.GetValidStateCommands()`
- **Tests**: New unit test class and integration test class following established patterns
