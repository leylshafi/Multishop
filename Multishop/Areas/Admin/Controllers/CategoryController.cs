using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Multishop.Areas.Admin.ViewModels;
using Multishop.Data;
using Multishop.Models;
using Multishop.Utilities.Extentions;

namespace Multishop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public CategoryController(AppDbContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.Include(c=>c.Products).ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid) return View();
            if (!categoryVM.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "Wrong file type");
                return View();
            }
            if (categoryVM.Photo.ValidateSize(2 * 1024))
            {
                ModelState.AddModelError("Photo", "It shouldn't exceed 2 mb");
                return View();
            }
            string fileName = await categoryVM.Photo.CreateFile(_env.WebRootPath, "assets", "img");
            Category category = _mapper.Map<Category>(categoryVM);
            category.ImageUrl = fileName;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            var category = await _context.Categories.Include(c=>c.Products).ThenInclude(pi=>pi.ProductImages).FirstOrDefaultAsync(c=>c.Id==id);
            if (category is null) return NotFound();

            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
			if (id <= 0) return BadRequest();
			Category category = await _context.Categories.FirstOrDefaultAsync(s => s.Id == id);
			if (category == null) return NotFound();

			category.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Update(int id)
		{
			if (id <= 0) return BadRequest();
			Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
			if (category is null) return NotFound();

			UpdateCategoryVM vm = _mapper.Map<UpdateCategoryVM>(category);
			vm.ImageUrl = category.ImageUrl;
			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Update(int id, UpdateCategoryVM categoryVM)
		{
			if (!ModelState.IsValid) return View(categoryVM);
			Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
			string oldImageUrl = existed.ImageUrl;
			if (existed is null) return NotFound();

			bool result = _context.Categories.Any(c => c.Name == categoryVM.Name && c.Id != id);
			if (!result)
			{
				existed = _mapper.Map(categoryVM, existed);
				existed.ImageUrl = oldImageUrl;
				if (categoryVM.Photo is not null)
				{
					if (!categoryVM.Photo.ValidateType())
					{
						ModelState.AddModelError("Photo", "Wrong file type");
						return View(categoryVM);
					}
					if (categoryVM.Photo.ValidateSize(2 * 1024))
					{
						ModelState.AddModelError("Photo", "It shouldn't exceed 2 mb");
						return View(categoryVM);
					}
					string newImage = await categoryVM.Photo.CreateFile(_env.WebRootPath, "assets", "img");
					existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
					existed.ImageUrl = newImage;

				}
				await _context.SaveChangesAsync();
			}
			else
			{
				ModelState.AddModelError("Title", "There is already such title");
				ModelState.AddModelError("Order", "There is already such order");
				return View(existed);
			}


			return RedirectToAction(nameof(Index));
		}
	}
}
