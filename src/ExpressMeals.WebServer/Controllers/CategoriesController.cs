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
        public async Task<ApiResponse<List<CategoryVm>>> GetCategories()
        {
            var categories = _mapper.Map<List<CategoryVm>>(await _context.Categories.AsNoTracking().ToListAsync());
            return new ApiResponse<List<CategoryVm>>(true, null, categories);
        }


        [HttpGet("{id}")]
        public async Task<ApiResponse<CategoryVm>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
            {
                throw new NotFoundException("Category not found", HttpStatusCode.NotFound);
            }
            var dto  =_mapper.Map<CategoryVm>(category);
            return new ApiResponse<CategoryVm>(true, null, dto);
            
        }

        [HttpGet("{id}/meals")]
        public async Task<ApiResponse<MealCategoryVm>> GetMealCategories(int id)
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
                throw new NotFoundException();
            }

            return new ApiResponse<MealCategoryVm>(true,null, productCategory);
        }

        [HttpPost]
        public async Task<ApiResponse<CategoryVm>> CreateCategory(CategoryVm model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (_context.Categories.Any(d => d.Name == model.Name))
            {
                throw new DuplicateException($"A Category with the name: {model.Name} already exists, please choose another name", HttpStatusCode.Conflict);
            }

            if (!string.IsNullOrWhiteSpace(model.Icon))
            {
                var image = Convert.FromBase64String(model.Icon);
                model.Icon = await _fileService.SaveFile(image, "jpg", "CategoryIcons");
            }
            var category = _mapper.Map<Category>(model);

            if (_context.Categories != null) _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return new ApiResponse<CategoryVm>(true, null, new CategoryVm());
        }

        [HttpPut]
        public async Task<ApiResponse<CategoryVm>> UpdateCategory(CategoryVm model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var categoryToUpdate = await _context.Categories.FindAsync(model.Id);

            if (categoryToUpdate == null)
            {
               throw new NotFoundException("Category not found", HttpStatusCode.NotFound);
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

            return new ApiResponse<CategoryVm>(true, null, new CategoryVm());
        }


        [HttpDelete("{id}")]
        public async Task<ApiResponse<int>> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category is null)
            {
                throw new NotFoundException("Category not found", HttpStatusCode.NotFound);
            }

            var imageFileName = Path.GetFileName(category.Icon);

            if (!string.Equals(imageFileName, "default.png"))
            {
                var imagePhysicalPath = Path.Combine(_env.ContentRootPath, "wwwroot", "CategoryIcons", imageFileName ?? string.Empty);
                System.IO.File.Delete(imagePhysicalPath);
            }
            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return new ApiResponse<int>(true,  null, id);
            
        }
    }
}
