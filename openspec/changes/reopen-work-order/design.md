## Context

The ChurchBulletin system uses a state machine pattern for work order lifecycle management. Each state transition is implemented as a record class extending `StateCommandBase`, which provides validation (`IsValid()`) and execution (`Execute()`) logic. The `StateCommandList` class manually instantiates all available commands and filters them by validity. The `StateCommandHandler` in DataAccess persists any state transition via MediatR. The Blazor UI renders valid commands as buttons automatically.

The current state machine has six transitions:
```
Draft --[Save]--> Draft
Draft --[Assign]--> Assigned
Assigned --[Begin]--> InProgress
Assigned --[Cancel]--> Cancelled
InProgress --[Complete]--> Complete
InProgress --[Shelve]--> Assigned
```

The Complete status is currently a terminal state with no outgoing transitions.

## Goals / Non-Goals

**Goals:**
- Add a Reopen transition from Complete back to InProgress, executable by the Assignee
- Clear `CompletedDate` when reopening so the work order can be completed again later with a new date
- Follow the exact same patterns as existing state commands for consistency
- Ensure the UI picks up the new command automatically through the existing rendering mechanism

**Non-Goals:**
- Adding audit log entries for the reopen action (not part of existing command pattern)
- Changing the Complete status or adding new statuses
- Adding a "Reopen" count or tracking how many times a work order has been reopened
- Requiring approval before reopening (single-step transition, same as other commands)
- Adding email notifications for the reopen event (no existing event/notification pattern for this)

## Decisions

### Decision 1: Name the command `CompleteToInProgressCommand` with verb "Reopen"

**Rationale:** Follows the existing naming convention where the class name describes the status transition (`FromStatusToToStatusCommand`) and the `TransitionVerbPresentTense` is a user-facing action name. "Reopen" clearly communicates the intent to both developers and users.

### Decision 2: Assignee is the authorized user (not Creator)

**Rationale:** The Assignee is the person responsible for the work. They are best positioned to know whether the work was truly complete. This mirrors `InProgressToCompleteCommand` where the Assignee is also the authorized user — the person who completed it can reopen it.

### Decision 3: Clear `CompletedDate` on reopen

**Rationale:** When a work order is reopened, it is no longer complete, so `CompletedDate` should be null. This mirrors the pattern in `AssignedToCancelledCommand` which clears `AssignedDate` and `Assignee` when cancelling. When the work order is completed again, `InProgressToCompleteCommand` will set a new `CompletedDate`.

### Decision 4: No new event type for the reopen transition

**Rationale:** Only `DraftToAssignedCommand` raises a `StateTransitionEvent` (specifically `WorkOrderAssignedToBotEvent` for bot assignees). The other commands do not raise events. Following this pattern, the reopen command will not raise an event unless a specific downstream need is identified later.

### Decision 5: No UI changes needed

**Rationale:** The Blazor UI (`WorkOrderManage.razor`) iterates over `ValidCommands` and renders a button for each. Adding the new command to `StateCommandList` is sufficient — the "Reopen" button will appear automatically when a Complete work order is viewed by its Assignee.

## Risks / Trade-offs

- **[State machine complexity]** Adding a cycle (Complete -> InProgress -> Complete) means work orders can be completed and reopened indefinitely. This is intentional — there is no business requirement to limit reopens.
- **[CompletedDate loss]** Clearing `CompletedDate` means the original completion timestamp is lost. If historical tracking is needed later, an audit trail feature would need to be added separately.
- **[Test count change]** `StateCommandListTests` will need updating to expect 7 commands instead of 6.

## Open Questions

- None. The implementation is straightforward and follows established patterns exactly.
