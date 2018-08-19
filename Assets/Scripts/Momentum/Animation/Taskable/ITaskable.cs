namespace Momentum
{
    public interface ITaskable
    {
        void AddTask(Task task);
        void RemoveTask(Task task);
        void StopAllTasks();
    }
}