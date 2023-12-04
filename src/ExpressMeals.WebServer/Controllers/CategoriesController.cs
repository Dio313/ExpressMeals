using AutoMapper;
using ExpressMeals.Contracts.ViewModels;
using ExpressMeals.Contracts.Wrappers;
using ExpressMeals.Domains.Entities;
using ExpressMeals.Domains.helpers;
using ExpressMeals.Infrastructures.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpressMeals.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDataContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _env;

        public CategoriesController(AppDataContext context, IMapper mapper,IFileService fileService, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
            _env = env;
        }


        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = _mapper.Map<List<CategoryVm>>(await _context.Categories.AsNoTracking().ToListAsync());
                return Ok(categories);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category is null)
                {
                    return BadRequest(new ApiResponse(false, new List<string> { "Category not found" }));
                }
                return Ok(_mapper.Map<CategoryVm>(category));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
            
        }

        [HttpGet("{id}/meals")]
        public async Task<IActionResult> GetMealCategories(int id)
        {
            var productCategory = await _context.Categories.AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new MealCategoryVm()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Meals = (List<MealVm>)p.Meals.Select(b => new MealVm()
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Price = b.Price,
                        ImageUrl = b.ImageUrl
                    })
                }).FirstOrDefaultAsync();

            if (productCategory == null)
            {
                return null;
            }

            return Ok(productCategory);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryVm model)
        {
            ArgumentNullException.ThrowIfNull(model);

            try
            {
                if (_context.Categories.Any(d => d.Name == model.Name))
                {
                    return Conflict(new ApiResponse(false, new List<string>() {$"A Category with the name: {model.Name} already exists, please choose another name" }));
                }

                if (!string.IsNullOrWhiteSpace(model.Icon))
                {
                    var image = Convert.FromBase64String(model.Icon);
                    model.Icon = await _fileService.SaveFile(image, "jpg", "CategoryIcons");
                }
                var category = _mapper.Map<Category>(model);

                if (_context.Categories != null) _context.Categories.Add(category);

                await _context.SaveChangesAsync();

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(false, new List<string> {ex.Message}));
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(CategoryVm model)
        {
            ArgumentNullException.ThrowIfNull(model);

            try
            {
                var categoryToUpdate = await _context.Categories.FindAsync(model.Id);

                if (categoryToUpdate == null)
                {
                    return null;
                }

                _mapper.Map(model, categoryToUpdate);

                if (!string.IsNullOrWhiteSpace(model.Icon))
                {
                    var editImage = Convert.FromBase64String(model.Icon);
                    categoryToUpdate.Icon = await _fileService.EditFile(editImage,
                        "jpg", "CategoryIcons", categoryToUpdate.Icon);
                }
                _context.Categories.Update(categoryToUpdate);

                await _context.SaveChangesAsync();

                return Ok(categoryToUpdate);
                
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(false, new List<string> {ex.Message}));
            }
            
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category is null)
            {
                return BadRequest(new ApiResponse(false, new List<string> { "Category not found" }));
            }

            try
            {
                var imageFileName = Path.GetFileName(category.Icon);

                if (!string.Equals(imageFileName, "default.png"))
                {
                    var imagePhysicalPath = Path.Combine(_env.ContentRootPath, "wwwroot", "CategoryIcons", imageFileName ?? string.Empty);
                    System.IO.File.Delete(imagePhysicalPath);
                }
                _context.Categories.Remove(category);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(false, new List<string> {ex.Message}));
            }
            
        }
    }
}
