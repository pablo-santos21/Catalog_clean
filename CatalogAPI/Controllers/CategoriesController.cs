using AutoMapper;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[Route("v1/[controller]/")]
[ApiController]
public class CategoriesController : Controller
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IRepository<Category> _repository;
    private readonly IMapper _mapper;

    public CategoriesController(IRepository<Category> repository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// BUSCAR TODAS AS CATEGORIAS PAGINADAS
    /// </summary>
    /// <param name="pageIndex">Qual a página?</param>
    /// <param name="pageSize">Quantos itens quer retornar?</param>
    /// <response code="200">Retorna a lista de categorias, paginada</response>
    /// <response code="400">Parâmetros de paginação inválidos</response>
    /// <response code="401">Token não autorizado</response>
    /// <response code="404">Não há categorias cadastradas</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("")]
    public async Task<ActionResult<CategoryDTO>> GetCategoriesAsync([FromQuery] int pageIndex = 1, int pageSize = 2)
    {
        
        var categories = await _repository.GetAllAsync(pageIndex, pageSize);

        if ( categories == null)
            return NotFound("Não há categorias cadastrados!");
        

        return Ok(categories);
       
    }

    /// <summary>
    /// BUSCAR UMA CATEGORIA
    /// </summary>
    /// <param name="name">Qual a classe deseja procurar?</param>
    /// <response code="200">Retorna uma categoria</response>
    /// <response code="404">Não há categorias cadastradas</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{name}")]
    public async Task<ActionResult<CategoryDTO>> GetCategoryAsync([FromRoute] string name)
    {
        var category = await _repository.GetAsync(c => c.Name == name);

        if (category == null)
            return NotFound($"Categoria {name} não foi encontrada");

        var categoriesDto = _mapper.Map<CategoryDTO>(category);

        return Ok(category);
    }

    /// <summary>
    /// BUSCAR TODOS OS PRODUTOS POR CATEGORIA
    /// </summary>
    /// <param name="pageIndex">Qual a página?</param>
    /// <param name="pageSize">Quantos itens quer retornar?</param>
    /// <response code="200">Retorna as categorias com os produtos cadastrados em cada uma</response>
    /// <response code="400">Parâmetros de paginação inválidos</response>
    /// <response code="404">Não há categorias cadastradas</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("products")]
    public async Task<ActionResult<CategoryDTO>> GetAllProductsCategoriesAsync([FromQuery] int pageIndex = 1, int pageSize = 2)
    {

        var categories = await _categoryRepository.GetAllProductsCategoriesAsync(pageIndex, pageSize);

        if (categories == null)
            return NotFound("Não há categorias cadastrados!");

        return Ok(categories);

    }

    /// <summary>
    /// CADASTRAR UMA CATEGORIA
    /// </summary>
    /// <returns></returns>
    /// <response code="201">Categoria cadastrada com sucesso</response>
    /// <response code="400">Cadastro não realizado, verifique os dados enviados</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<ActionResult<CategoryDTO>> CreateAsync([FromBody] CategoryDTO categoryDto)
    {

        if (categoryDto is null)
            return BadRequest("Dados inválidos!");

        var category = _mapper.Map<Category>(categoryDto);

        var categoryCreate = await _repository.CreateAsync(category);

        var categoryCreateDto = _mapper.Map<CategoryDTO>(categoryCreate);

        return new CreatedAtRouteResult("Categoria: \n", categoryCreate);


    }

    /// <summary>
    /// ATUALIZAR UMA CATEGORIA
    /// </summary>
    /// <returns></returns>
    /// <param name="id">Qual o Id da categoria que deseja atualizar?</param>
    /// <response code="200">Categoria atualizada com sucesso</response>
    /// <response code="400">Atualização não realizada, verifique os dados enviados</response>
    /// <response code="404">Categoria não foi encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<ActionResult<CategoryDTO>> PutAsync(
        [FromRoute] int id,
        [FromBody] CategoryDTO categoryDto)
    {
       
        if (id != categoryDto.Id)
            return BadRequest(new { message = "ID da Categoria nao é igual" });


        var category = _mapper.Map<Category>(categoryDto);

        var updateCategory = await _repository.UpdateAsync(category);

        var updateCategoryDto = _mapper.Map<CategoryDTO>(updateCategory);

        return Ok($"Categoria {updateCategoryDto.Name} foi atualizada");
    }

    /// <summary>
    /// EXCLUIR UMA CATEGORIA
    /// </summary>
    /// <returns></returns>
    /// <param name="id">Qual o Id da categoria que deseja excluir?</param>
    /// <response code="200">Categoria excluida com sucesso</response>
    /// <response code="204">Exclusão não realizada, verifique os dados enviados</response>
    /// <response code="404">Categoria não foi encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<ActionResult<CategoryDTO>> DeleteAsync([FromRoute] int id)
    {
        var category = await _repository.GetAsync(c => c.Id == id);

        if (category is null)
            return NotFound($"Categoria não foi encontrada");

        var removeCategory = await _repository.DeleteAsync(category);

        var removeCategoryDto = _mapper.Map<CategoryDTO>(removeCategory);

        return Ok($"A categoria {removeCategoryDto.Name} foi deletada!");
    }
}
