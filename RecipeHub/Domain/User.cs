using Microsoft.AspNetCore.Identity;

namespace RecipeHub.Domain
{
    public class User: IdentityUser
    {
        public ICollection<UserFavouriteRecipe> FavouriteRecipes { get; set; }
    }
}
