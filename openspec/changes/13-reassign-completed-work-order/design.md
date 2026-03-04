## Context

The ChurchBulletin system uses a state command pattern for all work order status transitions. Each transition is a record inheriting from `StateCommandBase` that defines a begin status, end status, authorization rule, and optional side effects. The `StateCommandHandler` (MediatR handler in DataAccess) persists the result via EF Core. The `StateCommandList` enumerates all available commands and filters to valid ones based on current state and user.

Existing transitions from Complete status:
- **Complete → InProgress** (`CompleteToInProgressCommand`): The Assignee reopens the work order to continue working on it

The new `CompleteToAssignedCommand` gives the Creator the ability to reassign the completed work order, moving it back to Assigned status. This follows the same pattern as `CompleteToInProgressCommand` but is authorized for the Creator instead of the Assignee.

## Goals / Non-Goals

**Goals:**
- Allow the Creator to move a completed work order back to Assigned status
- Clear the CompletedDate when reassigning (since the work order is no longer complete)
- Follow the existing state command pattern exactly (inherit from `StateCommandBase`, register in `StateCommandList`)
- Full test coverage: unit tests for validation/transition, integration tests for persistence

**Non-Goals:**
- Changing the Assignee as part of the reassign (the existing Assignee is preserved)
- Notification to the assignee about the reassignment
- Audit trail or history log of the reassignment
- UI changes beyond what the existing dynamic button rendering provides
- Supporting Reassign from any status other than Complete

## Decisions

### Decision 1: Name the command `CompleteToAssignedCommand` with verb "Reassign"

**Rationale:** This follows the existing naming convention where command class names describe the status transition (`CompleteToInProgressCommand`, `AssignedToDraftCommand`) and the `Name`/`TransitionVerbPresentTense` is the user-facing action verb. "Reassign" clearly communicates the intent and is distinct from the existing "Reopen" verb used by `CompleteToInProgressCommand`.

**Alternatives considered:**
- "Reopen": Already used by `CompleteToInProgressCommand` — would cause `GetMatchingCommand().Single()` to fail when the user is both Creator and Assignee
- "Send Back": Ambiguous — doesn't clearly indicate the target status

### Decision 2: Only the Creator can reassign

**Rationale:** The Creator owns the work order lifecycle for assignment decisions. The Assignee already has the "Reopen" command to move back to InProgress. Giving the Creator a separate command to move to Assigned provides a complementary workflow.

### Decision 3: Clear CompletedDate on reassign

**Rationale:** Moving from Complete to Assigned means the work order is no longer complete. The `CompleteToInProgressCommand` already clears `CompletedDate` when reopening. The same cleanup applies when reassigning.

## Risks / Trade-offs

- **[Verb uniqueness]** The "Reassign" verb must be unique across all commands to avoid `GetMatchingCommand().Single()` failures. → Mitigation: No other command uses "Reassign" as its verb.
- **[State machine complexity]** Adding another transition from Complete increases the number of valid paths. → Mitigation: The transition is simple (Complete → Assigned) with clear authorization (Creator only). Well-tested via unit and integration tests.

## Open Questions

- Should the Creator be able to change the Assignee as part of the reassign? (Out of scope — can be a separate feature)
