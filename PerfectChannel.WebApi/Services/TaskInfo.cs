namespace PerfectChannel.WebApi.Services
{
    public enum Statuses
    {
        Pending,
        Completed
    }

    public class TaskInfo
    {
        public string Description { get; }

        public Statuses Status { get; private set; }

        public TaskInfo(string description)
        {
            Description = description;
            Status = Statuses.Pending;
        }

        public void ChangeStatus()
        {
            if (Status == Statuses.Pending) Status = Statuses.Completed;
            else Status = Statuses.Pending;
        }
    }
}