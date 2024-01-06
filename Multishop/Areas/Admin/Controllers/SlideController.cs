using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Multishop.Areas.Admin.ViewModels;
using Multishop.Data;
using Multishop.Models;
using Multishop.Utilities.Extentions;

namespace Multishop.Areas.Admin.Controllers;

[Area("Admin")]
public class SlideController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    public SlideController(AppDbContext context, IMapper mapper, IWebHostEnvironment env)
    {
        _context = context;
        _mapper = mapper;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var slides = await _context.Slides.ToListAsync();
        return View(slides);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateSlideVM slideVM)
    {
        if (!ModelState.IsValid) return View();
        if (!slideVM.Photo.ValidateType())
        {
            ModelState.AddModelError("Photo", "Wrong file type");
            return View();
        }
        if (slideVM.Photo.ValidateSize(2 * 1024))
        {
            ModelState.AddModelError("Photo", "It shouldn't exceed 2 mb");
            return View();
        }
        string fileName = await slideVM.Photo.CreateFile(_env.WebRootPath, "assets", "img");
        if (slideVM.ButtonTitle is null) slideVM.ButtonTitle = "Shop Now";
        Slide slide = _mapper.Map<Slide>(slideVM);
        slide.ImageUrl = fileName;
        await _context.Slides.AddAsync(slide);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return BadRequest();
        Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
        if (slide == null) return NotFound();

        slide.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
        _context.Slides.Remove(slide);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0) return BadRequest();
        var slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
        if (slide == null) return NotFound();

        return View(slide);
    }

    public async Task<IActionResult> Update(int id)
    {
        if (id <= 0) return BadRequest();
        Slide slide = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
        if (slide is null) return NotFound();

        UpdateSlideVM vm = _mapper.Map<UpdateSlideVM>(slide);
        vm.ImageUrl = slide.ImageUrl;
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, UpdateSlideVM slidevm)
    {
        if (!ModelState.IsValid) return View(slidevm);
        Slide existed = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
        string oldImageUrl = existed.ImageUrl;
        if (existed is null) return NotFound();

        bool result = _context.Slides.Any(c => c.Title == slidevm.Title && c.Order == slidevm.Order && c.Id != id);
        if (!result)
        {
            existed = _mapper.Map(slidevm,existed);
            existed.ImageUrl = oldImageUrl;
            if (slidevm.Photo is not null)
            {
                if (!slidevm.Photo.ValidateType())
                {
                    ModelState.AddModelError("Photo", "Wrong file type");
                    return View(slidevm);
                }
                if (slidevm.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "It shouldn't exceed 2 mb");
                    return View(slidevm);
                }
                string newImage = await slidevm.Photo.CreateFile(_env.WebRootPath, "assets", "img");
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
