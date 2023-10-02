

using Data.Models;

namespace Data.DataAccess.Data.Interface.IData
{
    public interface ICategoryRepository:IRepository<Category> 
    {
        void Update(Category category);
     
    }
}
