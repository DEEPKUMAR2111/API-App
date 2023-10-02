

using Data.Models;

namespace Data.DataAccess.Data.Interface.IData
{ 
    public interface IProductRepository: IRepository<Product>
    {
        void Update(Product product);
    }
}
