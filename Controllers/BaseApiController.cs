using DatingApp.Data;
using DatingApp.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    // фиксируем вход юзера если он авторизован
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
    }
}
