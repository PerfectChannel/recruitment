namespace PerfectChannel.WebApi.Services
{
    public interface ITodoListService
    {
        /// <summary>
        /// Returns a json with 2 lists (pending and completed)
        /// </summary>
        string GetList();

        /// <summary>
        /// Add new tasks to the list, using the Task Description.
        /// </summary>
        /// <returns>true if successfully added to the list, false in other case</returns>
        bool AddTask(string taskDescription);

        /// <summary>
        /// Changes the status of a certain task, using the taskID
        /// </summary>
        /// <returns>true if successfully, false in other case (probably not id found)</returns>
        bool ChangeStatus(string taskID);
    }
}