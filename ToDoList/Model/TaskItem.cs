using System.Text.Json.Serialization;

public class TaskItem : IHasId
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public int Status { get; set; }

    public int Priority { get; set; }

    [JsonIgnore]
    public IMyCollection<int> AssignedMembers { get; set; }
    [JsonIgnore]
    public IMyCollection<int> DependantOn { get; set; }


    [JsonPropertyName("AssignedMembers")]
    public int[] AssignedMembersSurrogate
    {
        get => AssignedMembers.ToArray();
        set
        {
            AssignedMembers = new MyArray<int>();
            foreach (var id in value) AssignedMembers.Add(id);
        }
    }

    [JsonPropertyName("DependantOn")]
    public int[] DependantOnSurrogate
    {
        get => DependantOn.ToArray();
        set
        {
            DependantOn = new MyArray<int>();
            foreach (var id in value) DependantOn.Add(id);
        }
    }

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