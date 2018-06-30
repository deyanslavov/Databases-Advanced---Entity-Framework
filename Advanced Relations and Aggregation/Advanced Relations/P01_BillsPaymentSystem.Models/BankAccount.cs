namespace P01_BillsPaymentSystem.Models
{
    using System.Collections.Generic;

    public class BankAccount
    {
        public int BankAccountId { get; set; }

        public decimal Balance { get; set; }

        public string BankName { get; set; }

        public string SwiftCode { get; set; }

        public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();

        public void Deposit(decimal amount)
        {
            this.Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (this.Balance - amount >= 0)
                this.Balance -= amount;
        }
    }
}
