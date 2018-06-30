namespace P01_BillsPaymentSystem.Models
{
    using System;
    using System.Collections.Generic;

    public class CreditCard
    {
        public int CreditCardId { get; set; }

        public DateTime ExpirationDate { get; set; }

        public decimal Limit { get; set; }

        public decimal MoneyOwed { get; set; }

        public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();

        public decimal LimitLeft => this.Limit - this.MoneyOwed;

        public void Deposit(decimal amount)
        {
            this.MoneyOwed -= amount;
        }

        public void Withdraw(decimal amount)
        {
            if (this.LimitLeft - amount >= 0)
            {
                this.MoneyOwed += amount;
            }
        }
    }
}
