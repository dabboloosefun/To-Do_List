public interface ITaskService {
    IEnumerable<TaskItem> GetAllTasks();
    void AddTask(string description, int priority);
    void RemoveTask(int id);
    void ToggleTaskStatus(int id, int status);
}