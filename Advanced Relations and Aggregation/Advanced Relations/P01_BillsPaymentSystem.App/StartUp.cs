namespace P01_BillsPaymentSystem.App
{
    using System;
    using System.Linq;
    using System.Globalization;

    using Microsoft.EntityFrameworkCore;

    using P01_BillsPaymentSystem.Data;
    using P01_BillsPaymentSystem.Models;
    
    public class StartUp
    {
        private static BillsPaymentSystemContext db = new BillsPaymentSystemContext();

        public static void Main()
        {
            Console.Write("Please enter a valid UserId: ");
            int userId = int.Parse(Console.ReadLine());

            Console.Write("Please enter the amount you wish to pay: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            PayBills(userId, amount, db);
        }

        // 04. Pay Bills

        public static string PayBills(int userId, decimal amount, BillsPaymentSystemContext db)
        {
            var result = "";
            using (db)
            {
                var user = db.Users.Where(u => u.UserId == userId).Select(u => new
                {
                    Name = $"{u.FirstName} {u.LastName}",

                    CreditCardsAmounts = u.PaymentMethods.Where(pm => pm.Type == PaymentMethodType.CreditCard).Select(pm => pm.CreditCard.LimitLeft).Sum(),

                    BankAccountsAmounts = u.PaymentMethods.Where(pm => pm.Type == PaymentMethodType.BankAccount).Select(pm => pm.BankAccount.Balance).Sum(),

                    bankaccs = u.PaymentMethods.Where(pm => pm.Type == PaymentMethodType.BankAccount).Select(pm => pm.BankAccount).ToList(),

                    ccs = u.PaymentMethods.Where(pm => pm.Type == PaymentMethodType.CreditCard).Select(pm => pm.CreditCard).ToList()
                }).FirstOrDefault();

                if (amount > (user.BankAccountsAmounts + user.CreditCardsAmounts))
                {
                    result = "Insufficient funds!";
                    Console.WriteLine(result);
                    return result;
                }
                else
                {
                    foreach (var account in user.bankaccs)
                    {
                        if (account.Balance >= amount)
                        {
                            account.Withdraw(amount);
                            result = $"Payment successful! You've withdrawn the {account.Balance} amount from your bank accounts!";
                            amount = 0;
                            Console.WriteLine(result);
                            return result;
                        }
                        else
                        {
                            amount -= account.Balance;
                            account.Withdraw(account.Balance);
                            result = $"Withdrawn: {account.Balance:F2} money. You still owe {amount:F2}";
                            if (amount > 0)
                            {
                                foreach (var cc in user.ccs)
                                {
                                    if (cc.LimitLeft >= amount)
                                    {
                                        cc.Withdraw(amount);
                                        result = $"Paymen successful! You've withdrawn the {amount} amount from your credit cards!";
                                        Console.WriteLine(result);
                                        return result;
                                    }

                                    amount -= cc.LimitLeft;
                                    cc.Withdraw(cc.LimitLeft);
                                    result = $"Withdrawn: {cc.LimitLeft:F2} money. You still owe {amount:F2}";
                                    Console.WriteLine(result);
                                    return result;
                                }
                            }
                            return result;
                        }

                    }
                    result = $"Paymen successful!You've made it!";
                    return result;
                }
            }
        }

        // 03. User Details

        private static void PrintUserDetails(BillsPaymentSystemContext db)
        {
            int userId = 0;
            bool userFound = false;

            while (!userFound)
            {
                try
                {
                    Console.Write("Please enter UserId: ");
                    int.TryParse(Console.ReadLine(), out userId);

                    if (userId == default(int))
                    {
                        throw new ArgumentException("Please enter a valid UserId!");
                    }

                    User user = db.Users.Find(userId);

                    if (user == null)
                    {
                        throw new ArgumentException($"User with Id {userId} does not exist in the database!");
                    }

                    user = db.Users
                        .Where(u => u.UserId == userId)
                        .Include(u => u.PaymentMethods)
                        .FirstOrDefault();

                    Console.WriteLine($"User: {user.FirstName} {user.LastName}");
                    Console.WriteLine("Bank Accounts:");

                    foreach (var p in user.PaymentMethods.Where(p => p.Type == PaymentMethodType.BankAccount))
                    {
                        Console.WriteLine($"-- ID: {p.Id}");

                        var bankAccount = db.BankAccounts.Find(p.BankAccountId);

                        Console.WriteLine($"--- Balance: {bankAccount.Balance}");
                        Console.WriteLine($"--- Bank: {bankAccount.BankName}");
                        Console.WriteLine($"--- Swift: {bankAccount.SwiftCode}");
                    }

                    Console.WriteLine("Credit Cards:");

                    foreach (var p in user.PaymentMethods.Where(p => p.Type == PaymentMethodType.CreditCard))
                    {
                        Console.WriteLine($"-- ID: {p.Id}");

                        var creditCard = db.CreditCards.Find(p.CreditCardId);

                        Console.WriteLine($"--- Limit: {creditCard.Limit}");
                        Console.WriteLine($"--- Money Owed: {creditCard.MoneyOwed}");
                        Console.WriteLine($"--- Limit Left: {creditCard.LimitLeft}");
                        Console.WriteLine($"--- Expiration Date: {creditCard.ExpirationDate.Year}/{creditCard.ExpirationDate.Month}");
                    }
                    userFound = true;
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        // 02. Seed Some Data

        private static void Seed(BillsPaymentSystemContext db)
        {
            using (db)
            {
                var user1 = new User()
                {
                    FirstName = "Choko",
                    LastName = "Boko",
                    Email = "choko.boko@mail.bg",
                    Password = "ChokoBoko1234"
                };

                var creditrcards = new CreditCard[]
                {
                    new CreditCard
                    {
                        ExpirationDate = DateTime.ParseExact("19.07.2017","dd.mm.yyyy",CultureInfo.InvariantCulture),
                        Limit = 4500m,
                        MoneyOwed = 230m
                    },
                    new CreditCard
                    {
                        ExpirationDate = DateTime.ParseExact("01.12.1989","dd.mm.yyyy",CultureInfo.InvariantCulture),
                        Limit = 10000m,
                        MoneyOwed = 0m
                    },
                    new CreditCard
                    {
                        ExpirationDate = DateTime.ParseExact("24.04.2010","dd.mm.yyyy",CultureInfo.InvariantCulture),
                        Limit = 500m,
                        MoneyOwed = 50m
                    },
                };

                var bankAccount = new BankAccount
                {
                    BankName = "UniCredit Bulbank",
                    Balance = 15000m,
                    SwiftCode = "SWEDJHEWI"
                };

                var paymentMethods = new PaymentMethod[]
                {
                    new PaymentMethod()
                    {
                        User = user1,
                        Type = PaymentMethodType.CreditCard,
                        CreditCard = creditrcards[0],
                        CreditCardId = 10
                    },
                    new PaymentMethod()
                    {
                        User = user1,
                        Type = PaymentMethodType.CreditCard,
                        CreditCard = creditrcards[1],
                        CreditCardId = 10
                    },
                    new PaymentMethod()
                    {
                        User = user1,
                        Type = PaymentMethodType.BankAccount,
                        BankAccount = bankAccount
                    }
                };

                db.Users.Add(user1);
                db.CreditCards.AddRange(creditrcards);
                db.BankAccounts.Add(bankAccount);
                db.PaymentMethods.AddRange(paymentMethods);
                db.SaveChanges();
            }
        }
    }
}
