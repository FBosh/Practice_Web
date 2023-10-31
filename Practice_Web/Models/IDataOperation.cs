using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Practice_Web.Models
{
    public interface IDataOperation<T> where T : class, new()
    {
        int Create(T item);

        IEnumerable<T> GetAll();

        T Get(int id);

        int Update(T item);

        int Delete(T item);
    }
}
