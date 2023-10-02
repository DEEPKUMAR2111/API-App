

using Data.DataAccess.Data.Interface.IData;

namespace Data.DataAccess.Data.Interface
{
    public class UnitOfwork : IUnitOfWork
    {
        private ApplicationDbContext  _db;
        public UnitOfwork(ApplicationDbContext db)
        {
            _db = db;
            CategoryRepository=new CategoryRepository(db);
            ProductRepository= new ProductRepository(db);
            ApplicationUserRepository = new ApplicationUserRepository(db);
            CartRepository = new CartRepository(db);
            OrderHeaderRepository = new OrderHeaderRepository(db);
            OrderDetailRepository = new OrderDetailRepository(db);
        }
        public ICategoryRepository CategoryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }

        public ICartRepository CartRepository { get; private set; }

        public IOrderDetailRepository OrderDetailRepository { get; private set; }

        public IOrderHeaderRepository OrderHeaderRepository { get; private set; }

        public IApplicationUserRepository ApplicationUserRepository { get; private set; }

        public void Save()
        {
          _db.SaveChanges();
        }
    }
}
