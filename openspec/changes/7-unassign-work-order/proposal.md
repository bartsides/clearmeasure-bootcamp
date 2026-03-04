## Why

The ChurchBulletin work order state machine currently has no way to revert an Assigned work order back to Draft. If a work order is assigned by mistake, or the assignee is no longer available before work begins, the only option is to Cancel the work order and create a new one. An Unassign command allows the Creator to return the work order to Draft status, clearing the assignee and making it available for reassignment — without losing the work order history.

## What Changes

- New `AssignedToDraftCommand` state command that transitions a work order from Assigned to Draft
- The command clears the Assignee and AssignedDate fields
- Only the Creator of the work order can execute the Unassign command
- The command is registered in `StateCommandList` alongside existing state commands
- Unit tests covering validation and state transition behavior
- Integration tests covering persistence through `StateCommandHandler`
- The UI automatically surfaces the Unassign button when the command is valid (existing `StateCommandList.GetValidStateCommands` pattern)

## Capabilities

### New Capabilities
- `unassign-state-command`: A new state command (`AssignedToDraftCommand`) that transitions a work order from Assigned back to Draft, clearing the Assignee and AssignedDate. Only the Creator can execute it.

### Modified Capabilities
- `state-command-list`: The `StateCommandList.GetAllStateCommands()` method is updated to include the new `AssignedToDraftCommand`

## Impact

- **Core project**: New `AssignedToDraftCommand` record in `src/Core/Model/StateCommands/`
- **StateCommandList**: One additional command registered in `GetAllStateCommands()`
- **Database**: No schema changes — reuses existing WorkOrder table columns (Status, Assignee, AssignedDate)
- **UI**: No direct UI changes needed — the existing `WorkOrderManage` page dynamically renders buttons for all valid state commands via `StateCommandList.GetValidStateCommands()`
- **Tests**: New unit test class and integration test class following established patterns
