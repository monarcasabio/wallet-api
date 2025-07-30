using Moq;
using WalletSystem.Core.Application.DTOs.Wallet;
using WalletSystem.Core.Application.Interfaces.Repositories;
using WalletSystem.Core.Application.Services;
using WalletSystem.Core.Domain.Entities;
using WalletSystem.Core.Domain.Exceptions;
using FluentValidation;         
using FluentValidation.Results;
using Xunit;

namespace WalletSystem.Tests.Unit;

public class WalletServiceTests
{
    [Fact]
    public async Task CreateWalletAsync_ReturnsWallet()
    {
        // Arrange
        var dto = new CreateWalletDto("A123", "Test");
        var repoMock = new Mock<IWalletRepository>();
        repoMock.Setup(r => r.AddAsync(It.IsAny<Wallet>()))
                .ReturnsAsync((Wallet w) => w);

        var sut = new WalletService(repoMock.Object);

        // Act
        var result = await sut.CreateWalletAsync(dto);

        // Assert
        Assert.Equal("A123", result.DocumentId);
        Assert.Equal("Test", result.Name);
        Assert.Equal(0, result.Balance);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var repoMock = new Mock<IWalletRepository>();
        repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Wallet?)null);
        var sut = new WalletService(repoMock.Object);

        var result = await sut.GetByIdAsync(99);
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateWalletNameAsync_Throws_WhenNotFound()
    {
        var repoMock = new Mock<IWalletRepository>();
        repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Wallet?)null);
        var sut = new WalletService(repoMock.Object);

        await Assert.ThrowsAsync<WalletNotFoundException>(
            () => sut.UpdateWalletNameAsync(99, "New"));
    }

    [Fact]
    public async Task DeactivateWalletAsync_Throws_WhenNotFound()
    {
        var repoMock = new Mock<IWalletRepository>();
        repoMock.Setup(r => r.DeactivateAsync(99))
                .ThrowsAsync(new WalletNotFoundException(99));
        var sut = new WalletService(repoMock.Object);

        await Assert.ThrowsAsync<WalletNotFoundException>(
            () => sut.DeactivateWalletAsync(99));
    }
}