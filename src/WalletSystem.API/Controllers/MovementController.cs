using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletSystem.Core.Application.DTOs.Movement;
using WalletSystem.Core.Application.Interfaces.Services;

namespace WalletSystem.API.Controllers;

[ApiController]
[Route("api/wallets/")]
public class MovementsController : ControllerBase
{
    private readonly IMovementService _service;
    private readonly IMapper _mapper;
    public MovementsController(IMovementService service, IMapper mapper) 
    { 
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("{walletId:int}/movements")]
    public async Task<IActionResult> GetByWallet(int walletId)
    {
        var movements = await _service.GetByWalletAsync(walletId);
        return Ok(_mapper.Map<IEnumerable<MovementDto>>(movements));
    }

    [Authorize]
    [HttpPost("movements")]
    public async Task<IActionResult> CreateMovement(int walletId, [FromBody] CreateMovementDto dto)
    {
        var movement = await _service.CreateMovementAsync(walletId, dto);
        return CreatedAtAction(nameof(GetByWallet), new { walletId }, movement);
    }

    [Authorize]
    [HttpPost("{fromWalletId:int}/transfer")]
    public async Task<IActionResult> Transfer(int fromWalletId, [FromBody] TransferDto dto)
    {
        await _service.TransferAsync(fromWalletId, dto);
        return NoContent();
    }
}