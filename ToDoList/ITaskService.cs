public interface ITaskService {
    IEnumerable<TaskItem> GetAllTasks();
    void AddTask(string description, int priority, List<int>? assignedMembers);
    void RemoveTask(int id);
    TaskItem? GetTaskById(int id);
    void UpdateTask(TaskItem task);
    void ToggleTaskStatus(int id, int status);
}