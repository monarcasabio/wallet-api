namespace WalletSystem.Core.Domain.Exceptions;

public class WalletNotFoundException : DomainException
{
    public WalletNotFoundException(int walletId)
        : base($"Wallet with ID {walletId} not found") { }
}