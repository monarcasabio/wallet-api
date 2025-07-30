using Moq;
using WalletSystem.Core.Application.DTOs.Wallet;
using WalletSystem.Core.Application.Interfaces.Repositories;
using WalletSystem.Core.Application.Services;
using WalletSystem.Core.Domain.Entities;
using WalletSystem.Core.Domain.Enums;
using FluentValidation;
using Xunit;
using AutoMapper;
using WalletSystem.Core.Application.Interfaces;
using WalletSystem.Core.Application.DTOs.Movement;

namespace WalletSystem.Tests.Unit;

public class MovementServiceTests
{
    [Fact]
    public async Task GetByWalletAsync_ReturnsEmpty_WhenNoMovements()
    {
        var movementRepoMock = new Mock<IMovementRepository>();
        var walletRepoMock = new Mock<IWalletRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();

        movementRepoMock
            .Setup(r => r.GetByWalletIdAsync(1))
            .ReturnsAsync(Enumerable.Empty<Movement>());

        var sut = new MovementService(
            movementRepoMock.Object,
            walletRepoMock.Object,
            unitOfWorkMock.Object,
            mapperMock.Object);

        var result = await sut.GetByWalletAsync(1);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateMovementAsync_Credit_ReturnsMovementDto()
    {
        var wallet = new Wallet { Id = 1, Balance = 100 };
        var movement = new Movement { Id = 99, WalletId = 1, Amount = 50, Type = MovementType.Credit };

        var walletRepo = new Mock<IWalletRepository>();
        var movementRepo = new Mock<IMovementRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        walletRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(wallet);
        movementRepo.Setup(r => r.AddAsync(It.IsAny<Movement>())).ReturnsAsync(movement);
        unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        mapper.Setup(m => m.Map<Movement>(It.IsAny<CreateMovementDto>())).Returns(movement);
        mapper.Setup(m => m.Map<MovementDto>(It.IsAny<Movement>())).Returns(new MovementDto(99, 1, null, 50, MovementType.Credit, DateTime.UtcNow));

        var sut = new MovementService(
            movementRepo.Object,
            walletRepo.Object,
            unitOfWork.Object,
            mapper.Object);

        var dto = new CreateMovementDto(50, MovementType.Credit);

        var result = await sut.CreateMovementAsync(1, dto);

        Assert.Equal(150, wallet.Balance);
        Assert.Equal(50, result.Amount);
        Assert.Equal(MovementType.Credit, result.Type);
    }
}