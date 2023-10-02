

namespace Data.DataAccess.Data.Interface.IData
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        ICartRepository CartRepository { get; }

        IOrderDetailRepository OrderDetailRepository { get; }
        IOrderHeaderRepository OrderHeaderRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }
        void Save();
    }
}
