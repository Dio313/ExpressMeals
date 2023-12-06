using Blazored.LocalStorage;
using ExpressMeals.WebUI.Models;
using Constant = Contracts.Constants.Constant;

namespace ExpressMeals.WebUI.HttpService;

public interface ICartService
{
    event Action OnChange;
    Task IncrementCart(ShoppingCart cartToAdd);
    Task DecrementCart(ShoppingCart cartToDecrement);
}

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorageService;
    public event Action OnChange;

    public CartService(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task IncrementCart(ShoppingCart cartToAdd)
    {
        var cart = await _localStorageService.GetItemAsync<List<ShoppingCart>>(Constant.ShoppingCart);
        bool itemInCart = false;

        if (cart == null)
        {
            cart = new List<ShoppingCart>();
        }
        foreach (var obj in cart)
        {
            if (obj.MealId == cartToAdd.MealId)
            {
                itemInCart = true;
                obj.Count += cartToAdd.Count;
            }
        }
        if (!itemInCart)
        {
            cart.Add(new ShoppingCart()
            {
                MealId = cartToAdd.MealId,
                Count = cartToAdd.Count
            });
        }
        await _localStorageService.SetItemAsync(Constant.ShoppingCart, cart);
        OnChange?.Invoke();

    }

    public async Task DecrementCart(ShoppingCart cartToDecrement)
    {
        var cart = await _localStorageService.GetItemAsync<List<ShoppingCart>>(Constant.ShoppingCart);

        for (int i = 0; i < cart.Count; i++)
        {
            if (cart[i].MealId == cartToDecrement.MealId)
            {
                if (cart[i].Count == 1 || cartToDecrement.Count == 0)
                {
                    cart.Remove(cart[i]);
                }
                else
                {
                    cart[i].Count -= cartToDecrement.Count;
                }
            }
        }

        await _localStorageService.SetItemAsync(Constant.ShoppingCart, cart);
        OnChange?.Invoke();
    }
}