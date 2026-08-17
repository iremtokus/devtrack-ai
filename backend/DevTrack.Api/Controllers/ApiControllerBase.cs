using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
namespace DevTrack.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase { protected Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); }
