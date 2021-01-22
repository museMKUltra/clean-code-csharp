namespace CleanCode.ControlStructure
{
    public interface ITransactionProcessor
    {
        void ProcessCreditCardPayment();
        void ProcessCreditCardRefund();
        void ProcessPaypalPayment();
        void ProcessPlanPayment();
        void ProcessPaypalRefund();
        void ProcessPlanRefund();
    }

    public class TransactionProcessor : ITransactionProcessor
    {
        public void ProcessCreditCardPayment()
        {
            throw new System.NotImplementedException();
        }

        public void ProcessCreditCardRefund()
        {
            throw new System.NotImplementedException();
        }

        public void ProcessPaypalPayment()
        {
            throw new System.NotImplementedException();
        }

        public void ProcessPlanPayment()
        {
            throw new System.NotImplementedException();
        }

        public void ProcessPaypalRefund()
        {
            throw new System.NotImplementedException();
        }

        public void ProcessPlanRefund()
        {
            throw new System.NotImplementedException();
        }
    }
}