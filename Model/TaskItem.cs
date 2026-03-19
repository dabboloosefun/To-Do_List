public class TaskItem
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public int Status { get; set; }

    public int Priority { get; set; }
    public DateTime CreationDate { get; set; }

    public TaskItem()
    {
        CreationDate = DateTime.Now;
    }
}