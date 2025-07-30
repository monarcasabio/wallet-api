using Moq;
using WalletSystem.Core.Application.DTOs.Wallet;
using WalletSystem.Core.Application.Interfaces.Repositories;
using WalletSystem.Core.Application.Services;
using WalletSystem.Core.Domain.Entities;
using WalletSystem.Core.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using Xunit;
using WalletSystem.Core.Application.DTOs.Movement;

namespace WalletSystem.Tests.Unit;

public class MovementServiceTests
{
    [Fact]
    public async Task GetByWalletAsync_ReturnsEmpty_WhenNoMovements()
    {
        var movementRepoMock = new Mock<IMovementRepository>(); 
        var walletRepoMock = new Mock<IWalletRepository>(); 

        movementRepoMock.Setup(r => r.GetByWalletIdAsync(1))
            .ReturnsAsync(Enumerable.Empty<Movement>());

        var sut = new MovementService(movementRepoMock.Object, walletRepoMock.Object);

        var result = await sut.GetByWalletAsync(1);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateMovementAsync_Credit_ReturnsMovementDto()
    {
        var wallet = new Wallet { Id = 1, Balance = 100 };
        var walletRepo = new Mock<IWalletRepository>();
        var movementRepo = new Mock<IMovementRepository>();
        walletRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(wallet);
        movementRepo.Setup(r => r.AddAsync(It.IsAny<Movement>()))
                    .ReturnsAsync((Movement m) => m);

        var sut = new MovementService(movementRepo.Object, walletRepo.Object);
        var dto = new CreateMovementDto(50, MovementType.Credit);

        var result = await sut.CreateMovementAsync(1, dto);

        Assert.Equal(150, wallet.Balance);
        Assert.Equal(50, result.Amount);
        Assert.Equal(MovementType.Credit, result.Type);
    }
}