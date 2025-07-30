public class Movement
{
    public int Id { get; set; }

    public int WalletId { get; set; }
    public int? RelatedWalletId { get; set; }

    public decimal Amount { get; set; }
    public MovementType Type { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public Wallet Wallet { get; set; } = default!;
    public Wallet? RelatedWallet { get; set; } = default!;
}