## Why

When a work order is completed, the Creator may discover that additional work is needed — for example, the task was only partially done, requirements changed, or the completed work was unsatisfactory. Currently, once a work order reaches the Complete status, it is a terminal state with no available actions. The Creator must create an entirely new work order, losing the history and context of the original. Adding a "Reopen" transition from Complete back to Assigned allows the Creator to efficiently reassign the same work order while preserving its full audit trail.

GitHub Issue: #5

## What Changes

- New state command `CompleteToAssignedCommand` implementing the Complete → Assigned transition, authorized for the Creator only
- Registration of the new command in `StateCommandList.GetAllStateCommands()`
- Unit tests validating the state transition logic and authorization rules
- Integration tests validating persistence of the reopened state through the `StateCommandHandler`
- The UI automatically displays a "Reopen" button for the Creator on completed work orders (no UI code changes needed — the dynamic button rendering picks up valid commands)

## Capabilities

### New Capabilities
- `reopen-state-command`: State command enabling the Creator to reopen a completed work order, transitioning it from Complete to Assigned status

### Modified Capabilities
<!-- No existing spec-level behavior changes. The new command is additive and follows the established state machine pattern. -->

## Impact

- **State machine**: Adds a new transition arc: `Complete --[Reopen]--> Assigned` (Creator only)
- **No schema changes**: No database migration required — uses existing `Status` column and `WorkOrderStatus` values
- **No new dependencies**: Uses existing `StateCommandBase` infrastructure
- **UI**: The "Reopen" button appears automatically via the existing dynamic command button rendering in `WorkOrderManage.razor`
- **MCP Server**: The `execute-work-order-command` MCP tool will automatically support "Reopen" since it resolves commands by name from `StateCommandList`
