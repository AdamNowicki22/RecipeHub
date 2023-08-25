﻿using RecipeHub.Domain;

namespace RecipeHub.Infrastructure.Repositories
{
    public interface IRecipeRepository
    {
        Task<IEnumerable<Recipe>> GetRecipes();
        Task<Recipe?> GetRecipe(int id);
        Task AddRecipe(Recipe recipe);
        bool UpdateRecipe(Recipe recipe);
        bool DeleteRecipe(int id);
        Task<IEnumerable<Recipe>> GetRecipesByIngredientIDs(List<int> ingredientIDs);
        
    }
}
