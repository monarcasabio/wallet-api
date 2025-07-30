namespace WalletSystem.Core.Domain.Entities;

public class Wallet
{
    public int Id { get; set; }
    public string DocumentId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Movimientos iniciados por esta billetera
    public ICollection<Movement> MovementsFrom { get; set; } = new List<Movement>();

    // Movimientos recibidos por esta billetera
    public ICollection<Movement> MovementsTo { get; set; } = new List<Movement>();
}