using BancoSol.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BancoSol.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly ISampleService _sampleService;

    public SampleController(ISampleService sampleService)
    {
        _sampleService = sampleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sampleService.GetAllAsync();
        return Ok(result);
    }
}
