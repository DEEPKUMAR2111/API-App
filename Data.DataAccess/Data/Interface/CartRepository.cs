using Data.DataAccess.Data;
using Data.DataAccess.Data.Interface.IData;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataAccess.Data.Interface
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private ApplicationDbContext _db;
        public CartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public int IncrementCartItem(Cart cart, int count)
        {
           cart.Count+=count;
            _db.CartItems.Update(cart);
            return cart.Count;
        }
        public int DecrementCartItem(Cart cart, int count)
        {
            cart.Count -= count;
            _db.CartItems.Update(cart);
            return cart.Count;
        }
    }
}
