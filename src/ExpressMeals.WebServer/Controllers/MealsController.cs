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
    public class MealsController : ControllerBase
    {
        private readonly AppDataContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _env;
        private const int MealsPerPage = 15;

        public MealsController(AppDataContext context, IMapper mapper,IFileService fileService, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<MealVm>>> GetAllPagedAsync(int? categoryId, string searchQuery, int? page)
        {
            try
            {
                var collections = _context.Meals.Select(s => s);

                if (categoryId != null)
                {
                    collections = collections.Where(p => p.Category.Id == categoryId);
                }

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    var queryString = searchQuery.Trim();
                    collections = collections.Where(s => s.Category != null && s.Category.Name != null && s.Category != null && s.Name != null && (s.Name.Contains(queryString)
                        || s.Category.Name.Contains(queryString)));
                }

                var meals = await collections.AsNoTracking()
                    .OrderBy(o => o.Name)
                    .Select(b => new MealVm()
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Price = b.Price,
                        ImageUrl = b.ImageUrl,
                        Category = b.Category.Name

                    }).ToListAsync();

                return Ok(await PaginatedList<MealVm>.CreateAsync(meals, page ?? 1, MealsPerPage));
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error retrieving data from the database ---- {exception.InnerException!.Message}");
            }
            
        }


        [HttpGet("SearchSuggestion")]
        public async Task<ActionResult<List<string>>> GetSearchSuggestionAsync(int? categoryId, string searchQuery, int? page)
        {
            try
            {
                var meals = await GetAllPagedAsync(categoryId, searchQuery, page);

                List<string> result = new List<string>();

                foreach (var item in meals.Value.Items)
                {
                    if (item.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    {
                        if (item.Name != null) result.Add(item.Name);

                        if (item.Category != null)
                        {
                            result.Add(item.Category);
                        }
                    }

                    if (item.Description != null)
                    {
                        var punctuation = item.Description.Where(char.IsPunctuation)
                            .Distinct().ToArray();
                        var words = item.Description.Split()
                            .Select(s => s.Trim(punctuation));

                        foreach (var word in words)
                        {
                            if (word.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                                && !result.Contains(word))
                            {
                                result.Add(word);
                            }
                        }
                    }
                }

                return Ok(result);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error retrieving data from the database ---- {exception.InnerException!.Message}");
            }
        }


        [HttpGet("lists")]
        public async Task<IActionResult> GetAllMeals()
        {
            try
            {
                var products = await _context.Meals.Include(c => c.Category).AsNoTracking().ToListAsync();
                return Ok( _mapper.Map<List<MealVm>>(products));
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error retrieving data from the database ---- {exception.InnerException!.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateMeal(MealVm model)
        {
            ArgumentNullException.ThrowIfNull(model);

            try
            {
                if (_context.Meals != null && _context.Meals.Any(d => d.Name == model.Name))
                {
                    return Conflict(new ApiResponse(false, new List<string>(){$"A Meal with the name: {model.Name} already exists, please choose another name" }));
                }

                if (!string.IsNullOrWhiteSpace(model.ImageUrl))
                {
                    var image = Convert.FromBase64String(model.ImageUrl);
                    model.ImageUrl = await _fileService.SaveFile(image, "jpg", "ProductImages");
                }
                var product = _mapper.Map<Meal>(model);
                _context.Add(product);
                await _context.SaveChangesAsync();
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(false, new List<string> {ex.Message}));
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMeal(MealVm model)
        {
            ArgumentNullException.ThrowIfNull(model);

            try
            {
                var productToUpdate = await _context.Meals.FindAsync(model.Id);

                if (productToUpdate == null)
                {
                    return BadRequest("Meal Id not found");
                }

                _mapper.Map(model, productToUpdate);

                if (!string.IsNullOrWhiteSpace(model.ImageUrl))
                {
                    var editImage = Convert.FromBase64String(model.ImageUrl);
                    productToUpdate.ImageUrl = await _fileService.EditFile(editImage,
                        "jpg", "ProductImages", productToUpdate.ImageUrl);
                }

                _context.Meals.Update(productToUpdate);

                await _context.SaveChangesAsync();
                return Ok(productToUpdate);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(false, new List<string> {ex.Message}));
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var product = await _context.Meals.FindAsync(id);

            if (product == null)
            {
                return BadRequest(new ApiResponse(false, new List<string>(){"Meal Id not found"}));
            }

            try
            {
                var imageFileName = Path.GetFileName(product.ImageUrl);

                if (!string.Equals(imageFileName, "default.png"))
                {
                    var imagePhysicalPath = Path.Combine(_env.ContentRootPath, "wwwroot", "ProductImages", imageFileName ?? string.Empty);
                    System.IO.File.Delete(imagePhysicalPath);
                }

                _context.Meals.Remove(product);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(false, new List<string> {ex.Message}));
            }
        }
    }
}
