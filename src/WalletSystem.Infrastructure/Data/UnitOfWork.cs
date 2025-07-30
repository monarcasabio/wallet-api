using WalletSystem.Core.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace WalletSystem.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
	private readonly WalletSystemDbContext _dbContext;
	private IDbContextTransaction _currentTransaction;

	public UnitOfWork(WalletSystemDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<int> SaveChangesAsync()
	{
		return await _dbContext.SaveChangesAsync();
	}

	public async Task BeginTransactionAsync()
	{
		if (_currentTransaction != null)
		{
			throw new InvalidOperationException("A transaction is already in progress.");
		}
		_currentTransaction = await _dbContext.Database.BeginTransactionAsync();
	}

	public async Task CommitAsync()
	{
		if (_currentTransaction == null)
		{
			throw new InvalidOperationException("No transaction in progress to commit.");
		}

		try
		{
			await _dbContext.SaveChangesAsync();
			await _currentTransaction.CommitAsync();
		}
		catch
		{
			await RollbackAsync();
			throw;
		}
		finally
		{
			_currentTransaction.Dispose();
			_currentTransaction = null;
		}
	}

	public async Task RollbackAsync()
	{
		if (_currentTransaction == null)
		{
			throw new InvalidOperationException("No transaction in progress to rollback.");
		}

		try
		{
			await _currentTransaction.RollbackAsync();
		}
		finally
		{
			_currentTransaction.Dispose();
			_currentTransaction = null;
		}
	}
}