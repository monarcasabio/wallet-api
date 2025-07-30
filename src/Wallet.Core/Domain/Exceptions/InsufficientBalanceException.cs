public class InsufficientBalanceException : DomainException
{
	public InsufficientBalanceException(decimal balance)
		: base($"Insufficient balance. Current: {balance}") { }
}