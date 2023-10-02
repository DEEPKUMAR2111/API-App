
using Data.DataAccess.Data;
using Data.DataAccess.Data.Interface.IData;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_App.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork _db;
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context, IUnitOfWork db)
        {
            _db = db;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetCategories()    
        {
            var categoryList = await _context.Categories.FromSqlRaw("Exec CategoriesList").ToListAsync();
            return Ok(categoryList);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCategoryById(int Id)
        {
            var Sqlstr = "EXEC CategoriesList @Id=" + Id;
            var studentList = await _context.Categories.FromSqlRaw(Sqlstr).ToListAsync();
            return Ok(studentList);

        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Product> Products = _db.ProductRepository.GetAll(includeProperties: "Category");
            return Ok(Products);
        }

        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var cart = new Cart()
            {
                Count = 1,
                ProductId = id,
                Product = _db.ProductRepository.GetFirstOrDefault(u => u.ProductId == id, includeProperties: "Category")
            };
            return Ok(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(Cart cart)
        {
           
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            cart.ApplicationUser = _db.ApplicationUserRepository.GetFirstOrDefault(x => x.UserName == claimsIdentity.Name);
            if (cart.ApplicationUser != null)
            {
                cart.ApplicationUserId = cart.ApplicationUser.Id;
                var cartItem = _db.CartRepository.GetFirstOrDefault(x => x.ProductId == cart.ProductId && x.ApplicationUserId == cart.ApplicationUserId);
                if (cartItem == null)
                {
                    cart.ApplicationUser = null;
                    _db.CartRepository.Add(cart);
                    _db.Save();

                }
              else
                {
                    _db.CartRepository.IncrementCartItem(cartItem, cart.Count);
                    _db.Save();
                }

                var Message = new { text = "Item added into cart items."};
                return Ok(Message);
            }
           else {
                return NotFound("Item not Found.");
            }

        }

    }
}
