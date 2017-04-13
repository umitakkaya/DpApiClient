using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Data.Repositories
{
    public interface IRepository<T> : IDisposable
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        IEnumerable<T> GetByIds(params int[] ids);
        void Insert(T entity);
        void Delete(int id);
        void Update(T entity);
        void Save();

    }
}
