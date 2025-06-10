using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TH_LTWEB_03.Models;
using TH_LTWEB_03.Repositories;
using TranTrungVietHoang.Models;

namespace TranTrungVietHoang.Controllers
{
    
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoriesController : Controller
    {

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            var categories = await _categoryRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(c => c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    categories = categories.OrderByDescending(c => c.Name);
                    break;
                default:
                    categories = categories.OrderBy(c => c.Name);
                    break;
            }

            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryRepository.AddAsync(category);
                    TempData["SuccessMessage"] = "Thêm danh mục thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi thêm danh mục: {ex.Message}";
                }
            }
            return View(category);
        }

        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryRepository.UpdateAsync(category);
                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật danh mục: {ex.Message}";
                }
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Danh mục không tồn tại!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _categoryRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Xóa danh mục thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa danh mục: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


    }
}

    

