namespace ClearMeasure.Bootcamp.Core.Model.StateCommands;

public record CompleteToAssignedCommand(WorkOrder WorkOrder, Employee CurrentUser)
    : StateCommandBase(WorkOrder, CurrentUser)
{
    public const string Name = "Reopen";

    public override WorkOrderStatus GetBeginStatus()
    {
        return WorkOrderStatus.Complete;
    }

    public override WorkOrderStatus GetEndStatus()
    {
        return WorkOrderStatus.Assigned;
    }

    protected override bool UserCanExecute(Employee currentUser)
    {
        return currentUser == WorkOrder.Creator;
    }

    public override string TransitionVerbPresentTense => Name;

    public override string TransitionVerbPastTense => "Reopened";
}
