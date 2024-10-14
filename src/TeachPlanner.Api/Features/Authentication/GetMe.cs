using System.IdentityModel.Tokens.Jwt;
using MediatR;

namespace TeachPlanner.Api.Features.Authentication;

public static class GetMe
{
  public static async ValueTask<IResult> Endpoint(HttpContext http, ISender sender)
  {
    var tokenString = http.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
    if (string.IsNullOrWhiteSpace(tokenString))
    {
      return Results.Unauthorized();
    }

    var handler = new JwtSecurityTokenHandler();
    var token = handler.ReadToken(tokenString) as JwtSecurityToken;
    var claims = token?.Claims.Select(c => new { c.Type, c.Value }).ToList();

    var expiresOn = token?.ValidTo;
    if (expiresOn?.CompareTo(DateTime.UtcNow) > 0)
    {
      return Results.Ok();
    }

    return Results.Unauthorized();
  }
}