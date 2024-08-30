using AutoMapper;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Catalog.API.Controllers;

[Route("v1/[controller]/")]
[ApiController]
public class PostsController : Controller
{

    private readonly IRepository<PostBlog> _repository;
    private readonly IMapper _mapper;

    public PostsController(IRepository<PostBlog> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }


    /// <summary>
    /// BUSCAR TODAS OS POSTS, PAGINADOS
    /// </summary>
    /// <param name="pageIndex">Qual a página?</param>
    /// <param name="pageSize">Quantos itens quer retornar?</param>
    /// <response code="200">Retorna a lista de posts, paginada</response>
    /// <response code="400">Parâmetros de paginação inválidos</response>
    /// <response code="404">Não há posts cadastrados</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("")]
    public async Task<ActionResult<PostDTO>> GetAsync([FromQuery] int pageIndex = 1, int pageSize = 2)
    {
        
            var posts = await _repository.GetAllAsync(pageIndex, pageSize);

            if (posts == null)
                return Ok("Não há produtos cadastrados!");

            //var postsDto = _mapper.Map<IEnumerable<PostDTO>>(posts);

            return Ok(posts);
        
    }

    /// <summary>
    /// BUSCAR UM POST
    /// </summary>
    /// <param name="name">Qual a classe deseja procurar?</param>
    /// <response code="200">Retorna um post</response>
    /// <response code="404">Não há post cadastrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:long}")]
    public async Task<ActionResult<PostDTO>> GetAPostAsync([FromRoute]long id)
    {
        
            var post = await _repository.GetAsync(x => x.Id == id);

            if (post is null)
                return NotFound($"O produto {id} não foi encontrado!");

            var postDto = _mapper.Map<PostDTO>(post);

            return Ok(post);
        
    }


    /// <summary>
    /// CADASTRAR UM POST
    /// </summary>
    /// <returns></returns>
    /// <response code="201">Post cadastrado com sucesso</response>
    /// <response code="400">Cadastro não realizado, verifique os dados enviados</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("")]
    [Authorize(Roles = "Admin, SuperAdmin, seller")]
    public async Task<ActionResult<PostDTO>> PostAsync([FromBody]PostDTO postDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (postDto is null)
                return BadRequest("Dados inválidos!");

            var post =  new PostBlog
            {
                Title = postDto.Title,
                SubTitle = postDto.SubTitle,
                Context = postDto.Context,
                IsApproved = postDto.IsApproved,
                IsDeleted = postDto.IsDeleted,
                ImagePost = postDto.ImagePost,
                Slug = postDto.Slug,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                Tags = postDto.Tags,
                CategoryBlogId = postDto.CategoryBlogId,
                UserID = userId
            };

            var postCreated = await _repository.CreateAsync(post);

            var postCreatedDto = _mapper.Map<PostDTO>(postCreated);

            return new CreatedAtRouteResult($"Post {postCreatedDto.Title} criadado com sucesso!", postCreatedDto);
        
    }


    /// <summary>
    /// ATUALIZAR UM POST
    /// </summary>
    /// <returns></returns>
    /// <param name="postDto">Qual o Id do post que deseja atualizar?</param>
    /// <response code="200">Post atualizada com sucesso</response>
    /// <response code="400">Atualização não realizada, verifique os dados enviados</response>
    /// <response code="404">Post não foi encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin, SuperAdmin, seller")]
    public async Task<ActionResult<PostDTO>> PutAsync(
        [FromRoute]long id,
        [FromBody] PostDTO postDto)
    {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id != postDto.Id)
                return BadRequest(new { message = "Categoria nao é igual" });

            var existingPost = await _repository.GetAsync(p => p.Id == id);
            if (existingPost == null)
                return NotFound(new { message = "Produto não encontrado." });

            existingPost.Title = postDto.Title;
            existingPost.SubTitle = postDto.SubTitle;
            existingPost.Context = postDto.Context;
            existingPost.IsApproved = postDto.IsApproved;
            existingPost.ImagePost = postDto.ImagePost;
            existingPost.IsDeleted = postDto.IsDeleted;
            existingPost.Slug = postDto.Slug;
            existingPost.UpdateAt = DateTime.UtcNow;
            existingPost.Tags = postDto.Tags;
            existingPost.CategoryBlogId = postDto.CategoryBlogId;
            existingPost.UserID = userId;

            var updatePost = await _repository.UpdateAsync(existingPost);

            var updatePostDto = _mapper.Map<PostDTO>(updatePost);

            return Ok($"O produto {updatePostDto.Title} foi Editada.");
        
    }


    /// <summary>
    /// EXCLUIR UM POST
    /// </summary>
    /// <returns></returns>
    /// <param name="id">Qual o Id do post que deseja excluir?</param>
    /// <response code="200">Post excluido com sucesso</response>
    /// <response code="204">Exclusão não realizada, verifique os dados enviados</response>
    /// <response code="404">Post não foi encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin, SuperAdmin, seller")]
    public async Task<ActionResult<PostDTO>> DeleteAsync([FromRoute]long id)
    {

            var post = await _repository.GetAsync(x => x.Id == id);

            if (post is null)
                return NotFound("Produto não encontrado");

            var postRemove = await _repository.DeleteAsync(post);

            var postRemoveDto = _mapper.Map<PostDTO>(postRemove);

            return Ok($"O post {postRemoveDto.Title} foi excluído.");
        
    }
}
