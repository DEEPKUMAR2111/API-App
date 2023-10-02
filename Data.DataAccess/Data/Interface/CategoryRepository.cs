
using Data.DataAccess.Data.Interface.IData;
using Data.Models;

namespace Data.DataAccess.Data.Interface
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
          _db.Categories.Update(category);
        }
    }
}
