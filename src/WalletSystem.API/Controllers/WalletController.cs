using Microsoft.AspNetCore.Mvc;
using WalletSystem.Core.Application.DTOs.Wallet;
using WalletSystem.Core.Application.Interfaces.Services;

namespace WalletSystem.API.Controllers;

[ApiController]
[Route("api/wallets")]
public class WalletsController : ControllerBase
{
    private readonly IWalletService _service;
    public WalletsController(IWalletService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateWalletDto dto, CancellationToken ct)
    {
        var wallet = await _service.CreateWalletAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = wallet.Id }, wallet);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var wallet = await _service.GetByIdAsync(id);
        return wallet is null ? NotFound() : Ok(wallet);
    }
}