using Microsoft.AspNetCore.Mvc;
using WalletSystem.Core.Application.DTOs.Movement;
using WalletSystem.Core.Application.Interfaces.Services;

namespace WalletSystem.API.Controllers;

[ApiController]
[Route("api/wallets/{walletId:int}/movements")]
public class MovementsController : ControllerBase
{
    private readonly IMovementService _service;
    public MovementsController(IMovementService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetByWallet(int walletId)
        => Ok(await _service.GetByWalletAsync(walletId));

    [HttpPost]
    public async Task<IActionResult> CreateMovement(int walletId, [FromBody] CreateMovementDto dto)
    {
        var movement = await _service.CreateMovementAsync(walletId, dto);
        return CreatedAtAction(nameof(GetByWallet), new { walletId }, movement);
    }
}