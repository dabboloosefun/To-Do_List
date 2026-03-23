public interface ITaskService {
    IEnumerable<TaskItem> GetAllTasks();
    void AddTask(string description, int priority, List<int>? assignedMembers, List<int>? dependantOn);
    void RemoveTask(int id);
    TaskItem? GetTaskById(int id);
    void UpdateTask(TaskItem task);
    bool DependanciesDone(TaskItem task); 
    bool ToggleTaskStatus(int id, int status);
}