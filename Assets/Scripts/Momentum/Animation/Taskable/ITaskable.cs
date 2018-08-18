namespace Momentum
{
    public interface ITaskable
    {
        void AddTask(Task task);
        void StopAllTasks();
    }
}