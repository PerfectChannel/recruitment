using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PerfectChannel.WebApi.Services
{
    public class TodoListService : ITodoListService
    {

        private readonly Dictionary<string, TaskInfo> Tasks;

        public TodoListService()
        {
            Tasks = new Dictionary<string, TaskInfo>();
        }

        /// <summary>
        /// Returns a json with 2 lists (pending and completed)
        /// </summary>
        public string GetList()
        {
            var pendingList = Tasks.Where(q => q.Value.Status == Statuses.Pending).Select(q => new KeyValuePair<string, string>(q.Key, q.Value.Description));
            var completedList = Tasks.Where(q => q.Value.Status == Statuses.Completed).Select(q => new KeyValuePair<string, string>(q.Key, q.Value.Description));
            List<dynamic> list = new List<dynamic>
            {
                pendingList,
                completedList
            };
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Add new tasks to the list, using the Task Description.
        /// </summary>
        /// <returns>true if successfully added to the list, false in other case</returns>
        public bool AddTask(string taskDescription)
        {
            // 5 Retries
            var attempts = 5;
            while (attempts > 0)
            {
                Guid g = Guid.NewGuid();
                if (Tasks.TryAdd(g.ToString(), new TaskInfo(taskDescription)))
                {
                    return true;
                }
                attempts--;
            }
            return false;
        }

        /// <summary>
        /// Changes the status of a certain task, using the taskID
        /// </summary>
        /// <returns>true if successfully, false in other case (probably not id found)</returns>
        public bool ChangeStatus(string taskID)
        {
            if (string.IsNullOrEmpty(taskID) || !Tasks.ContainsKey(taskID))
            {
                return false;
            }
            Tasks[taskID].ChangeStatus();
            return true;
        }
    }
}