﻿using Microsoft.EntityFrameworkCore;
using RecipeHub.Domain;

namespace RecipeHub.Infrastructure.Repositories
{
	public class UserFavouriteRecipesRepository : IUserFavouriteRecipesRepository
	{
        private readonly RecipeDBContext _dbContext;

        public UserFavouriteRecipesRepository(RecipeDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Recipe>> GetUserFavouriteRecipesAsync(string userId)
        {
            var user = await _dbContext.Users
                .Include(u => u.FavouriteRecipes)
                .ThenInclude(uf => uf.Recipe)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.FavouriteRecipes.Select(uf => uf.Recipe).ToList() ?? new List<Recipe>();
        }

        public async Task<bool> AddRecipeToUserFavouritesAsync(string userId, int recipeId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            var recipe = await _dbContext.Recipes.FindAsync(recipeId);

            var favouriteRecipe = new UserFavouriteRecipe
            {
                User = user,
                Recipe = recipe,
            };

            await _dbContext.UserFavouriteRecipes.AddAsync(favouriteRecipe);
            await _dbContext.SaveChangesAsync();

            return false;
        }

        public async Task<bool> DeleteRecipeFromUserFavouritesAsync(string userId, int recipeId)
        {
            var user = await _dbContext.Users
                .Include(u => u.FavouriteRecipes)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                var userFavourite = user.FavouriteRecipes.FirstOrDefault(uf => uf.RecipeId == recipeId);

                if (userFavourite != null)
                {
                    user.FavouriteRecipes.Remove(userFavourite);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

    }
}