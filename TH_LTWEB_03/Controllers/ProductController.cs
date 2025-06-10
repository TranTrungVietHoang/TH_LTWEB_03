using Microsoft.AspNetCore.Mvc;
using TH_LTWEB_03.Models;
using TH_LTWEB_03.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using TranTrungVietHoang.Models;

namespace TH_LTWEB_03.Controllers
{
    
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IProductRepository productRepository,
        ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        //Hiển thị danh sách sản phẩm với tìm kiếm và sắp xếp
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

        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Xử lý thêm sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageUrl != null)
                    {
                        product.ImageUrl = await SaveImage(imageUrl);
                    }
                    await _productRepository.AddAsync(product);
                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi thêm sản phẩm: {ex.Message}";
                }
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Hàm lưu hình ảnh
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName;
        }

        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _productRepository.GetByIdAsync(id);
                    if (imageUrl == null)
                    {
                        product.ImageUrl = existingProduct.ImageUrl;
                    }
                    else
                    {
                        product.ImageUrl = await SaveImage(imageUrl);
                    }

                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Description = product.Description;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.ImageUrl = product.ImageUrl;

                    await _productRepository.UpdateAsync(existingProduct);
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật sản phẩm: {ex.Message}";
                }
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Sản phẩm không tồn tại!";
                    return RedirectToAction(nameof(Index));
                }
                await _productRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa sản phẩm: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
