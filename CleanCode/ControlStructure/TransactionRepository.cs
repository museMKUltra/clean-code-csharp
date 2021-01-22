using System.Collections;
using System.Collections.Generic;

namespace CleanCode.ControlStructure
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetTransactions();
    }

    public class TransactionRepository : ITransactionRepository
    {
        public IEnumerable<Transaction> GetTransactions()
        {
            return new List<Transaction>();
        }
    }
}