public class TaskItem
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public int Status { get; set; }

    public int Priority { get; set; }
    public IMyCollection<int> AssignedMembers { get; set; }
    public IMyCollection<int> DependantOn { get; set; }
    public DateTime CreationDate { get; set; }

    public TaskItem()
    {
        CreationDate = DateTime.Now;
        AssignedMembers = new MyArray<int>();
        DependantOn = new MyArray<int>();
    }

    public TaskItem(IMyCollection<int>? assignedMembers, IMyCollection<int>? dependantOn = null)
    {
        CreationDate = DateTime.Now;
        AssignedMembers = assignedMembers == null ? new MyArray<int>() : assignedMembers;
        DependantOn = dependantOn == null ? new MyArray<int>() : dependantOn;
    }
}