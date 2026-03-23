public class TaskItem
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public int Status { get; set; }

    public int Priority { get; set; }
    public List<int> AssignedMembers { get; set; }
    public List<int> DependantOn { get; set; }
    public DateTime CreationDate { get; set; }

    public TaskItem(List<int>? assignedMembers, List<int>? dependantOn)
    {
        CreationDate = DateTime.Now;
        AssignedMembers = assignedMembers == null ? new List<int>() : assignedMembers;
        DependantOn = dependantOn == null ? new List<int>() : dependantOn;
    }
}