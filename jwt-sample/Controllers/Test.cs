
using jwt_sample.Api.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace jwt_sample.Controllers;


[Route("[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    readonly IToken _token;
    public TestController(IToken token)
    {
        _token = token;
    }

    [HttpGet]
    public ActionResult<string> Get()
    {
        return "Hello World!";
    }

    [HttpGet("GetToken/{userid}")]
    public IActionResult GetToken(string userid) => Ok(_token.GetToken(userid));

    [Authorize(Roles = "USER")]
    [HttpGet("hello")]
    public IActionResult Hello() => Ok("Hello World! USER!");

}



