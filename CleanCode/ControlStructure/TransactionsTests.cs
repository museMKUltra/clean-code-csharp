using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace CleanCode.ControlStructure
{
    [TestFixture]
    public class TransactionsTests
    {
        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<ITransactionRepository>();
            _processor = new Mock<ITransactionProcessor>();
            _transactions = new Transactions(_processor.Object, _repository.Object);
        }

        private Mock<ITransactionRepository> _repository;
        private Transactions _transactions;
        private Mock<ITransactionProcessor> _processor;

        [Test]
        public void Process_TransactionsRepositoryIsEmpty_ThrowArgumentOutOfRangeException()
        {
            Assert.That(() => _transactions.Process(), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Process_TransactionStatusIsNotOpen_ThrowInvalidOperationException()
        {
            _repository.Setup(r => r.GetTransactions()).Returns(new List<Transaction>
            {
                new Transaction {Id = 0, Status = ""}
            });

            Assert.That(() => _transactions.Process(), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Process_TransactionTypeIsPaymentAndMethodIsCreditCard_VerifyPrecessCreditCardPayment()
        {
            _repository.Setup(r => r.GetTransactions()).Returns(new List<Transaction>
            {
                new Transaction
                {
                    Id = 0, Status = "OPEN", Type = TransactionTypes.PAYMENT.ToString(),
                    Method = TransactionMethods.CREDIT_CARD.ToString()
                }
            });

            _transactions.Process();

            _processor.Verify(p => p.ProcessCreditCardPayment());
        }

        [Test]
        public void Process_TransactionTypeIsPaymentAndMethodIsCreditCard_VerifyPrecessCreditCardRefundNotCalled()
        {
            _repository.Setup(r => r.GetTransactions()).Returns(new List<Transaction>
            {
                new Transaction
                {
                    Id = 0, Status = "OPEN", Type = TransactionTypes.PAYMENT.ToString(),
                    Method = TransactionMethods.CREDIT_CARD.ToString()
                }
            });

            _transactions.Process();

            _processor.Verify(p => p.ProcessCreditCardRefund(), Times.Never);
        }

        [Test]
        public void Process_TransactionTypeIsPaymentAndMethodIsPaypal_VerifyPrecessPaypalPayment()
        {
            _repository.Setup(r => r.GetTransactions()).Returns(new List<Transaction>
            {
                new Transaction
                {
                    Id = 0, Status = "OPEN", Type = TransactionTypes.PAYMENT.ToString(),
                    Method = TransactionMethods.PAYPAL.ToString()
                }
            });

            _transactions.Process();

            _processor.Verify(p => p.ProcessPaypalPayment());
        }

        [Test]
        public void Process_TransactionTypeIsPaymentAndMethodIsPlan_VerifyPrecessPlanPayment()
        {
            _repository.Setup(r => r.GetTransactions()).Returns(new List<Transaction>
            {
                new Transaction
                {
                    Id = 0, Status = "OPEN", Type = TransactionTypes.PAYMENT.ToString(),
                    Method = TransactionMethods.PLAN.ToString()
                }
            });

            _transactions.Process();

            _processor.Verify(p => p.ProcessPlanPayment());
        }

        [Test]
        public void Process_TransactionTypeIsRefundAndMethodIsCreditCard_VerifyPrecessCreditCardRefund()
        {
            _repository.Setup(r => r.GetTransactions()).Returns(new List<Transaction>
            {
                new Transaction
                {
                    Id = 0, Status = "OPEN", Type = TransactionTypes.REFUND.ToString(),
                    Method = TransactionMethods.CREDIT_CARD.ToString()
                }
            });

            _transactions.Process();

            _processor.Verify(p => p.ProcessCreditCardRefund());
        }

        [Test]
        public void Process_TransactionTypeIsRefundAndMethodIsPaypal_VerifyPrecessPaypalRefund()
        {
            _repository.Setup(r => r.GetTransactions()).Returns(new List<Transaction>
            {
                new Transaction
                {
                    Id = 0, Status = "OPEN", Type = TransactionTypes.REFUND.ToString(),
                    Method = TransactionMethods.PAYPAL.ToString()
                }
            });

            _transactions.Process();

            _processor.Verify(p => p.ProcessPaypalRefund());
        }

        [Test]
        public void Process_TransactionTypeIsRefundAndMethodIsPlan_VerifyPrecessPlanRefund()
        {
            _repository.Setup(r => r.GetTransactions()).Returns(new List<Transaction>
            {
                new Transaction
                {
                    Id = 0, Status = "OPEN", Type = TransactionTypes.REFUND.ToString(),
                    Method = TransactionMethods.PLAN.ToString()
                }
            });

            _transactions.Process();

            _processor.Verify(p => p.ProcessPlanRefund());
        }

        [Test]
        public void Process_TransactionTypeIsInvalid_ThrowInvalidOperationException()
        {
            _repository.Setup(r => r.GetTransactions()).Returns(new List<Transaction>
            {
                new Transaction
                {
                    Id = 0, Status = "OPEN", Type = "",
                }
            });

            Assert.That(() => _transactions.Process(), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Process_TransactionsWithMultipleConditions_VerifyProcessTransactionHandler()
        {
            _repository.Setup(r => r.GetTransactions()).Returns(new List<Transaction>
            {
                new Transaction
                {
                    Id = 0, Status = "OPEN", Type = TransactionTypes.REFUND.ToString(),
                    Method = TransactionMethods.PLAN.ToString()
                },
                new Transaction
                {
                    Id = 1, Status = "OPEN", Type = TransactionTypes.PAYMENT.ToString(),
                    Method = TransactionMethods.PAYPAL.ToString()
                }
            });

            _transactions.Process();

            _processor.Verify(p => p.ProcessPlanRefund(), Times.Once);
            _processor.Verify(p => p.ProcessPaypalPayment(), Times.Once);
        }
    }
}