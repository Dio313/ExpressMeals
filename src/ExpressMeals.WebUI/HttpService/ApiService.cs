using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using ExpressMeals.Contracts.ViewModels;
using ExpressMeals.Contracts.Wrappers;
using ExpressMeals.WebUI.AuthProvider;
using Microsoft.AspNetCore.Components.Authorization;

namespace ExpressMeals.WebUI.HttpService;

public class ApiService : IApiService
{
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;
    public ApiService(IHttpClientFactory clientFactory, ILocalStorageService localStorage,  AuthenticationStateProvider authStateProvider)
    {
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
        _httpClient = clientFactory.CreateClient("ExpressMealsApi");
        _httpClient.BaseAddress = new Uri("https://localhost:7177/");
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };
    }


    #region Categories

    public async Task<List<CategoryVm>> GetAllCategories()
    {
        var result = await _httpClient.GetAsync("api/Categories");
        var content = await result.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<ApiResponse<List<CategoryVm>>>(content, _serializerOptions);
        return items.Data;
    }
    public async Task<CategoryVm> GetCategory(int id)
    {
        var result = await _httpClient.GetAsync($"api/categories/{id}");
        var content = await result.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<ApiResponse<CategoryVm>>(content, _serializerOptions);
        return items.Data;
    }

    public async Task<MealCategoryVm> GetMealCategory(int id)
    {
        var result = await _httpClient.GetAsync($"api/categories/{id}/meals");
        var content = await result.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<ApiResponse<MealCategoryVm>>(content, _serializerOptions);
        return items.Data;
    }

    public async Task<ApiResponse<CategoryVm>> CreateCategory(CategoryVm model)
    {
        var response =  await _httpClient.PostAsJsonAsync("api/Categories", model);
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<ApiResponse<CategoryVm>>(responseAsString, _serializerOptions);
        return responseObject; 
        
    }

    public async Task<ApiResponse<CategoryVm>> UpdateCategory(CategoryVm model)
    {
        var response = await _httpClient.PutAsJsonAsync("api/Categories", model);
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<ApiResponse<CategoryVm>>(responseAsString, _serializerOptions);
        return responseObject; 
    }

    public async Task<HttpResponseMessage> DeleteCategory(int id)
    {
        return await _httpClient.DeleteAsync($"api/categories/{id}");
    }

    #endregion

    #region Meals

    public async Task<PaginatedList<MealVm>> GetMealPagedResult (int? categoryId, string searchQuery, int? page)
    {
        var itemsUrl = "api/meals" + "?categoryId={0}&searchQuery={1}&page={2}";
        return await _httpClient.GetFromJsonAsync<PaginatedList<MealVm>>(
            string.Format(itemsUrl, categoryId, searchQuery, page));
    }
    

    public async Task<PaginatedList<MealVm>> GetMealFilterPaged (int? categoryId, string searchQuery, int? page)
    {
        var itemsUrl = "api/meals" + "/filterPages?categoryId={0}&searchQuery={1}&page={2}";
        return await _httpClient.GetFromJsonAsync<PaginatedList<MealVm>>(
            string.Format(itemsUrl, categoryId, searchQuery, page));
    }

    public async Task<List<string>> GetSearchSuggestion(int? categoryId, string searchQuery, int? page)
    {
        var itemsUrl = "api/meals" + "/SearchSuggestion?categoryId={0}&searchQuery={1}&page={2}";
        return await _httpClient.GetFromJsonAsync<List<string>>(
            string.Format(itemsUrl, categoryId, searchQuery, page));
    }

    public async Task<List<MealVm>> GetAllMeals()
    {
        var result = await _httpClient.GetAsync("api/meals" + "/lists");
        var content = await result.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<ApiResponse<List<MealVm>>>(content, _serializerOptions);
        return items.Data;
    }

    public async Task<MealVm> GetMeal(int id)
    {
        var result = await _httpClient.GetAsync($"api/meals/{id}");
        var content = await result.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<ApiResponse<MealVm>>(content, _serializerOptions);
        return items.Data;
    }

    public async Task<ApiResponse<MealVm>> CreateMeal(MealVm model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/meals", model);
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<ApiResponse<MealVm>>(responseAsString, _serializerOptions);
        return responseObject; 
    }

    public async Task<ApiResponse<MealVm>> UpdateMeal(MealVm model)
    {
        var response = await _httpClient.PutAsJsonAsync("api/meals", model);
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<ApiResponse<MealVm>>(responseAsString, _serializerOptions);
        return responseObject;
    }

    public async Task<HttpResponseMessage> DeleteMeal(int id)
    {
        return await _httpClient.DeleteAsync($"api/meals/{id}");
    }

    #endregion

    #region Account

    public async Task<ClaimsPrincipal> CurrentUser()
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        return state.User;
    }
    public async Task<ApiResponse<RegisterVm>> Register(RegisterVm registerModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Accounts/register", registerModel);
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<ApiResponse<RegisterVm>>(responseAsString, _serializerOptions);
        return responseObject; 
    }

    public async Task<ApiResponse<LoginResponse>> Login(LoginVm loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync( "api/Accounts/login", loginModel);
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(responseAsString, _serializerOptions);
        if (responseObject.IsSuccess)
        {
            var token = responseObject.Data.AccessToken;
            var refreshToken = responseObject.Data.RefreshToken;
            await _localStorage.SetItemAsync("authToken", token);
            await _localStorage.SetItemAsync("refreshToken", refreshToken);
            ((CustomAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated(responseObject.Data.AccessToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", responseObject.Data.AccessToken);
            return responseObject;
        }

        return new ApiResponse<LoginResponse>(false, new List<string>(){"Login Fail"}, null);
    }


    public async Task<string> RefreshToken()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

        var tokenRequest = JsonSerializer.Serialize(new LoginResponse { AccessToken = token, RefreshToken = refreshToken });
        var bodyContent = new StringContent(tokenRequest, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync( "api/Accounts/refresh", bodyContent);

        //var result = await response.ToResult<LoginResponse>();
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(responseAsString, _serializerOptions);

        if (!responseObject.IsSuccess)
        {
            throw new ApplicationException("Something went wrong during the refresh token action");
        }

        token = responseObject.Data.AccessToken;
        refreshToken = responseObject.Data.RefreshToken;
        await _localStorage.SetItemAsync("authToken", token);
        await _localStorage.SetItemAsync("refreshToken", refreshToken);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return token;
    }


    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("refreshToken");
        ((CustomAuthenticationStateProvider)_authStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    #endregion

   
   
}