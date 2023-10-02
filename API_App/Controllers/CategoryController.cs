using Data.Models;
using Data.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Data.DataAccess.Data.Interface.IData;
using Microsoft.AspNetCore.Authorization;

namespace API_App.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[Action]")]
    public class CategoryController : ControllerBase
    {
      
        private readonly IUnitOfWork _db;
        private readonly ApplicationDbContext _contetx;

        public CategoryController(IUnitOfWork db, ApplicationDbContext contetx)
        {
            _db = db;
            _contetx = contetx;
        }

        [HttpGet]
        public  IActionResult GetCategory(int? pageNumber, int? pageSize, string? search)
        {
            int currentPage = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 5;
            var categories =  _db.CategoryRepository.GetAll();

            if (categories == null)
            {
                return NotFound();
            }

            if (search != null)
            {
                search= search.ToLower();
                categories=categories.Where(x => x.CategoryName.ToLower().Contains(search) || x.Discription.ToLower().Contains(search)).ToList();
            }

            var categoryList = new
            {
                data = categories.Skip((currentPage - 1) * currentPageSize).Take(currentPageSize),
                totalCount = categories.Count()
            };

            return Ok(categoryList);
        }

        [HttpPost]
        public IActionResult AddCategory([FromBody] Category category)
        {

            if (category == null)
            {
                return NotFound();
            }

             _db.CategoryRepository.Add(category);
             _db.Save();
            return Ok("Category Added Successfully.");
        }

        [HttpGet("{id}")]
        public IActionResult CategoryById(int id)
        {
            Category category =  _db.CategoryRepository.GetFirstOrDefault(x => x.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

      
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            Category category =  _db.CategoryRepository.GetFirstOrDefault(x => x.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
             _db.CategoryRepository.Remove(category);
            _db.Save();
            return Ok( "Category Deleted Successfully.");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id,[FromBody] Category category)
        {
            if (category.CategoryId != id)
            {
                return NotFound();
            }

            Category categoryDB =  _db.CategoryRepository.GetFirstOrDefault(x => x.CategoryId == id);
            if (categoryDB == null)
            {
                return NotFound();
            }
            categoryDB.CategoryName = category.CategoryName;
            categoryDB.CreatedDate = category.CreatedDate;
            categoryDB.Discription=category.Discription;

            _db.CategoryRepository.Update(categoryDB);
             _db.Save();
            return Ok("Category Updated SuccessFully.");
        }


    }
}