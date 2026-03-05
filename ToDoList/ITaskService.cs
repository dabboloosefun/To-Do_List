public interface ITaskService {
    IEnumerable<TaskItem> GetAllTasks();
    void AddTask(string description);
    void RemoveTask(int id);
    void ToggleTaskStatus(int id, int status);
}