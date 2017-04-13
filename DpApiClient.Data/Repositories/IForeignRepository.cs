using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DpApiClient.Data.Repositories
{
    public interface IForeignRepository<T> : IDisposable
    {
        IEnumerable<T> GetAll();
        T GetById(string id);
        IEnumerable<T> GetByIds(params string[] ids);
        void InsertOrUpdate(T entity);
        void Insert(T entity);
        void Delete(string id);
        void Update(T entity);
        void Save();
    }
}
