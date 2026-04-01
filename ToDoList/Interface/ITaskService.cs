public interface ITaskService
{
    IEnumerable<TaskItem> GetAllTasks();
    IEnumerable<TaskItem> GetTasksByPriority(int priority);
    IEnumerable<TaskItem> GetTasksByStatus(int status);
    IEnumerable<TaskItem> GetTasksByDate(DateTime date);
    void AddTask(string description, int priority, IMyCollection<int>? assignedMembers);
    void RemoveTask(int id);
    TaskItem? GetTaskById(int id);
    void UpdateTask(TaskItem task);
    bool DependanciesDone(TaskItem task);
    bool ToggleTaskStatus(int id, int status);
}