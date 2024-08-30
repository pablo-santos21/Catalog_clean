using AutoMapper;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[Route("v1/[controller]/")]
[ApiController]
public class CategoriesBlogController : Controller
{
    private readonly ICategoryBlogRepository _categoryBlogRepository;
    private readonly IRepository<CategoryBlog> _repository;
    private readonly IMapper _mapper;

    public CategoriesBlogController(IRepository<CategoryBlog> repository,
        ICategoryBlogRepository categoryBlogRepository,
        IMapper mapper)
    {
        _repository = repository;
        _categoryBlogRepository = categoryBlogRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// BUSCAR TODAS AS CATEGORIAS DOS POSTS DO BLOG, PAGINADAS
    /// </summary>
    /// <param name="pageIndex">Qual a página?</param>
    /// <param name="pageSize">Quantos itens quer retornar?</param>
    /// <response code="200">Retorna a lista de categorias do blog, paginada</response>
    /// <response code="400">Parâmetros de paginação inválidos</response>
    /// <response code="404">Não há categorias cadastradas</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("")]
    public async Task<ActionResult<CategoryBlogDTO>> GetCategoryBlogAsync([FromQuery] int pageIndex = 1, int pageSize = 2)
    {

            var categories = await _repository.GetAllAsync(pageIndex, pageSize);

            if (categories == null)
                return NotFound("Não há categorias cadastrados!");

            //var CategoriesDto = _mapper.Map<IEnumerable<CategoryBlogDTO>>(categories);

            return Ok(categories);
        
    }

    /// <summary>
    /// BUSCAR UMA CATEGORIA DE BLOG
    /// </summary>
    /// <param name="name">Qual a classe deseja procurar?</param>
    /// <response code="200">Retorna uma categoria de blog</response>
    /// <response code="404">Não há categorias cadastradas</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{name}")]
    public async Task<ActionResult<CategoryBlogDTO>> GetACategory([FromRoute]string name)
    {

        var category = await _repository.GetAsync(c => c.Name == name);

        if (category == null)
            return NotFound($"Categoria {name} não foi encontrada");

        var CategoriesDto = _mapper.Map<CategoryBlogDTO>(category);

        return Ok(CategoriesDto);

    }

    /// <summary>
    /// BUSCAR TODOS AS CATEGORIA DE BLOG E SEUS POSTS
    /// </summary>
    /// <param name="pageIndex">Qual a página?</param>
    /// <param name="pageSize">Quantos itens quer retornar?</param>
    /// <response code="200">Retorna as categorias com os posts cadastrados em cada uma</response>
    /// <response code="400">Parâmetros de paginação inválidos</response>
    /// <response code="404">Não há categorias cadastradas</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("post")]
    public async Task<ActionResult<CategoryBlogDTO>> GetAllPostsCategoriesAsync([FromQuery] int pageIndex = 1, int pageSize = 2)
    {

        var categories = await _categoryBlogRepository.GetAllPostsCategoriesAsync(pageIndex, pageSize);

        if (categories == null)
            return NotFound("Não há categorias cadastrados!");

        return Ok(categories);


    }

    /// <summary>
    /// CADASTRAR UMA CATEGORIA DE BLOG
    /// </summary>
    /// <returns></returns>
    /// <response code="201">Categoria cadastrada com sucesso</response>
    /// <response code="400">Cadastro não realizado, verifique os dados enviados</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("")]
    [Authorize(Roles = "SuperUser, Admin")]
    public async Task<ActionResult<CategoryBlogDTO>> CreateAsync([FromBody] CategoryBlogDTO categoryBlogDto)
    {

        if (categoryBlogDto is null)
            return BadRequest("Dados inválidos!");

        var categoryBlog = _mapper.Map<CategoryBlog>(categoryBlogDto);

        var createCategory = await _repository.CreateAsync(categoryBlog);

        var categoryCreatedDto = _mapper.Map<CategoryBlogDTO>(createCategory);

        return new CreatedAtRouteResult("Categoria: \n", categoryCreatedDto);

    }

    /// <summary>
    /// ATUALIZAR UMA CATEGORIA DO BLOG
    /// </summary>
    /// <returns></returns>
    /// <param name="id">Qual o Id da categoria que deseja atualizar?</param>
    /// <param name="categoryBlogDto"></param>
    /// <response code="200">Categoria atualizada com sucesso</response>
    /// <response code="400">Atualização não realizada, verifique os dados enviados</response>
    /// <response code="404">Categoria não foi encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:long}")]
    [Authorize(Roles = "SuperUser, Admin")]
    public async Task<ActionResult<CategoryBlogDTO>> PutAsync(
        [FromRoute] long id,
        [FromBody] CategoryBlogDTO categoryBlogDto)
    {

        if (id != categoryBlogDto.Id)
            return BadRequest(new { message = "ID da Categoria nao é igual" });


        var categoryBlog = _mapper.Map<CategoryBlog>(categoryBlogDto);

        var categoryUpdate = await _repository.UpdateAsync(categoryBlog);

        var categoryUpdateDto = _mapper.Map<CategoryBlogDTO>(categoryUpdate);

        return Ok($"Categoria {categoryUpdateDto.Name} foi atualizada");
    }

    /// <summary>
    /// EXCLUIR UMA CATEGORIA DO BLOG
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
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SuperUser, Admin")]
    public async Task<ActionResult<CategoryBlogDTO>> DeleteAsync([FromRoute]long id)
    {

        var category = await _repository.GetAsync(c => c.Id == id);

        if (category is null)
            return NotFound($"Categoria não foi encontrada");

        var removeCategory = await _repository.DeleteAsync(category);

        var removeCategoryDto = _mapper.Map<CategoryBlogDTO>(removeCategory);

        return Ok($"A categoria {removeCategoryDto.Name} foi deletada!");

    }

}
