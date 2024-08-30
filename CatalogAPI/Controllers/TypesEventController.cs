using AutoMapper;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[Route("v1/[controller]/")]
[ApiController]
public class TypesEventController : Controller
{
    private readonly ITypeEventRepository _typeEventRepository;
    private readonly IRepository<TypeEvent> _repository;
    private readonly IMapper _mapper;

    public TypesEventController(IRepository<TypeEvent> repository,
        ITypeEventRepository typeEventRepository,
        IMapper mapper)
    {
        _repository = repository;
        _typeEventRepository = typeEventRepository;
        _mapper = mapper;
    }


    /// <summary>
    /// BUSCAR TODAS OS TIPOS DE EVENTO, PAGINADAOS
    /// </summary>
    /// <param name="pageIndex">Qual a página?</param>
    /// <param name="pageSize">Quantos itens quer retornar?</param>
    /// <response code="200">Retorna a lista de tipos de evento, paginada</response>
    /// <response code="400">Parâmetros de paginação inválidos</response>
    /// <response code="404">Não há categorias cadastradas</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("")]
    public async Task<ActionResult<TypeEventDTO>> GetTypeAsync([FromQuery] int pageIndex = 1, int pageSize = 2)
    {
        
            var types = await _repository.GetAllAsync(pageIndex, pageSize);

            if (types == null)
                return NotFound("Não há tipos de evento cadastrados!");

            //var typesDto = _mapper.Map<IEnumerable<TypeEventDTO>>(types);

            return Ok(types);
    }

    /// <summary>
    /// BUSCAR TODOS OS EVENTOS POR TIPOS DE EVENTO
    /// </summary>
    /// <param name="pageIndex">Qual a página?</param>
    /// <param name="pageSize">Quantos itens quer retornar?</param>
    /// <response code="200">Retorna os tipos de evento com os eventos cadastrados em cada um</response>
    /// <response code="400">Parâmetros de paginação inválidos</response>
    /// <response code="404">Não há categorias cadastradas</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("events")]
    public async Task<ActionResult<TypeEventDTO>> GetAllEventTypeAsync([FromQuery] int pageIndex = 1, int pageSize = 2)
    {
        
            var types = await _typeEventRepository.GetAllEventTypeAsync(pageIndex, pageSize);

            if (types == null)
                return NotFound("Não há tipos de evento cadastrados!");

        return Ok(types);
        
    }

    /// <summary>
    /// BUSCAR UM TIPOS DE EVENTO
    /// </summary>
    /// <param name="name">Qual a classe deseja procurar?</param>
    /// <response code="200">Retorna um tipos de evento</response>
    /// <response code="404">Não há tipos de evento cadastrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{name}")]
    public async Task<ActionResult<TypeEventDTO>> GetATypeAsync([FromRoute] string name)
    {
        var type = await _repository.GetAsync(c => c.Name == name);

        if (type == null)
            return NotFound($"Tipo {name} não foi encontrada");

        var typeDto = _mapper.Map<TypeEventDTO>(type);

        return Ok(typeDto);
    }


    /// <summary>
    /// CADASTRAR UM TIPOS DE EVENTO
    /// </summary>
    /// <returns></returns>
    /// <response code="201">Tipo de evento cadastrado com sucesso</response>
    /// <response code="400">Cadastro não realizado, verifique os dados enviados</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<ActionResult<TypeEventDTO>> PostAsync([FromBody] TypeEventDTO typeEventDto)
    {

        if (typeEventDto is null)
            return BadRequest("Dados inválidos!");

        var typeEvent = _mapper.Map<TypeEvent>(typeEventDto);

        var tipoCriado = await _repository.CreateAsync(typeEvent);

        var TypeCreatedDto = _mapper.Map<TypeEventDTO>(tipoCriado);

        return new CreatedAtRouteResult("Tipo: \n", TypeCreatedDto);

    }

    /// <summary>
    /// ATUALIZAR UM TIPOS DE EVENTO
    /// </summary>
    /// <returns></returns>
    /// <param name="id">Qual o Id do tipos de evento que deseja atualizar?</param>
    /// <response code="200">Tipo de evento atualizado com sucesso</response>
    /// <response code="400">Atualização não realizada, verifique os dados enviados</response>
    /// <response code="404">Tipo de evento não foi encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<ActionResult<TypeEventDTO>> PutAsync(
        [FromRoute] int id,
        [FromBody] TypeEventDTO typeEventDto)
    {
        if(id != typeEventDto.Id)
                return BadRequest(new { message = "O Id nao é igual" });


        var typeEvent = _mapper.Map<TypeEvent>(typeEventDto);

        var updateTypeEvent = await _repository.UpdateAsync(typeEvent);

        var updateTypeEventDto = _mapper.Map<TypeEventDTO>(updateTypeEvent);

        return Ok($"O tipo de evento, {updateTypeEventDto.Name}, foi atualizado");
    }

    /// <summary>
    /// EXCLUIR UM TIPOS DE EVENTO
    /// </summary>
    /// <returns></returns>
    /// <param name="id">Qual o Id do tipos de evento que deseja excluir?</param>
    /// <response code="200">Tipo de evento excluido com sucesso</response>
    /// <response code="204">Exclusão não realizada, verifique os dados enviados</response>
    /// <response code="404">Tipo de evento não foi encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<ActionResult<TypeEventDTO>> DeleteAsync([FromRoute] int id)
    {

        var type = await _repository.GetAsync(c => c.Id == id);

        if (type is null)
            return NotFound($"Tipo não foi encontrada");

        var removeTypeEvent = await _repository.DeleteAsync(type);

        var removeTypeEventDto = _mapper.Map<TypeEventDTO>(removeTypeEvent);

        return Ok($"O tipo {removeTypeEventDto.Name} foi deletada!");

    }
}
