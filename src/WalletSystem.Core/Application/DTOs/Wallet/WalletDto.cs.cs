namespace WalletSystem.Core.Application.DTOs.Wallet;

public record WalletDto(int Id,
                        string DocumentId,
                        string Name,
                        decimal Balance,
                        DateTime CreatedAt,
                        DateTime UpdatedAt,
                        bool IsActive);