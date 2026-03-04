## Context

The ChurchBulletin system uses a state command pattern for all work order status transitions. Each transition is a record inheriting from `StateCommandBase` that defines a begin status, end status, authorization rule, and optional side effects. The `StateCommandHandler` (MediatR handler in DataAccess) persists the result via EF Core. The `StateCommandList` enumerates all available commands and filters to valid ones based on current state and user.

Existing transitions from Assigned status:
- **Assigned → InProgress** (`AssignedToInProgressCommand`): The assignee begins work
- **Assigned → Cancelled** (`AssignedToCancelledCommand`): The creator cancels, clearing assignee and assigned date

The `AssignedToCancelledCommand` already demonstrates the pattern of clearing `Assignee` and `AssignedDate` during execution. The new `AssignedToDraftCommand` follows the same pattern but transitions to Draft instead of Cancelled.

## Goals / Non-Goals

**Goals:**
- Allow the Creator to revert an Assigned work order back to Draft status
- Clear the Assignee and AssignedDate when unassigning
- Follow the existing state command pattern exactly (inherit from `StateCommandBase`, register in `StateCommandList`)
- Full test coverage: unit tests for validation/transition, integration tests for persistence

**Non-Goals:**
- Notification to the former assignee (can be added later via a state transition event)
- Audit trail or history log of the unassignment
- UI changes beyond what the existing dynamic button rendering provides
- Supporting Unassign from any status other than Assigned

## Decisions

### Decision 1: Name the command `AssignedToDraftCommand` with verb "Unassign"

**Rationale:** This follows the existing naming convention where command class names describe the status transition (`DraftToAssignedCommand`, `AssignedToInProgressCommand`, `AssignedToCancelledCommand`) and the `Name`/`TransitionVerbPresentTense` is the user-facing action verb. "Unassign" clearly communicates the intent.

**Alternatives considered:**
- `RevertToDesignCommand` / "Revert": Too generic, doesn't communicate what's being reverted
- `UnassignWorkOrderCommand`: Doesn't follow the `{BeginStatus}To{EndStatus}Command` naming pattern

### Decision 2: Only the Creator can unassign

**Rationale:** This matches the authorization rule of `DraftToAssignedCommand` (only Creator can assign) and `AssignedToCancelledCommand` (only Creator can cancel). The Creator owns the work order lifecycle at this stage.

### Decision 3: Clear Assignee and AssignedDate on unassign

**Rationale:** Returning to Draft means the work order is unassigned. The `AssignedToCancelledCommand` already sets both to null when cancelling. The same cleanup applies when unassigning — the work order should be indistinguishable from a freshly created draft (except it retains its Number and other metadata).

## Risks / Trade-offs

- **[State machine complexity]** Adding another transition increases the number of valid paths. → Mitigation: The transition is simple (Assigned → Draft) with no side effects beyond clearing assignment fields. Well-tested via unit and integration tests.
- **[UI button ordering]** The Unassign button will appear alongside Cancel and Begin for Assigned work orders when the user is the Creator. → Mitigation: The existing UI renders all valid commands dynamically. Button ordering follows `StateCommandList` registration order.

## Open Questions

- Should the former assignee receive a notification when unassigned? (Out of scope for this issue — can be added via `IStateTransitionEvent` later)
