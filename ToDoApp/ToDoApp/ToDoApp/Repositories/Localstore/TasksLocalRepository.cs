using Newtonsoft.Json;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Models;
using Xamarin.Essentials;

namespace ToDoApp.Repositories.Localstore
{
    public class TasksLocalRepository : IRepository<TaskModel>
    {
        private static string _taskKey = string.Empty;
        private static IList<TaskModel> _currentTasks = null;

        public Task<bool> Add(TaskModel model)
        {
            var result = false;
            var taskModels = GetTaskModels(model.UserId);
            if (taskModels != null)
            {
                _currentTasks.Add(model);

                result = this.SaveTaskModels();
            }

            return Task.FromResult(result);
        }

        public Task<bool> Delete(TaskModel model)
        {
            var result = false;
            if (_currentTasks != null)
            {
                var existed = _currentTasks.FirstOrDefault(x => x.Id == model.Id);
                if (existed != null)
                {
                    if (_currentTasks.Remove(existed))
                    {
                        result = this.SaveTaskModels();
                    }
                }
                else
                {
                    result = true;
                }
            }

            return Task.FromResult(result);
        }

        public TaskModel Get()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TaskModel> GetQuery(string userId)
        {
            IQueryable<TaskModel> query = null;
            var taskModels = GetTaskModels(userId);
            if (taskModels != null)
            {
                query = taskModels.AsQueryable();
            }

            return query;
        }

        public Task<IEnumerable<TaskModel>> GetAllAsync(string userId)
        {
            var taskModels = GetTaskModels(userId);

            return Task.FromResult(taskModels.AsEnumerable());
        }

        public IQuery GetAll(string userId)
        {
            throw new NotImplementedException();
        }

        public IQuery GetAllContains(string userId, string field, object value)
        {
            throw new NotImplementedException();
        }

        public IQuery GetAllContains(string userId, string field1, object value1, string field2, object value2)
        {
            throw new NotImplementedException();
        }

        public IQuery GetAllContains(string userId, string field1, object value1, string field2, object value2, string field3, object value3)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(TaskModel model)
        {
            var result = false;
            if (_currentTasks != null)
            {
                var existed = _currentTasks.FirstOrDefault(x => x.Id == model.Id);
                if (existed != null)
                {
                    existed.Update(model);
                    result = this.SaveTaskModels();
                }
            }

            return Task.FromResult(result);
        }

        private IList<TaskModel> GetTaskModels(string userId)
        {
            IList<TaskModel> taskModels = new List<TaskModel>();
            if (_currentTasks == null)
            {
                var tasksKey = GetTasksKey(userId);
                var tasksJson = Preferences.Get(tasksKey, string.Empty);
                if (!string.IsNullOrEmpty(tasksJson))
                {
                    taskModels = JsonConvert.DeserializeObject<List<TaskModel>>(tasksJson);
                }

                _currentTasks = taskModels;
                _taskKey = tasksKey;
            }
            else
            {
                taskModels = _currentTasks;
            }

            return taskModels;
        }

        private bool SaveTaskModels()
        {
            var result = false;
            if (!string.IsNullOrEmpty(_taskKey))
            {
                Preferences.Set(_taskKey, JsonConvert.SerializeObject(_currentTasks));
                result = true;
            }

            return result;
        }

        private string GetTasksKey(string userId)
        {
            return "task-" + userId;
        }
    }
}
