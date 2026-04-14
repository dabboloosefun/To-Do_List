using System.ComponentModel;
using System.Runtime.CompilerServices;

class TaskService : ITaskService
{
    private readonly IRepository<TaskItem> _repository;
    private readonly IMyCollection<TaskItem> _tasks;

    public TaskService(IRepository<TaskItem> repository)
    {
        _repository = repository;
        IEnumerable<TaskItem> loaded = _repository.Load();
        _tasks = new MyArray<TaskItem>();

        foreach (var task in loaded)
        {
            _tasks.Add(task);
        }
    }

    public IEnumerable<TaskItem> GetAllTasks()
    {
        return ConvertToEnumerable(_tasks);
    }

    public IEnumerable<TaskItem> GetTasksByPriority(int priority)
    {
        return ConvertToEnumerable(_tasks.Filter(t => t.Priority == priority));
    }

    public IEnumerable<TaskItem> GetTasksByStatus(int status)
    {
        return ConvertToEnumerable(_tasks.Filter(t => t.Status == status));
    }

    public IEnumerable<TaskItem> GetTasksByDate(DateTime date)
    {
        return ConvertToEnumerable(_tasks.Filter(t => t.CreationDate.Date == date.Date));
    }

    public TaskItem? GetTaskById(int id)
    {
        return _tasks.FindBy(id, (task, key) => task.Id == key);
    }

    public void AddTask(string description, int priority, IMyIndexable<int>? assignedMembers)
    {
        int newId = _tasks.Count > 0 ? GetLastTaskId() + 1 : 1;
        TaskItem newTask = new TaskItem(assignedMembers)
        {
            Id = newId,
            Description = description,
            Status = -1,
            Priority = priority
        };
        _tasks.Add(newTask);
        SaveTasks();
    }

    public void RemoveTask(int id)
    {
        TaskItem? task = GetTaskById(id);
        if (task != null)
        {
            _tasks.Remove(task);
            SaveTasks();
        }
    }

    public void UpdateTask(TaskItem task)
    {
        var existing = GetTaskById(task.Id);
        if (existing != null)
        {
            _tasks.Remove(existing);
            _tasks.Add(task);
            SaveTasks();
        }
    }

    private int GetLastTaskId()
    {
        int lastId = 0;
        IMyIterator<TaskItem> iterator = _tasks.GetIterator();
        while (iterator.HasNext())
        {
            var t = iterator.Next();
            if (t.Id > lastId) lastId = t.Id;
        }
        return lastId;
    }

    private void SaveTasks()
    {
        List<TaskItem> list = new List<TaskItem>();
        IMyIterator<TaskItem> iterator = _tasks.GetIterator();
        while (iterator.HasNext())
        {
            list.Add(iterator.Next());
        }
        _repository.Save(list);
    }

    private IEnumerable<TaskItem> ConvertToEnumerable(IMyCollection<TaskItem> collection)
    {
        IMyIterator<TaskItem> iterator = collection.GetIterator();
        while (iterator.HasNext())
        {
            yield return iterator.Next();
        }
    }

    public void RemoveDependency(TaskItem task, int dependencyId)
    {
        if (GetTaskById(dependencyId) == null) return;
        if (task.DependantOn.FindBy<int>(dependencyId, (t, id) => t == id) != 0) task.DependantOn.Remove(dependencyId);
        SaveTasks();
    }

    public void AddDependency(TaskItem task, int dependencyId)
    {
        TaskItem? dependency = GetTaskById(dependencyId);
        if (dependency == null || IsCircular(task.Id, dependency)) return;

        task.DependantOn.Add(dependencyId);
        SaveTasks();
    }

    public bool IsCircular(int taskId, TaskItem dependency, HashSet<int>? visited = null)
    {
        bool isCircular = false;
        visited ??= new HashSet<int>{};

        if (!visited.Add(dependency.Id)) return false;

        if (dependency.DependantOn.FindBy<int>(taskId, (id, taskId) => id == taskId) != 0)
        {
            isCircular = true;
            return isCircular;
        }

        for (int i = 0; i < dependency.DependantOn.Count; i++)
        {
            TaskItem? next = GetTaskById(dependency.DependantOn[i]);
            if (next == null) continue;
            bool result = IsCircular(taskId, next, visited);
            if (result == true)
            {
                isCircular = true;
                break;
            }
        }

        return isCircular;
    }

    public bool CanStartTask(TaskItem item)
    {
        int amountDone = 0;
        IMyIterator<int> iterator = item.DependantOn.GetIterator();

        while (iterator.HasNext())
        {
            int taskId = iterator.Next();
            TaskItem? dependancy = GetTaskById(taskId);
            if (dependancy == null)
            {
                amountDone--;
                continue;
            }

            if (dependancy.Status == 1)
            {
                amountDone++;
                continue;
            }

            amountDone--;
        }

        return amountDone >= item.DependantOn.Count ? true : false;
    }

    public bool ToggleTaskStatus(int id, int status)
    {
        TaskItem? task = GetTaskById(id);

        if (task != null && status < 2)
        {
            if (CanStartTask(task))
            {
                task.Status = status;
                SaveTasks();
                return true;
            }
        }
        else if (task != null)
        {
            task.Status = -1;
            SaveTasks();
        }

        return false;
    }
}