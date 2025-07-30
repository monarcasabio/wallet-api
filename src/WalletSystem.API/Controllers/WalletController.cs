using Microsoft.AspNetCore.Mvc;
using WalletSystem.Core.Application.DTOs.Wallet;
using WalletSystem.Core.Application.Interfaces.Services;

namespace WalletSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _service;
    public WalletController(IWalletService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateWalletDto dto, CancellationToken ct)
    {
        var wallet = await _service.CreateWalletAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = wallet.Id }, wallet);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id) => Ok();
}