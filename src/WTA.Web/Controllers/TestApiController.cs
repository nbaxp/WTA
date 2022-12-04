using Microsoft.AspNetCore.Mvc;

namespace WTA.Web.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TestApiController : ControllerBase
  {
    [HttpGet]
    public string Get()
    {
      return "Test Api Controller";
    }
  }
}
