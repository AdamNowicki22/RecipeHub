﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeHub.Domain;
using RecipeHub.DTO_s;
using RecipeHub.Infrastructure;
using RecipeHub.Infrastructure.Repositories;

namespace RecipeHub.Controllers
{
    [Route("api/Recipe")]
    [ApiController]
    public class RecipeController: ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RecipeController(IMapper mapper, IRecipeRepository recipeRepository, ILogger<Recipe> logger)
        {
            _mapper = mapper;
            _recipeRepository = recipeRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<RecipesAllDto>> GetAllRecipes()
        {
            _logger.LogInformation("Getting all recipes");

            var recipes = _recipeRepository.GetRecipes();

            return Ok(recipes);
        }
        
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RecipeDTO>>> GetRecipeByID(int id)
        {
            _logger.LogInformation($"Getting recipe with id {id}");

            var recipe = await _recipeRepository.GetRecipe(id);
            
            if (recipe == null)
            {
                _logger.LogWarning($"Recipe with id {id} wasn't found");
                return NotFound();
            }
            
            var recipeDto = _mapper.Map<IEnumerable<RecipeDTO>>(recipe);
            
            return Ok(recipeDto);
        }

        [HttpGet("recipesbyingredients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RecipeDTO>>> GetRecipesByIngredientIDs([FromQuery] List<int> ingredientIDs)
        {
            if (ingredientIDs == null || ingredientIDs.Count == 0)
            {
                return BadRequest("No ingredient IDs provided.");
            }

            var recipes = await _recipeRepository.GetRecipesByIngredientIDs(ingredientIDs);

            var recipesDto = _mapper.Map<IEnumerable<RecipeByIngredientsDTO>>(recipes);
            
            return Ok(recipesDto);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddRecipe([FromBody] RecipeForAddDto recipeForAddDto)
        {
            _logger.LogInformation("Adding new recipe");

            if (string.IsNullOrWhiteSpace(recipeForAddDto.Name))
            {
                ModelState.AddModelError("emptyName", "Name cannot be empty!");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Adding recipe failed because of bad request");

                return BadRequest(ModelState);
            }

            var recipe = _mapper.Map<Recipe>(recipeForAddDto);

            _recipeRepository.AddRecipe(recipe);

            _logger.LogInformation($"New recipe added with id {recipe.Id}");


            return CreatedAtAction(nameof(GetAllRecipes),
                new { id = recipe.Id }, recipe);

        }

    }
}
