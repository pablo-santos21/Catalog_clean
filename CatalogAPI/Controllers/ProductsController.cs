using AutoMapper;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Catalog.API.Controllers;

[Route("v1/[controller]/")]
[ApiController]
public class ProductsController : Controller
{
    private readonly IRepository<Product> _repository;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProductsController(IRepository<Product> repository, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _mapper = mapper;
        _userManager = userManager;
    }

    /// <summary>
    /// BUSCAR TODAS OS PRODUTOS, PAGINADOS
    /// </summary>
    /// <param name="pageIndex">Qual a página?</param>
    /// <param name="pageSize">Quantos itens quer retornar?</param>
    /// <response code="200">Retorna a lista de produtos, paginada</response>
    /// <response code="400">Parâmetros de paginação inválidos</response>
    /// <response code="404">Não há produtos cadastrados</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("")]
    public async Task<ActionResult<ProductDTO>> GetAsync([FromQuery] int pageIndex = 1, int pageSize = 2)
    {
        
            var products = await _repository.GetAllAsync(pageIndex, pageSize);

            if (products == null)
                return NotFound("Não há produtos cadastrados!");
                
            //var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Ok(products);
        
    }

    /// <summary>
    /// BUSCAR UM PRODUTO
    /// </summary>
    /// <param name="name">Qual a classe deseja procurar?</param>
    /// <response code="200">Retorna um produto</response>
    /// <response code="404">Não há produto cadastrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProductDTO>> GetByIdAsync([FromRoute] long id)
    {

            var product = await _repository.GetAsync(p => p.Id == id);

            if (product is null)
                return NotFound($"O produto {id} não foi encontrado!");

            var productDto = _mapper.Map<ProductDTO>(product);

        return Ok(productDto);

    }

    /// <summary>
    /// CADASTRAR UM PRODUTO
    /// </summary>
    /// <returns></returns>
    /// <response code="201">Produto cadastrado com sucesso</response>
    /// <response code="400">Cadastro não realizado, verifique os dados enviados</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("")]
    [Authorize(Roles = "Admin, SuperAdmin, Seller")]
    public async Task<ActionResult<ProductDTO>> PostAsync([FromBody] ProductDTO productDto)
    {
        var userId =  User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (productDto is null)
                return BadRequest("Dados inválidos!");

        //var product = _mapper.Map<Product>(productDto);
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                Image = productDto.Image,
                IsActive = productDto.IsActive,
                Slug = productDto.Slug,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                CategoryId = productDto.CategoryId,
                UserID = userId
            };

        var productCreated = await _repository.CreateAsync(product);

            var newPRoductCreatedDto = _mapper.Map<ProductDTO>(productCreated);

            return new CreatedAtRouteResult($"O produto {newPRoductCreatedDto.Name} foi criado.\n", newPRoductCreatedDto);
        
    }

    /// <summary>
    /// ATUALIZAR UM PRODUTO
    /// </summary>
    /// <returns></returns>
    /// <param name="id">Qual o Id do produto que deseja atualizar?</param>
    /// <response code="200">Produto atualizado com sucesso</response>
    /// <response code="400">Atualização não realizada, verifique os dados enviados</response>
    /// <response code="404">Produto não foi encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin, SuperAdmin, Seller")]
    public async Task<ActionResult<ProductDTO>> PutAsync(
        [FromRoute] long id,
        [FromBody] ProductDTO productDto)
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id != productDto.Id)
                return BadRequest(new { message = "ID do produto não corresponde." });


            var existingProduct = await _repository.GetAsync(p => p.Id == id);
            if (existingProduct == null)
                return NotFound(new { message = "Produto não encontrado." });


            existingProduct.Name = productDto.Name;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.Stock = productDto.Stock;
            existingProduct.Image = productDto.Image;
            existingProduct.IsActive = productDto.IsActive;
            existingProduct.Slug = productDto.Slug;
            existingProduct.UpdateAt = DateTime.UtcNow; 
            existingProduct.CategoryId = productDto.CategoryId;
            existingProduct.UserID = userId;

            var productUpdate = await _repository.UpdateAsync(existingProduct);

            var updateProductDto = _mapper.Map<ProductDTO>(productUpdate);
                    
            return Ok($"O produto {updateProductDto.Name} foi Editada.");
     
    }

    /// <summary>
    /// EXCLUIR UM PRODUTO
    /// </summary>
    /// <returns></returns>
    /// <param name="id">Qual o Id do produto que deseja excluir?</param>
    /// <response code="200">Produto excluido com sucesso</response>
    /// <response code="204">Exclusão não realizada, verifique os dados enviados</response>
    /// <response code="404">Produto não foi encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin, SuperAdmin, Seller")]
    public async Task<ActionResult<ProductDTO>> DeleteAsync([FromRoute] long id)
    {

            var product = await _repository.GetAsync(p => p.Id == id);

            if (product is null)
                return NotFound("Produto não encontrado");
            
            var productRemove = await _repository.DeleteAsync(product);

            var productRemoveDto = _mapper.Map<ProductDTO>(productRemove);

            return Ok($"O produto, {productRemoveDto.Name}, foi deletado");
        
    }
}
