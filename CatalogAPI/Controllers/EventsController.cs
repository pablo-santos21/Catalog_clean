using AutoMapper;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[Route("v1/[controller]/")]
[ApiController]
public class EventsController : Controller
{
    private readonly IRepository<ScheduledEvent> _repository;
    private readonly IMapper _mapper;

    public EventsController(IRepository<ScheduledEvent> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// BUSCAR TODAS OS EVENTOS, PAGINADOS
    /// </summary>
    /// <param name="pageIndex">Qual a página?</param>
    /// <param name="pageSize">Quantos itens quer retornar?</param>
    /// <response code="200">Retorna a lista de eventos, paginada</response>
    /// <response code="400">Parâmetros de paginação inválidos</response>
    /// <response code="404">Não há eventos cadastrados</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("")]
    public async Task<ActionResult<EventDTO>> GetAsync([FromQuery] int pageIndex = 1, int pageSize = 2)
    {
       
            var events = await _repository.GetAllAsync(pageIndex, pageSize);

            if (events == null)
                return Ok("Não há eventos cadastrados!");

            //var eventsDto = _mapper.Map<IEnumerable<ScheduledEvent>>(events);

            return Ok(events);
       
    }


    /// <summary>
    /// BUSCAR UM EVENTO
    /// </summary>
    /// <param name="name">Qual a classe deseja procurar?</param>
    /// <response code="200">Retorna um evento</response>
    /// <response code="404">Não há evento cadastrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:long}")]
    public async Task<ActionResult<EventDTO>> GetByIdAsync([FromRoute]long id)
    {
            var getEvent = await _repository.GetAsync(e => e.Id == id);

            if (getEvent == null)
                return NotFound("Evento não encontrados");

            var eventDto = _mapper.Map<EventDTO>(getEvent);
            

            return Ok(eventDto);
             
    }

    /// <summary>
    /// CADASTRAR UM EVENTO
    /// </summary>
    /// <returns></returns>
    /// <response code="201">Evento cadastrado com sucesso</response>
    /// <response code="400">Cadastro não realizado, verifique os dados enviados</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<ActionResult<EventDTO>> PostAsync(
        [FromBody] EventDTO eventDto)
    {

            if (eventDto is null)
                return BadRequest("Dados inválidos!");

            var events = _mapper.Map<ScheduledEvent>(eventDto);
            
            var eventCreated = await _repository.CreateAsync(events);

            var eventCretedDto = _mapper.Map<EventDTO>(eventCreated);

            return new CreatedAtRouteResult($"O evento {eventCretedDto.Title} foi criado. \n", eventCretedDto);
                      
    }

    /// <summary>
    /// ATUALIZAR UM EVENTO
    /// </summary>
    /// <returns></returns>
    /// <param name="id">Qual o Id do evento que deseja atualizar?</param>
    /// <response code="200">Evento atualizado com sucesso</response>
    /// <response code="400">Atualização não realizada, verifique os dados enviados</response>
    /// <response code="404">Evento não foi encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<ActionResult<EventDTO>> PutAsync(
        [FromRoute]long id, 
        [FromBody] EventDTO eventDTO)
    {

            if (id != eventDTO.Id)
                return BadRequest(new { message = "ID da Categoria nao é igual." });

            var events = _mapper.Map<ScheduledEvent>(eventDTO);

            var getEvent = await _repository.UpdateAsync(events);

            var getEventDto = _mapper.Map<EventDTO>(getEvent);

            return Ok($"O evento {getEventDto.Title} foi atualizada");
        
    }

    /// <summary>
    /// EXCLUIR UM EVENTO
    /// </summary>
    /// <returns></returns>
    /// <param name="id">Qual o Id do evento que deseja excluir?</param>
    /// <response code="200">Evento excluido com sucesso</response>
    /// <response code="204">Exclusão não realizada, verifique os dados enviados</response>
    /// <response code="404">Evento não foi encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<ActionResult<EventDTO>> DeleteAsync([FromRoute]long id)
    {

        
            var getEvent = await _repository.GetAsync(x => x.Id == id);

            if (getEvent == null)
                return NotFound("O evento não foi encontrado");

            var eventRemove = await _repository.DeleteAsync(getEvent);

            var eventRemoveDto = _mapper.Map<EventDTO>(eventRemove);

            return Ok($"O evento {eventRemoveDto.Title} foi apagado");
        
    }

}
