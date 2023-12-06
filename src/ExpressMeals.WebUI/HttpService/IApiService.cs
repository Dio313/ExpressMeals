using ExpressMeals.Contracts.ViewModels;
using ExpressMeals.Contracts.Wrappers;
using System.Security.Claims;

namespace ExpressMeals.WebUI.HttpService;

public interface IApiService
{
    Task<List<CategoryVm>> GetAllCategories();
    Task<CategoryVm> GetCategory(int id);
    Task<MealCategoryVm> GetMealCategory(int id);
    Task<ApiResponse<CategoryVm>> CreateCategory(CategoryVm model);
    Task<ApiResponse<CategoryVm>> UpdateCategory(CategoryVm model);
    Task<HttpResponseMessage> DeleteCategory(int id);

    Task<PaginatedList<MealVm>> GetMealPagedResult (int? categoryId, string searchQuery, int? page);
    Task<PaginatedList<MealVm>> GetMealFilterPaged(int? categoryId, string searchQuery, int? page);
    Task<List<string>> GetSearchSuggestion(int? categoryId, string searchQuery, int? page);
    Task<List<MealVm>> GetAllMeals();
    Task<MealVm> GetMeal(int id);
    Task<ApiResponse<MealVm>> CreateMeal(MealVm model);
    Task<ApiResponse<MealVm>> UpdateMeal(MealVm model);
    Task<HttpResponseMessage> DeleteMeal(int id);

    Task<ClaimsPrincipal> CurrentUser();
    Task<ApiResponse<RegisterVm>> Register(RegisterVm registerModel);
    Task<ApiResponse<LoginResponse>> Login(LoginVm loginModel);
    Task<string> RefreshToken();
    Task Logout();


   
}