## Context

The ChurchBulletin work order system implements a state machine with six transitions managed by `StateCommandBase`-derived records. Each command specifies a begin status, end status, and authorization rule (Creator or Assignee). The `StateCommandList` registers all commands, and the UI dynamically renders action buttons for valid commands based on the current work order status and logged-in user. Currently, the Complete status is terminal — no commands have `Complete` as their begin status.

## Goals / Non-Goals

**Goals:**
- Allow the Creator of a completed work order to reopen it, transitioning status from Complete to Assigned
- Follow the existing `StateCommandBase` pattern exactly — no architectural changes
- Ensure the Assignee is preserved when reopening (the same person who completed it remains assigned)
- Provide full test coverage at unit and integration levels

**Non-Goals:**
- Allowing the Assignee to reopen (only the Creator can reopen)
- Changing any existing state transitions
- Adding a new status value (reuses existing `Assigned` status)
- Modifying the UI (buttons are rendered dynamically from valid commands)

## Decisions

### Decision 1: Name the command `CompleteToAssignedCommand` with verb "Reopen"

**Rationale:** The class name follows the established `{FromStatus}To{ToStatus}Command` naming convention (e.g., `InProgressToAssignedCommand`, `AssignedToCancelledCommand`). The transition verb "Reopen" clearly communicates the action to the user and distinguishes it from the "Shelve" command which also transitions to Assigned but from InProgress.

**Alternatives considered:**
- "Reassign": Ambiguous — could imply changing the assignee, which is not the primary intent
- "Restart": Could imply going back to InProgress or Draft

### Decision 2: Only the Creator can reopen

**Rationale:** The Creator is the stakeholder who initiated the work. They are best positioned to determine whether completed work meets expectations. This is consistent with the existing pattern where the Creator controls the assignment lifecycle (Assign, Cancel) while the Assignee controls the execution lifecycle (Begin, Shelve, Complete).

### Decision 3: Preserve the existing Assignee on reopen

**Rationale:** When a work order is reopened, it returns to Assigned status with the same Assignee. The Creator can then cancel and re-assign to a different person if needed, using the existing Cancel and Assign flow. This keeps the Reopen command simple and composable with existing transitions.

## Risks / Trade-offs

- **[State complexity]** Adding a cycle in the state machine (Complete → Assigned → InProgress → Complete → ...) means work orders can be reopened indefinitely. → Mitigation: This is intentional. Business processes often require rework. No artificial limit is needed.
- **[CompletedDate preservation]** The `CompletedDate` set during completion will remain after reopening. On subsequent completion, it will be overwritten. → Mitigation: This is acceptable for the current requirements. If audit history of multiple completions is needed, that would be a separate feature.

## Open Questions

- Should there be a limit on how many times a work order can be reopened?
- Should reopening clear the `CompletedDate` field, or leave it as-is until the next completion?
