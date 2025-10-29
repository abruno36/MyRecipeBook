using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Communication.Requests;      
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Application.UseCases.User.Register;

namespace MyRecipeBook.API.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IRegisterUserUseCase _registerUserUseCase;

    public UserController(IRegisterUserUseCase registerUserUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RequestRegisterUserJson request)
    {
        var result = await _registerUserUseCase.Execute(request);

        return Created(string.Empty, result);
    }
}
