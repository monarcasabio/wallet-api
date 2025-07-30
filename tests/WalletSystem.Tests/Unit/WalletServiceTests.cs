using Moq;
using WalletSystem.Core.Application.DTOs.Wallet;
using WalletSystem.Core.Application.Services;
using WalletSystem.Core.Domain.Entities;
using WalletSystem.Core.Domain.Exceptions;
using FluentValidation;
using Xunit;
using AutoMapper;
using WalletSystem.Core.Application.Interfaces;
using WalletSystem.Core.Application.DTOs.Movement;

namespace WalletSystem.Tests.Unit;

public class WalletServiceTests
{
    [Fact]
    public async Task CreateWalletAsync_ReturnsWalletDto()
    {
        var dto = new CreateWalletDto("A123", "Test");
        var wallet = new Wallet { Id = 1, DocumentId = "A123", Name = "Test", Balance = 0, IsActive = true };

        var repo = new Mock<IWalletRepository>();
        var mapper = new Mock<IMapper>();
        var uow = new Mock<IUnitOfWork>();

        mapper.Setup(m => m.Map<Wallet>(dto)).Returns(wallet);
        mapper.Setup(m => m.Map<WalletDto>(wallet)).Returns(new WalletDto(1, "A123", "Test", 0, DateTime.UtcNow, DateTime.UtcNow, true));
        repo.Setup(r => r.AddAsync(wallet)).ReturnsAsync(wallet);

        var sut = new WalletService(repo.Object, mapper.Object, uow.Object);

        var result = await sut.CreateWalletAsync(dto);

        Assert.Equal("A123", result.DocumentId);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var repo = new Mock<IWalletRepository>();
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Wallet?)null);

        var sut = new WalletService(repo.Object, Mock.Of<IMapper>(), Mock.Of<IUnitOfWork>());

        var result = await sut.GetByIdAsync(99);
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateWalletNameAsync_Throws_WhenNotFound()
    {
        var repo = new Mock<IWalletRepository>();
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Wallet?)null);

        var sut = new WalletService(repo.Object, Mock.Of<IMapper>(), Mock.Of<IUnitOfWork>());

        await Assert.ThrowsAsync<WalletNotFoundException>(
            () => sut.UpdateWalletNameAsync(99, "New"));
    }

    [Fact]
    public async Task DeactivateWalletAsync_Throws_WhenNotFound()
    {
        var repo = new Mock<IWalletRepository>();
        repo.Setup(r => r.DeactivateAsync(99)).ThrowsAsync(new WalletNotFoundException(99));

        var sut = new WalletService(repo.Object, Mock.Of<IMapper>(), Mock.Of<IUnitOfWork>());

        await Assert.ThrowsAsync<WalletNotFoundException>(
            () => sut.DeactivateWalletAsync(99));
    }
}