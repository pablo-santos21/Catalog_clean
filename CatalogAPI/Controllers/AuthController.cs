using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Catalog.Application.Interfaces;
using Catalog.Application.DTOs;
using Catalog.Infrastructure.Identity;

namespace Catalog.API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    //[ApiConventionType(typeof(DefaultApiConventions))]
    //API Convertion serve para documentar todos os endponts com os status code padrão
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<IdentityApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ITokenService tokenService,
                            UserManager<IdentityApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            IConfiguration configuration,
                            ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// CADASTRAR ROLES
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Roles cadastrada com sucesso</response>
        /// <response code="400">Role ja existente</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [Route("createRole")]
        [Authorize(Roles = "SuperUser, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation(1, "Roles Added");
                    return StatusCode(StatusCodes.Status201Created,
                        new Response { StatusCode = "success", Message = $"Role {roleName}, adicionada com sucesso!" });
                }
                else
                {
                    _logger.LogInformation(2, "Error");
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { StatusCode = "Error", Message = $"Problema ao adicionar nova role {roleName}, verifique os dados." });
                }
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { StatusCode = "Error", Message = "Role ja existe" });
        }

        /// <summary>
        /// CADASTRAR O USUÁRIO EM UMA ROLE
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Usuário cadastrado na role com sucesso</response>
        /// <response code="400">Usuário já cadastrado na role</response>
        /// <response code="500">Erro interno do servidor</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("addUserToRole")]
        //[Authorize(Roles = "SuperUser, admin")]
        public async Task<IActionResult> AddUserToRole(string email ,string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.AddToRolesAsync(user, new[] { roleName });

                if (result.Succeeded)
                {
                    _logger.LogInformation(1, $"User {user.Email} foi adicionado na role {roleName}");
                    return StatusCode(StatusCodes.Status200OK,
                        new Response { StatusCode = "success", Message = $"User {user.Email} foi adicionado na role {roleName}" });
                }
                else
                {
                    _logger.LogInformation(2, $"Error: Não foi possivel adicionar o user {user.Email} na role {roleName}");
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { StatusCode = "Error", Message = $"Error: Não foi possivel adicionar o user {user.Email} na role {roleName}" });
                }
            }
            return BadRequest(new { error = "Não foi possível encontrar o usuário" });
        }

        /// <summary>
        /// REALIZAR O LOGIN
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Login Realizado com sucesso</response>
        /// <response code="400">Dados são obrigatórios</response>
        /// <response code="401">Cadastro não realizado, verifique os dados enviados</response>
        /// <response code="500">Erro interno do servidor</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            // Verifica se os dados de login são válidos
            if (string.IsNullOrWhiteSpace(login.UserName) || string.IsNullOrWhiteSpace(login.Password))
            {
                return BadRequest("Nome de usuário e senha são obrigatórios.");
            }

            // Busca o usuário pelo nome de usuário
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user is null || !await _userManager.CheckPasswordAsync(user, login.Password))
            {
                return Unauthorized("Nome de usuário ou senha incorretos.");
            }

            // Obtém as roles do usuário
            var userRoles = await _userManager.GetRolesAsync(user);

            // Cria uma lista de claims
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("id", user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // Gera o token de acesso
            var token = _tokenService.GenerateAccsessToken(authClaims, _configuration);
            var refreshToken = _tokenService.GenerateRefreshToken();

            if (!int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes))
            {
                refreshTokenValidityInMinutes = 1440; // Valor padrão, caso falhe a leitura da configuração
            }

            // Atualiza o usuário com o novo refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                // Lida com falha na atualização do usuário
                return StatusCode(StatusCodes.Status500InternalServerError, "Falha ao atualizar o usuário.");
            }

            // Retorna o token JWT e o refresh token
            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
            //return Ok($"login feito por {user.UserName} de id {user.Id}");
        }

        /// <summary>
        /// CADASTRAR O USUÁRIO
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Usuário cadastrado com sucesso!</response>
        /// <response code="500">Erro interno do servidor / Usuário já existe!</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO user)
        {
            var userExists = await _userManager.FindByNameAsync(user.UserName!);

            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { StatusCode = "Error", Message = "Usuário ja existe!" });
            }

            IdentityApplicationUser usuario = new()
            {
                Email = user.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = user.UserName
            };

            var result = await _userManager.CreateAsync(usuario, user.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { StatusCode = "Error", Message = "Falhou a criação do usuário!" });
            }


            return Ok(new Response { StatusCode = "Success", Message = "Usuário cadastrado com sucesso!" });
        }

        /// <summary>
        /// REVALIDAR TOKEN DE ACESSO
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Token revalidado com sucesso</response>
        /// <response code="401">Dados inválidos, verifique os dados enviados.</response>
        /// <response code="500">Erro interno do servidor</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("refresh-token")]
        [Authorize]

        public async Task<IActionResult> RefreshToken(TokenDTO tokenDto)
        {
            if (tokenDto is null)
            {
                return BadRequest("Request de cliente inválido!");
            }

            string? accessToken = tokenDto.AccessToken
                    ?? throw new ArgumentNullException(nameof(tokenDto));

            string? refreshToken = tokenDto.RefreshToken
                    ?? throw new ArgumentNullException(nameof(tokenDto));

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);

            if (principal == null)
                return BadRequest("Invalid access token/refresh token!");

            string username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username!);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid access token/refresh token!");

            var newAccessToken = _tokenService.GenerateAccsessToken(principal.Claims.ToList(), _configuration);

            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken,
            });
        }

        /// <summary>
        /// REVOGAR O TOKEN DE ACESSO DO USUÁRIO.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Token revogado com sucesso</response>
        /// <response code="400">Usuário nao encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "SuperUser, admin")]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return BadRequest("Invalid user name");

            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        
    }
}
