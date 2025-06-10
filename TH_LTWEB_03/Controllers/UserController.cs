using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TH_LTWEB_03.Repositories;
using TranTrungVietHoang.Models;

namespace TranTrungVietHoang.Controllers
{
     
    
    public class UserController : Controller
    {

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public UserController(IProductRepository productRepository,
        ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSort"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["CategorySort"] = sortOrder == "Category" ? "category_desc" : "Category";
            ViewData["CurrentFilter"] = searchString;

            var products = await _productRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "Price":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "Category":
                    products = products.OrderBy(p => p.Category.Name);
                    break;
                case "category_desc":
                    products = products.OrderByDescending(p => p.Category.Name);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            return View(products);
        }

        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
