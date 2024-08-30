using System.Security.Claims;

namespace Catalog.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccsessToken(IEnumerable<Claim> claims); //gerar token JWT

    string GenerateRefreshToken(); //gerar refresh do token

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    //extrair a claims do token expirado e gerar o novo token de acesso usando o refresh token
}
