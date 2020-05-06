using System;
using System.Collections.Generic;

namespace Bank
{
    enum TransactionOption
    {
        Print,
        Execute,
        Rollback,
        Quit
    }
    class Bank
    {
        private List<Account> _accounts;
        private List<Transaction> _transactions;
        private List<Transaction> tempTransactionList;

        public List<Account> Accounts { get => _accounts; set => _accounts = value; }
        internal List<Transaction> Transactions { get => _transactions; set => _transactions = value; }
        internal List<Transaction> TempTransactionList { get => tempTransactionList; set => tempTransactionList = value; }

        public Bank(){
            _accounts = new List<Account>();
            _transactions = new List<Transaction>(); 
            tempTransactionList = new List<Transaction>();
        }

        public void AddAccount(Account account)
        {
            Accounts.Add(account);
        }

        public Account GetAccount(String name)
        {

            foreach(Account account in Accounts)
            {
                if(account.Name.ToLower() == name.ToLower())
                    return account;
            }
            return null;
        }

        public void ExecuteTransaction(Transaction transaction)
        {
            if(transaction.GetType() ==  typeof(DepositTransaction))
                Deposit(transaction);
            else if(transaction.GetType() == typeof(WithdrawTransaction))
                WithDraw(transaction);
            else
                Transfer(transaction);
        }

        public void RollbackTransaction(Transaction transaction)
        {
            transaction.Rollback();
            Transactions.Remove(transaction);
            TempTransactionList.Remove(transaction);
            Console.WriteLine("Your rollback is successful");
        }

        public void PrintTransactionHistory(Account account)
        {
            Transaction transaction;
            List<Transaction> transactionsList = new List<Transaction>();
            for(int i = 0; i < Transactions.Count; i++)
            {
                transaction = Transactions[i];
                if(transaction.GetType() ==  typeof(DepositTransaction)) {
                    DepositTransaction depositTransaction = (DepositTransaction)transaction;
                    if(depositTransaction.Account == account)
                        transactionsList.Add(depositTransaction);
                }
                else if(transaction.GetType() == typeof(WithdrawTransaction)) {
                    WithdrawTransaction withdrawTransaction = (WithdrawTransaction)transaction;
                    if(withdrawTransaction.Account == account)
                        transactionsList.Add(withdrawTransaction);
                }
                else {
                    TransferTransaction transferTransaction = (TransferTransaction)transaction;
                    if(transferTransaction.FromAccount == account || transferTransaction.ToAccount == account)
                        transactionsList.Add(transferTransaction);
                }
            }
            for(int i = 0; i < transactionsList.Count; i++)
            {
                Console.WriteLine("{0}. {1}",i+1, Print(transactionsList[i]));
            }
            TempTransactionList = transactionsList;
        }

        private string Print(Transaction transaction)
        {
            String transactionType = "", name = "" ;
            decimal amount = 0M;
            if(transaction.GetType() ==  typeof(DepositTransaction)) {
                transactionType = "Deposite";
                DepositTransaction depositTransaction = (DepositTransaction)transaction;
                name = depositTransaction.Account.Name;
                amount = depositTransaction.Amount;
            }
            else if(transaction.GetType() == typeof(WithdrawTransaction)) {
                transactionType = "Withdraw";
                WithdrawTransaction withdrawTransaction = (WithdrawTransaction)transaction;
                name = withdrawTransaction.Account.Name;
                amount = withdrawTransaction.Amount;
            }
            else {
                TransferTransaction transferTransaction = (TransferTransaction)transaction;
                name = transferTransaction.FromAccount.Name;
                amount = transferTransaction.Amount;
                transactionType = "Transfer";

                return transactionType + " to " + transferTransaction.ToAccount.Name + " with " + amount.ToString("C") + " executed by " + name + " at " + transaction.DateStamp.ToString();
            }
            return transactionType + " with " + amount.ToString("C") +  " executed by " + name + " at " + transaction.DateStamp.ToString();
        }

        private void Deposit(Transaction transaction)
        { 
            String executionAnswer, rollBackAnswer;
            bool continueDeposit = true;
            TransactionOption option;
            DepositTransaction deposit = (DepositTransaction)transaction;
            decimal amount = deposit.Amount; 

            while(continueDeposit)
            {
                option = (TransactionOption)ReadTransationOption();

                switch(option)
                {
                    case (TransactionOption)0:
                        if((deposit.Executed == true) && (deposit.Success == true))
                        {
                            deposit.Print();
                            Console.ReadLine();
                        }
                        else
                            Console.WriteLine("The deposit operation has not been executed.");

                        continueDeposit = askContinue("options");
                        break;
                    case (TransactionOption)1:
                        if((deposit.Executed == false) && (deposit.Success == false))
                        {
                            while(true)
                            {
                                try
                                {
                                    Console.Write("Enter the amount you would like to deposit: ");
                                    amount = Convert.ToDecimal(Console.ReadLine());

                                    if(amount > 0)
                                    {
                                        Console.Write("Would you like to execute this deposit operation? (y/n) ");
                                        executionAnswer = Console.ReadLine();
                                        if((executionAnswer.ToLower() == "yes") || (executionAnswer.ToLower() == "y"))
                                        {
                                                deposit.Amount = amount;
                                                deposit.Execute(); 
                                                Transactions.Add(transaction);
                                                break;
                                        }
                                        break;
                                    } else
                                        Console.WriteLine("Erorr: Your input must be positive");
                                }
                                catch (System.Exception)
                                {
                                    Console.WriteLine("Error: Your input must be a number");
                                }
                            }
                        } else
                            Console.WriteLine("The deposit operation was executed.");

                        continueDeposit = askContinue("options");
                        break;

                    case (TransactionOption)2:
                        if((deposit.Reversed == false) && (deposit.Success == true))
                        {
                            Console.Write("Would you like to reverse this deposit operation? (y/n) ");
                            rollBackAnswer = Console.ReadLine();
                            if((rollBackAnswer.ToLower() == "yes") || (rollBackAnswer.ToLower() == "y")) deposit.Rollback();
                        }
                        else
                            Console.WriteLine("The deposit operation was reversed already.");

                        continueDeposit = askContinue("options");
                        break;

                    case (TransactionOption)3:
                        continueDeposit = false;
                        break;
                }
            }
        }

        private void WithDraw(Transaction transaction)
        {
            String executionAnswer, rollBackAnswer;
            bool continueWithdraw = true;
            TransactionOption option;
            WithdrawTransaction withdraw = (WithdrawTransaction)transaction;
            decimal amount = withdraw.Amount;
            decimal balance = withdraw.Account.Balance;

            while(continueWithdraw)
            {
                option = (TransactionOption)ReadTransationOption();

                switch(option)
                {
                    case (TransactionOption)0:
                        if((withdraw.Executed == true) && (withdraw.Success == true))
                        {
                            withdraw.Print();
                            Console.ReadLine();
                        }
                        else
                            Console.WriteLine("The withdraw operation has not been executed.");

                        continueWithdraw = askContinue("options");
                        break;

                    case (TransactionOption)1:
                        if((withdraw.Executed == false) && (withdraw.Success == false))
                        {
                            while(true)
                            {
                                try
                                {
                                    Console.Write("Enter the amount you would like to withdraw: ");
                                    amount = Convert.ToDecimal(Console.ReadLine());

                                    if(amount > 0)
                                    {
                                        Console.Write("Would you like to execute this withdraw operation? (y/n) ");
                                        executionAnswer = Console.ReadLine();
                                        if((executionAnswer.ToLower() == "yes") || (executionAnswer.ToLower() == "y"))
                                        {
                                            if(balance < amount)
                                            {
                                                Console.WriteLine("Failed. You cannot withdraw more than your balance.");
                                                continue;
                                            }
                                            else
                                            {
                                                withdraw.Amount = amount;
                                                withdraw.Execute(); 
                                                Transactions.Add(transaction);
                                                break;
                                            }
                                        }
                                        break;
                                    } else
                                        Console.WriteLine("Erorr: Your input must be positive");
                                }
                                catch (System.Exception)
                                {
                                    Console.WriteLine("Error: Your input must be a number");
                                }
                            }
                        } else
                            Console.WriteLine("The withdraw operation was executed.");

                        continueWithdraw = askContinue("options");
                        break;

                    case (TransactionOption)2:
                        if((withdraw.Reversed == false) && (withdraw.Success == true))
                        {
                            Console.Write("Would you like to reverse this withdraw operation? (y/n) ");
                            rollBackAnswer = Console.ReadLine();
                            if((rollBackAnswer.ToLower() == "yes") || (rollBackAnswer.ToLower() == "y")) withdraw.Rollback();
                        }
                        else
                            Console.WriteLine("The withdraw operation was reversed already.");

                        continueWithdraw = askContinue("options");
                        break;

                    case (TransactionOption)3:
                        continueWithdraw = false; 
                        break;
                }
            }

        }

        private void Transfer(Transaction transaction)
        {
            String executionAnswer, rollBackAnswer;
            bool continueTransfer = true;
            TransactionOption option;
            TransferTransaction transfer = (TransferTransaction)transaction;
            Account fromAccount = transfer.FromAccount;
            Account toAccount = transfer.ToAccount;
            decimal amount = transfer.Amount;

            while(continueTransfer)
            {
                option = (TransactionOption)ReadTransationOption();

                switch(option)
                {
                    case (TransactionOption)0:
                        if((transfer.Executed == true) && (transfer.Success == true))
                        {
                            transfer.Print();
                            Console.ReadLine();
                        }
                        else
                            Console.WriteLine("The transfer operation has not been executed.");

                        continueTransfer = askContinue("options");
                        break;
                    case (TransactionOption)1:
                        if((transfer.Executed == false) && (transfer.Success == false))
                        {
                            while(true)
                            {
                                try
                                {
                                    Console.Write("Enter the amount you would like to transfer: ");
                                    amount = Convert.ToDecimal(Console.ReadLine());

                                    if(amount > 0)
                                    {
                                        Console.Write("Would you like to execute this transfer operation? (y/n) ");
                                        executionAnswer = Console.ReadLine();
                                        if((executionAnswer.ToLower() == "yes") || (executionAnswer.ToLower() == "y"))
                                        {
                                            if(fromAccount.Balance < amount)
                                            {
                                                Console.WriteLine("Failed. You cannot transfer more than your balance.");
                                                continue;
                                            }
                                            else
                                            {
                                                transfer.Amount = amount;
                                                transfer.Execute();
                                                Transactions.Add(transaction);
                                                break;
                                            }
                                        }
                                        break;
                                    } else
                                        Console.WriteLine("Erorr: Your input must be positive");
                                }
                                catch (System.Exception)
                                {
                                    Console.WriteLine("Error: Your input must be a number");
                                }
                            }
                        } else
                            Console.WriteLine("The transfer operation was executed.");

                        continueTransfer = askContinue("options");
                        break;
                    case (TransactionOption)2:
                        if(toAccount.Balance < transfer.Amount)
                            Console.WriteLine("The sender has insufficient funds to do rollback.");
                        else if((transfer.Reversed == false) && (transfer.Success == true))
                        {
                            Console.Write("Would you like to reverse this transfer operation? (y/n) ");
                            rollBackAnswer = Console.ReadLine();
                            if((rollBackAnswer.ToLower() == "yes") || (rollBackAnswer.ToLower() == "y")) transfer.Rollback();
                        }
                        else
                            Console.WriteLine("The transfer operation was reversed already.");

                        continueTransfer = askContinue("options");
                        break;
                    case (TransactionOption)3:
                        continueTransfer = false;
                        break;
                }
            }

        }
        private bool askContinue(String hole)
        {
            String continueTransaction;

            Console.Write($"Would you like to continue more {hole}? (y/n) ");
            continueTransaction = Console.ReadLine();
            if((continueTransaction.ToLower() == "yes") || (continueTransaction.ToLower() == "y"))
                return true;
            else
            {   
                Console.Clear();
                return false;
            }
        }

        private int ReadTransationOption()
        {
            Console.Clear();
            bool isValid = false;
            int option = 0;
            do
            {
                try
                {
                    Console.WriteLine("Four options:" +
                    "\n1 - " + (TransactionOption)0 +
                    "\n2 - " + (TransactionOption)1 +
                    "\n3 - " + (TransactionOption)2 +
                    "\n4 - " + (TransactionOption)3);

                    Console.Write("Please enter a number corresponding to the action: ");
                    option = Convert.ToInt32(Console.ReadLine());

                    if((option >= 1) && (option <= 4))
                    {
                        isValid = true;
                    } else 
                    {
                        Console.WriteLine("Your input value cannot be less than 1 or larger than 3");
                        Console.WriteLine();
                    }
                } catch (System.Exception)
                {
                    Console.WriteLine("Error: Your input value must be an integer.");
                    Console.WriteLine();
                }
            } while (!isValid);

            return option - 1;

        }

    }
}