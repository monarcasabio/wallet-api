using Moq;
using WalletSystem.Core.Application.DTOs.Wallet;
using WalletSystem.Core.Application.Interfaces.Repositories;
using WalletSystem.Core.Application.Services;
using WalletSystem.Core.Domain.Entities;
using WalletSystem.Core.Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Xunit;

public class MovementServiceTests
{
    [Fact]
    public async Task GetByWalletAsync_ReturnsEmpty_WhenNoMovements()
    {
        var repoMock = new Mock<IMovementRepository>();
        repoMock.Setup(r => r.GetByWalletIdAsync(1))
            .ReturnsAsync(Enumerable.Empty<Movement>());
        var sut = new MovementService(repoMock.Object);

        var result = await sut.GetByWalletAsync(1);
        Assert.Empty(result);
    }
}