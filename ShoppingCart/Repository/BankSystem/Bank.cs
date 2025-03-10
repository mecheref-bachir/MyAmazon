﻿using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using ShoppingCart.Repository.BankSystem.BankSystemModels;

namespace ShoppingCart.Repository.BankSystem
{
    public class Bank : IBank
    {
        private DataContext _context;
        public Bank(DataContext context)
        {
            _context = context;
        }


        public async Task<Status> ProceedPayment(PaymentData paymentData)
        {
            Status status = new Status();
            Transaction transaction = new Transaction();
            transaction.TransactionValue = paymentData.Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionStatus = "TS";
            transaction.CardNumber = paymentData.userAccount.CardNumber;
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            status.StatusCode = 1;
            status.Message = "Payment Succeed";
            return status;

        }

        public async Task<Status>  VerifyPayment(PaymentData paymentData)
        {
            Status status = new Status();
            Account account = await _context.Accounts.Where( c=> c.CardNumber == paymentData.userAccount.CardNumber).FirstOrDefaultAsync();
            if (account == null)
            {
                status.StatusCode = 0;
                status.Message = "Account Not Found";
                return status;
            }

           if ((account.NameOnCard != paymentData.userAccount.NameOnCard) ||
                (account.CVV != paymentData.userAccount.CVV) ||
                (account.ExpirationDate != paymentData.userAccount.ExpirationDate) ||
                (account.PaymentType != paymentData.userAccount.PaymentType )
                ) 
            {
                status.StatusCode = 0;
                status.Message = "Invalid Account";
                return status;
            }

           if(paymentData.Amount>account.CurrentAmount)
            {
                status.StatusCode = 0;
                status.Message = "Inssuficent Amount";
                return status;
            }

            return status;


        }
    }
}
