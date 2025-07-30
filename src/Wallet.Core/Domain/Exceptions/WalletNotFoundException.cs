public class WalletNotFoundException : DomainException
{
    public WalletNotFoundException(int walletId)
        : base($"Wallet with ID {walletId} not found") { }
}