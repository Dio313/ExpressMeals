using System.Net;
using AutoMapper;
using ExpressMeals.Contracts.ViewModels;
using ExpressMeals.Contracts.Wrappers;
using ExpressMeals.Domains.Entities;
using ExpressMeals.Domains.Exceptions;
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
        public async Task<PaginatedList<MealVm>> GetAllMealsPaged(int? categoryId, string searchQuery, int? page)
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
                    CategoryName = b.Category.Name

                }).ToListAsync();


            return await PaginatedList<MealVm>.CreateAsync(meals, page ?? 1, MealsPerPage);
            
        }

        [HttpGet("filterPages")]
        public async Task<PaginatedList<MealVm>> GetAllFilterPagedAsync(int? categoryId, string searchQuery, int? page)
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
                    CategoryName = b.Category.Name

                }).ToListAsync();


            return await PaginatedList<MealVm>.CreateAsync(meals, page ?? 1, MealsPerPage);
        }


        [HttpGet("SearchSuggestion")]
        public async Task<List<string>> GetSearchSuggestion(int? categoryId, string searchQuery, int? page)
        {
            var meals = await GetAllFilterPagedAsync(categoryId, searchQuery, page);

            List<string> result = new();

            foreach (var item in meals.Items)
            {
                if (item.Name != null) result.Add(item.Name);

                if (item.CategoryName != null)
                {
                    result.Add(item.CategoryName);
                }
            }

            return result;
        }

        [HttpGet("lists")]
        public async Task<ApiResponse<List<MealVm>>> GetAllAsync()
        {
            var meals = await _context.Meals.Include(c => c.Category).AsNoTracking().ToListAsync();
            var dto = _mapper.Map<List<MealVm>>(meals);
            return new ApiResponse<List<MealVm>>(true, null, dto);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<MealVm>> GetByIdAsync(int id)
        {
            var meal = await _context.Meals.Include(c => c.Category).AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id);
            if (meal == null)
            {
                throw new NotFoundException("Meal not found", HttpStatusCode.NotFound);
            }
            var productDto = _mapper.Map<MealVm>(meal);
            return new ApiResponse<MealVm>(true, null, productDto);
        }

        [HttpPost]
        public async Task<ApiResponse<MealVm>> CreateAsync(MealVm model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (_context.Meals != null && _context.Meals.Any(d => d.Name == model.Name))
            {
                throw new DuplicateException(
                    $"A Meal with the name: {model.Name} already exists, please choose another name",
                    HttpStatusCode.Conflict);
            }

            if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                var image = Convert.FromBase64String(model.ImageUrl);
                model.ImageUrl = await _fileService.SaveFile(image, "jpg", "ProductImages");
            }
            var product = _mapper.Map<Meal>(model);
            _context.Add((object)product);
            await _context.SaveChangesAsync();
            return new ApiResponse<MealVm>(true, null, new MealVm());
        }

        [HttpPut]
        public async Task<ApiResponse<MealVm>> UpdateAsync(MealVm model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (_context.Meals != null)
            {
                var productToUpdate = await _context.Meals.FindAsync(model.Id);

                if (productToUpdate == null)
                {
                    throw new NotFoundException("Meal Id not found", HttpStatusCode.NotFound);
                }

                _mapper.Map(model, productToUpdate);

                if (!string.IsNullOrWhiteSpace(model.ImageUrl))
                {
                    var editImage = Convert.FromBase64String(model.ImageUrl);
                    productToUpdate.ImageUrl = await _fileService.EditFile(editImage,
                        "jpg", "ProductImages", productToUpdate.ImageUrl);
                }

                _context.Meals.Update(productToUpdate);
            }

            await _context.SaveChangesAsync();
            return new ApiResponse<MealVm>(true, null, new MealVm());
        }


        [HttpDelete("{id}")]
        public async Task<ApiResponse<int>> DeleteAsync(int id)
        {
            if (_context.Meals != null)
            {
                var product = await _context.Meals.FindAsync(id);

                if (product == null)
                {
                    throw new NotFoundException("Meal Id not found", HttpStatusCode.NotFound);
                }
                var imageFileName = Path.GetFileName(product.ImageUrl);

                if (!string.Equals(imageFileName, "default.png"))
                {
                    var imagePhysicalPath = Path.Combine(_env.ContentRootPath, "wwwroot", "ProductImages", imageFileName ?? string.Empty);
                    System.IO.File.Delete(imagePhysicalPath);
                }

                _context.Meals.Remove(product);
            }

            await _context.SaveChangesAsync();

            return new ApiResponse<int>(true, null, id);
        }
    }
}
