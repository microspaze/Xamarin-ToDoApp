using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoApp.Repositories
{
    public interface IRepository<T>
    {
        T Get();

        IQueryable<T> GetQuery(string userId);

        Task<IEnumerable<T>> GetAllAsync(string userId);

        IQuery GetAll(string userId);

        IQuery GetAllContains(string userId, string field, object value);

        IQuery GetAllContains(string userId, string field1, object value1, string field2, object value2);

        IQuery GetAllContains(string userId, string field1, object value1, string field2, object value2, string field3, object value3);
        
        Task<bool> Update(T model);

        Task<bool> Add(T model);

        Task<bool> Delete(T model);
    }
}
