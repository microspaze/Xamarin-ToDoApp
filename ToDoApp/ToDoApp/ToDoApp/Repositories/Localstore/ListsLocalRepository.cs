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
    public class ListsLocalRepository : IRepository<ListModel>
    {
        private static string _listKey = string.Empty;
        private static IList<ListModel> _currentList = null;

        public Task<bool> Add(ListModel model)
        {
            var result = false;
            if (_currentList != null)
            {
                _currentList.Add(model);

                result = this.SaveListModels();
            }

            return Task.FromResult(result);
        }

        public Task<bool> Delete(ListModel model)
        {
            var result = false;
            if (_currentList != null)
            {
                var existed = _currentList.FirstOrDefault(x => x.Id == model.Id);
                if (existed != null)
                {
                    if (_currentList.Remove(existed))
                    {
                        result = this.SaveListModels();
                    }
                }
                else
                {
                    result = true;
                }
            }

            return Task.FromResult(result);
        }

        public ListModel Get()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ListModel> GetQuery(string userId)
        {
            IQueryable<ListModel> query = null;
            var listModels = GetListModels(userId);
            if (listModels != null)
            {
                query = listModels.AsQueryable();
            }

            return query;
        }

        public Task<IEnumerable<ListModel>> GetAllAsync(string userId)
        {
            var listModels = GetListModels(userId);

            return Task.FromResult(listModels.AsEnumerable());
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

        public Task<bool> Update(ListModel model)
        {
            var result = false;
            if (_currentList != null)
            {
                var existed = _currentList.FirstOrDefault(x => x.Id == model.Id);
                if (existed != null)
                {
                    existed.Color = model.Color;
                    existed.Name = model.Name;
                    result = this.SaveListModels();
                }
            }

            return Task.FromResult(result);
        }

        private IList<ListModel> GetListModels(string userId)
        {
            IList<ListModel> listModels = new List<ListModel>();
            if (_currentList == null)
            {
                var listKey = GetListKey(userId);
                var listJson = Preferences.Get(listKey, string.Empty);
                if (!string.IsNullOrEmpty(listJson))
                {
                    listModels = JsonConvert.DeserializeObject<List<ListModel>>(listJson);
                    _currentList = listModels;
                    _listKey = listKey;
                }
            }
            else
            {
                listModels = _currentList;
            }

            return listModels;
        }

        private bool SaveListModels()
        {
            var result = false;
            if (!string.IsNullOrEmpty(_listKey))
            {
                Preferences.Set(_listKey, JsonConvert.SerializeObject(_currentList));
                result = true;
            }

            return result;
        }

        private string GetListKey(string userId)
        {
            return "list-" + userId;
        }
    }
}
