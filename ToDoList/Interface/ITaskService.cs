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

    bool CanStartTask(TaskItem task);
    void RemoveDependency(TaskItem task, int dependencyId);
    void AddDependency(TaskItem task, int dependencyId);
    bool IsCircular(int taskId, TaskItem next, HashSet<int>? visited = null);

    bool ToggleTaskStatus(int id, int status);
}