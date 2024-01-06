using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Multishop.Areas.Admin.ViewModels;
using Multishop.Data;
using Multishop.Models;
using Multishop.Utilities.Extentions;
using NuGet.Packaging;

namespace Multishop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.ProductImages).Include(p => p.Category).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            var vm = new CreateProductVM
            {
                Categories = await _context.Categories.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
                Colors = await _context.Colors.ToListAsync()
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                return View(productVM);
            }
            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("CategoryId", "There is no such category");
                return View(productVM);
            }
            foreach (int id in productVM.SizeIds)
            {
                bool sizeResult = await _context.Sizes.AnyAsync(t => t.Id == id);
                if (!sizeResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("SizeIds", "There is no such size");
                    return View(productVM);
                }
            }
			foreach (int id in productVM.ColorIds)
			{
				bool sizeResult = await _context.Colors.AnyAsync(t => t.Id == id);
				if (!sizeResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("ColorIds", "There is no such color");
					return View(productVM);
				}
			}
			if (!productVM.MainPhoto.ValidateType())
            {
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("MainPhoto", "Wrong file type");
                return View(productVM);
            }
            if (!productVM.MainPhoto.ValidateSize(600))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("MainPhoto", "Wrong file size");
                return View(productVM);
            }
            ProductImage image = new ProductImage
            {
                IsPrimary = true,
                Url = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "img")
            };
            Product product = _mapper.Map<Product>(productVM);
            product.ProductImages.Add(image);
            foreach (int id in productVM.SizeIds)
            {
                var pSize = new ProductSize
                {
                    SizeId = id,
                    Product = product
                };
                product.ProductSizes.Add(pSize);
            }
			foreach (int id in productVM.ColorIds)
			{
				var pColor = new ProductColor
				{
					ColorId = id,
					Product = product
				};
				product.ProductColors.Add(pColor);
			}
			TempData["Message"] = "";
            if (productVM.Photos is not null)
            {
                foreach (IFormFile photo in productVM.Photos)
                {
                    if (!photo.ValidateType())
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type wrong</p>";
                        continue;
                    }
                    if (!photo.ValidateSize(600))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size wrong</p>";
                        continue;
                    }

                    product.ProductImages.Add(new ProductImage
                    {
                        IsPrimary = false,
                        Url = await photo.CreateFile(_env.WebRootPath, "assets", "img")
                    });

                }
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            foreach (ProductImage image in existed.ProductImages)
            {
                image.Url.DeleteFile(_env.WebRootPath, "assets", "img");
            }
            _context.Products.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            var vm = new UpdateProductVM
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ProductImages = product.ProductImages,
                SizeIds = product.ProductSizes.Select(ps => ps.SizeId).ToList(),
                ColorIds = product.ProductColors.Select(ps => ps.ColorId).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
                Colors = await _context.Colors.ToListAsync()

            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            Product existed = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .Include(p=>p.ProductColors)
                .FirstOrDefaultAsync(c => c.Id == id);
            productVM.ProductImages = existed.ProductImages;
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                return View(productVM);
            }
            if (existed is null) return NotFound();

            bool result = await _context.Products.AnyAsync(c => c.CategoryId == productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                return View(productVM);
            }
            foreach (int idS in productVM.SizeIds)
            {
                bool sizeResult = await _context.Sizes.AnyAsync(t => t.Id == idS);
                if (!sizeResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("SizeIds", "There is no such size");
                    return View(productVM);
                }
            }
			foreach (int idS in productVM.ColorIds)
			{
				bool colorResult = await _context.Colors.AnyAsync(t => t.Id == idS);
				if (!colorResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("ColorIds", "There is no such color");
					return View(productVM);
				}
			}

			result = _context.Products.Any(c => c.Name == productVM.Name && c.Id != id);
            if (result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("Name", "There is already such product");
                return View(productVM);
            }

            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.ValidateType())
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("MainPhoto", "File type is not valid");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(600))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("MainPhoto", "File size is not valid");
                    return View(productVM);
                }
            }
            if (productVM.MainPhoto is not null)
            {
                string fileName = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "img");
                ProductImage mainImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                mainImage.Url.DeleteFile(_env.WebRootPath, "assets", "img");
                _context.ProductImages.Remove(mainImage);
                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    Url = fileName
                });
            }

            if (productVM.ImageIds is null)
            {
                productVM.ImageIds = new();
            }
            var removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == false).ToList();
            foreach (ProductImage pi in removeable)
            {
                pi.Url.DeleteFile(_env.WebRootPath, "assets", "img");
                existed.ProductImages.Remove(pi);
            }

            existed.ProductSizes.RemoveAll(pSize => !productVM.SizeIds.Contains(pSize.Id));

            existed.ProductSizes.AddRange(productVM.SizeIds.Where(sizeId => !existed.ProductSizes.Any(pt => pt.Id == sizeId))
                                 .Select(sizeId => new ProductSize { SizeId = sizeId }));

			existed.ProductColors.RemoveAll(pColor => !productVM.ColorIds.Contains(pColor.Id));

			existed.ProductColors.AddRange(productVM.ColorIds.Where(colorId => !existed.ProductColors.Any(pt => pt.Id == colorId))
								 .Select(colorId => new ProductColor { ColorId = colorId }));

			TempData["Message"] = "";
            if (productVM.Photos is not null)
            {
                foreach (IFormFile photo in productVM.Photos)
                {
                    if (!photo.ValidateType())
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type wrong</p>";
                        continue;
                    }
                    if (!photo.ValidateSize(600))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size wrong</p>";
                        continue;
                    }

                    existed.ProductImages.Add(new ProductImage
                    {
                        IsPrimary = false,
                        Url = await photo.CreateFile(_env.WebRootPath, "assets", "img")
                    });
                }
            }

            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            if (productVM.Price != existed.Price) existed.OldPrice = existed.Price;

            existed.Price = productVM.Price;
            existed.CategoryId = productVM.CategoryId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
