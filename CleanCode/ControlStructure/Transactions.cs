using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCode.ControlStructure
{
    public class Transactions
    {
        private ITransactionRepository _transactionRepository;
        private ITransactionProcessor _transactionProcessor;

        public Transactions(ITransactionProcessor transactionProcessor = null,
            ITransactionRepository transactionRepository = null)
        {
            _transactionRepository = transactionRepository ?? new TransactionRepository();
            _transactionProcessor = transactionProcessor ?? new TransactionProcessor();
        }

        public void Process()
        {
            var transactions = _transactionRepository.GetTransactions();

            ValidateTransactions(transactions);

            ProcessTransactions(transactions);
        }

        private void ProcessTransactions(IEnumerable<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                ValidateTransaction(transaction);

                ProcessTransaction(transaction);
            }
        }

        private void ProcessTransaction(Transaction transaction)
        {
            TransactionHandler.HandlePayment handlePayment = null;
            TransactionHandler.HandleRefund handleRefund = null;

            SetTransactionHandler(transaction, ref handlePayment, ref handleRefund);

            ImplementTransactionHandler(transaction, handlePayment, handleRefund);
        }

        private static void ImplementTransactionHandler(Transaction transaction,
            TransactionHandler.HandlePayment handlePayment,
            TransactionHandler.HandleRefund handleRefund)
        {
            var transactionHandler = new TransactionHandler();
            if (IsPayment(transaction))
            {
                transactionHandler.ProcessPayment(handlePayment);
            }
            else if (IsRefund(transaction))
            {
                transactionHandler.ProcessRefund(handleRefund);
            }
        }

        private void SetTransactionHandler(Transaction transaction, ref TransactionHandler.HandlePayment handlePayment,
            ref TransactionHandler.HandleRefund handleRefund)
        {
            if (IsCreditCard(transaction))
            {
                handlePayment = _transactionProcessor.ProcessCreditCardPayment;
                handleRefund = _transactionProcessor.ProcessCreditCardRefund;
            }

            if (IsPaypal(transaction))
            {
                handlePayment = _transactionProcessor.ProcessPaypalPayment;
                handleRefund = _transactionProcessor.ProcessPaypalRefund;
            }

            if (IsPlan(transaction))
            {
                handlePayment = _transactionProcessor.ProcessPlanPayment;
                handleRefund = _transactionProcessor.ProcessPlanRefund;
            }
        }

        private static bool IsRefund(Transaction transaction)
        {
            return transaction.Type == TransactionTypes.REFUND.ToString();
        }

        private static bool IsPayment(Transaction transaction)
        {
            return transaction.Type == TransactionTypes.PAYMENT.ToString();
        }

        private static bool IsPlan(Transaction transaction)
        {
            return transaction.Method == TransactionMethods.PLAN.ToString();
        }

        private static bool IsPaypal(Transaction transaction)
        {
            return transaction.Method == TransactionMethods.PAYPAL.ToString();
        }

        private static bool IsCreditCard(Transaction transaction)
        {
            return transaction.Method == TransactionMethods.CREDIT_CARD.ToString();
        }

        private static void ValidateTransaction(Transaction transaction)
        {
            if (!IsOpen(transaction) || !IsTypeValid(transaction))
            {
                throw new InvalidOperationException();
            }
        }

        private static bool IsTypeValid(Transaction transaction)
        {
            return Enum.IsDefined(typeof(TransactionTypes), transaction.Type);
        }

        private static bool IsOpen(Transaction transaction)
        {
            return transaction.Status == "OPEN";
        }

        private static void ValidateTransactions(IEnumerable<Transaction> transactions)
        {
            if (transactions.Count() == 0)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal class TransactionHandler
    {
        public delegate void HandlePayment();

        public delegate void HandleRefund();

        public void ProcessPayment(HandlePayment handlePayment)
        {
            handlePayment();
        }

        public void ProcessRefund(HandleRefund handleRefund)
        {
            handleRefund();
        }
    }
}