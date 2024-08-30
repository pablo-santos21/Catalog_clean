using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[Route("v1/health-check")]
[ApiController]
public class HealthCheck : ControllerBase
{
    /// <summary>
    /// TESTE DE CHECAGEM DE CONEXÃO
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Get()
    {
       return StatusCode(StatusCodes.Status200OK, "API Retornada com sucesso!");
    }
}
