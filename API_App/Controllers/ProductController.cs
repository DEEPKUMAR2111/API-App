using Data.DataAccess.Data;
using Data.DataAccess.Data.Interface.IData;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_App.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[Action]")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _db;
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context, IUnitOfWork db)
        {
            _db = db;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProducts(int? pageNumber,int? pageSize)
        {
            int currentPage = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 5;
            var list=_db.ProductRepository.GetAll(includeProperties:"Category");
            var Products =  (from product in list
                                  select new
                                  {
                                      Id = product.ProductId,
                                      Name = product.Name,
                                      Discription = product.Discription,
                                      Price = product.Price,
                                      Image = product.ImgUrl,
                                      Catagery=product.Category.CategoryName
                                  });
           
            if (Products == null)
            {
                return NotFound();
            }

            var productList = new 
            { 
                data= Products.Skip((currentPage - 1) * currentPageSize).Take(currentPageSize),
                totalCount=Products.Count()
            };

            return Ok(productList);
        }

        [HttpGet("{id}")]
        public IActionResult ProductById(int id)
        {
            Product product =  _db.ProductRepository.GetFirstOrDefault(x => x.ProductId == id,includeProperties:"Category");
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }


        [HttpPost]
        public IActionResult AddProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return NotFound();
            }

           product.Category=  _db.CategoryRepository.GetFirstOrDefault(x=>x.CategoryId == product.CategoryId);
            if (product.Category == null)
            {
                return NotFound();
            }

             _db.ProductRepository.Add(product);
             _db.Save();
            return Ok("Product Added Successfully.");
        }

        [HttpDelete("{id}")]
        public  IActionResult DeleteProduct(int id)
        {
            Product product =  _db.ProductRepository.GetFirstOrDefault(x => x.ProductId== id);
            if (product == null)
            {
                return NotFound();
            }
            _db.ProductRepository.Remove(product);
             _db.Save();
            return Ok("Product Deleted Successfully.");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            if (product.ProductId != id)
            {
                return NotFound();
            }

            Product productDB =  _db.ProductRepository.GetFirstOrDefault(x => x.ProductId == id);
            if (productDB == null)
            {
                return NotFound();
            }

            productDB.Name = product.Name;
            productDB.Discription = product.Discription;
            productDB.Price = product.Price;
            productDB.ImgUrl = product.ImgUrl;
            productDB.CategoryId = product.CategoryId;
            productDB.Category =  _db.CategoryRepository.GetFirstOrDefault(x => x.CategoryId == product.CategoryId);
            if (productDB.Category == null)
            {
                return NotFound();
            }

            _db.ProductRepository.Update(productDB);
             _db.Save();

            return Ok("Product Updated SuccessFully.");
        }
    }
}