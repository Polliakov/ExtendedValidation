using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Polliakov.ExtendedValidation.Attributes;
using Polliakov.ExtendedValidation.TestApi.Dtos.Requests;

namespace Polliakov.ExtendedValidation.TestApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        [HttpPost]
        public IActionResult TestValidation([FromBody] SomthingRequest request)
        {
            return Ok();
        }
    }
}
