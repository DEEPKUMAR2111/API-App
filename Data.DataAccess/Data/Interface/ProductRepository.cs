
using Data.DataAccess.Data.Interface.IData;
using Data.Models;

namespace Data.DataAccess.Data.Interface
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product data)
        {
            var dataFromDb = _db.Products.FirstOrDefault(u => u.ProductId == data.ProductId);
            if (dataFromDb != null)
            {
                dataFromDb.Name = data.Name;
                dataFromDb.Discription = data.Discription;
                dataFromDb.Price = data.Price;
                dataFromDb.Category = data.Category;
                dataFromDb.CategoryId = data.CategoryId;
                if (dataFromDb.ImgUrl != null)
                {
                    dataFromDb.ImgUrl = data.ImgUrl;
                }
            }

        }
    }
}
