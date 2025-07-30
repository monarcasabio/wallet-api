using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Wallet.Core.Application.DTOs.Transfer;
using Wallet.Core.Application.DTOs.Wallet;
using Wallet.Core.Domain.Entities;
using Wallet.Core.Domain.Enums;
using Wallet.Infrastructure.Data;
using Xunit;

namespace Wallet.IntegrationTests;

public class TransferIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TransferIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<WalletSystemDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database for testing
                services.AddDbContext<WalletSystemDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Build service provider
                var sp = services.BuildServiceProvider();

                // Create scope and ensure database is created
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<WalletSystemDbContext>();
                db.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Transfer_CompleteFlow_UpdatesBothWalletsAndCreatesMovements()
    {
        // Arrange - Create source wallet with balance
        var createSourceDto = new CreateWalletDto("DOC001", "John Doe");
        var sourceResponse = await _client.PostAsJsonAsync("/api/wallets", createSourceDto);
        sourceResponse.EnsureSuccessStatusCode();
        var sourceWallet = await sourceResponse.Content.ReadFromJsonAsync<WalletDto>();

        // Add initial balance to source wallet via credit movement
        var creditDto = new CreateMovementDto(1000m, MovementType.Credit);
        await _client.PostAsJsonAsync($"/api/wallets/{sourceWallet!.Id}/movements", creditDto);

        // Create destination wallet
        var createDestDto = new CreateWalletDto("DOC002", "Jane Smith");
        var destResponse = await _client.PostAsJsonAsync("/api/wallets", createDestDto);
        destResponse.EnsureSuccessStatusCode();
        var destWallet = await destResponse.Content.ReadFromJsonAsync<WalletDto>();

        // Act - Perform transfer
        var transferDto = new TransferDto { ToWalletId = destWallet!.Id, Amount = 250m };
        var transferResponse = await _client.PostAsJsonAsync(
            $"/api/wallets/{sourceWallet.Id}/transfers", transferDto);

        // Assert - Transfer successful
        Assert.Equal(HttpStatusCode.NoContent, transferResponse.StatusCode);

        // Verify source wallet balance
        var sourceAfter = await _client.GetFromJsonAsync<WalletDto>($"/api/wallets/{sourceWallet.Id}");
        Assert.Equal(750m, sourceAfter!.Balance); // 1000 - 250

        // Verify destination wallet balance
        var destAfter = await _client.GetFromJsonAsync<WalletDto>($"/api/wallets/{destWallet.Id}");
        Assert.Equal(250m, destAfter!.Balance); // 0 + 250

        // Verify movements for source wallet
        var sourceMovements = await _client.GetFromJsonAsync<List<MovementDto>>(
            $"/api/wallets/{sourceWallet.Id}/movements");
        Assert.Equal(2, sourceMovements!.Count); // Initial credit + debit

        var debitMovement = sourceMovements.First(m => m.Type == MovementType.Debit);
        Assert.Equal(250m, debitMovement.Amount);
        Assert.Equal(destWallet.Id, debitMovement.RelatedWalletId);

        // Verify movements for destination wallet
        var destMovements = await _client.GetFromJsonAsync<List<MovementDto>>(
            $"/api/wallets/{destWallet.Id}/movements");
        Assert.Single(destMovements!); // Only credit from transfer

        var creditMovement = destMovements.First();
        Assert.Equal(MovementType.Credit, creditMovement.Type);
        Assert.Equal(250m, creditMovement.Amount);
        Assert.Equal(sourceWallet.Id, creditMovement.RelatedWalletId);
    }

    [Fact]
    public async Task Transfer_InsufficientBalance_ReturnsBadRequest()
    {
        // Arrange - Create wallets
        var source = await CreateWalletAsync("DOC003", "User A");
        var dest = await CreateWalletAsync("DOC004", "User B");

        // Add small balance
        await _client.PostAsJsonAsync($"/api/wallets/{source.Id}/movements",
            new CreateMovementDto(50m, MovementType.Credit));

        // Act - Try to transfer more than available
        var transferDto = new TransferDto { ToWalletId = dest.Id, Amount = 100m };
        var response = await _client.PostAsJsonAsync(
            $"/api/wallets/{source.Id}/transfers", transferDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Verify balances unchanged
        var sourceAfter = await _client.GetFromJsonAsync<WalletDto>($"/api/wallets/{source.Id}");
        Assert.Equal(50m, sourceAfter!.Balance);

        var destAfter = await _client.GetFromJsonAsync<WalletDto>($"/api/wallets/{dest.Id}");
        Assert.Equal(0m, destAfter!.Balance);
    }

    [Fact]
    public async Task Transfer_ToNonExistentWallet_ReturnsNotFound()
    {
        // Arrange
        var source = await CreateWalletAsync("DOC005", "User C");
        await _client.PostAsJsonAsync($"/api/wallets/{source.Id}/movements",
            new CreateMovementDto(100m, MovementType.Credit));

        // Act
        var transferDto = new TransferDto { ToWalletId = 9999, Amount = 50m };
        var response = await _client.PostAsJsonAsync(
            $"/api/wallets/{source.Id}/transfers", transferDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Transfer_ToSameWallet_ReturnsBadRequest()
    {
        // Arrange
        var wallet = await CreateWalletAsync("DOC006", "User D");
        await _client.PostAsJsonAsync($"/api/wallets/{wallet.Id}/movements",
            new CreateMovementDto(100m, MovementType.Credit));

        // Act
        var transferDto = new TransferDto { ToWalletId = wallet.Id, Amount = 50m };
        var response = await _client.PostAsJsonAsync(
            $"/api/wallets/{wallet.Id}/transfers", transferDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Transfer_NegativeAmount_ReturnsBadRequest()
    {
        // Arrange
        var source = await CreateWalletAsync("DOC007", "User E");
        var dest = await CreateWalletAsync("DOC008", "User F");

        // Act
        var transferDto = new TransferDto { ToWalletId = dest.Id, Amount = -50m };
        var response = await _client.PostAsJsonAsync(
            $"/api/wallets/{source.Id}/transfers", transferDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<WalletDto> CreateWalletAsync(string documentId, string name)
    {
        var dto = new CreateWalletDto(documentId, name);
        var response = await _client.PostAsJsonAsync("/api/wallets", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<WalletDto>())!;
    }
}