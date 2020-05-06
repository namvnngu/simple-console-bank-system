using System;

namespace Bank
{
    enum MenuOption
    {
        Withdraw,
        Deposit,
        Transfer,
        Print,
        AddNewAccount,
        PrintTransactionHistory,
        Quit
    }

    class BankSystem 
    {

        static int ReadUserOption()
        {
            bool isValid = false;
            int option = 0;
            do
            {
                try
                {
                    Console.WriteLine("Four options:" +
                    "\n1 - " + (MenuOption)0 +
                    "\n2 - " + (MenuOption)1 +
                    "\n3 - " + (MenuOption)2 +
                    "\n4 - " + (MenuOption)3 +
                    "\n5 - " + "Add new acccount" +
                    "\n6 - " + "Print Transaction History" +
                    "\n7 - " + (MenuOption)6);
                    Console.Write("Please enter a number corresponding to the action: ");
                    option = Convert.ToInt32(Console.ReadLine());

                    if((option >= 1) && (option <= 7))
                    {
                        isValid = true;
                    } else 
                    {
                        Console.WriteLine("Your input value cannot be less than 1 or larger than 7");
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

        static void DoWithdraw(Bank bank)
        {
          
            Console.Clear();
            Account account = FindAccount(bank);

            if(account != null)
            {
                decimal amount = 0;
                WithdrawTransaction withdraw = new WithdrawTransaction(account, amount);
                bank.ExecuteTransaction(withdraw);
            }
        }

        static void DoDeposite(Bank bank)
        {
            Console.Clear();
            Account account = FindAccount(bank);


            if(account != null)
            {
                decimal amount = 0; 
                DepositTransaction deposit = new DepositTransaction(account, amount);
                bank.ExecuteTransaction(deposit);
            }
        }

        static void DoTransfer(Bank bank)
        {
            Console.Clear();
            Console.WriteLine("Who would be the sender?");
            Account fromAccount = FindAccount(bank);
            Console.WriteLine("Who would be the receiver?");
            Account toAccount = FindAccount(bank);
            decimal amount = 0;

            if(fromAccount != null && toAccount != null)
            {

                TransferTransaction transfer = new TransferTransaction(fromAccount, toAccount, amount);
                bank.ExecuteTransaction(transfer);
            }
        }

        static void DoAddNewAccount(Bank bank)
        {
            String name;
            decimal balance = 0;
            Account account;
            Account sameNameAccount;
            bool isValid = false;

            while(true) 
            {
                Console.Write("Enter the name of the account: ");
                name = Console.ReadLine().TrimEnd();
                sameNameAccount = bank.GetAccount(name);
                if(sameNameAccount == null)
                    break;
                else
                {
                    Console.WriteLine("Oops! You set the same name with other account");
                    continue;
                }
            }

            while(!isValid)
            {
                try
                {
                    Console.Write("Enter the balance of the account: ");
                    balance = Convert.ToDecimal(Console.ReadLine());
                    if(balance >= 0)
                        isValid = true;
                    else
                        Console.WriteLine("Error: Your balance should be positive");
                } catch
                {
                    Console.WriteLine("Error: Your balance must be a number");
                }
            } 

            account = new Account(name, balance);
            bank.AddAccount(account);
        }

        static Account FindAccount(Bank bank)
        {
            String name;
            Account account;
            String answer;
            bool isValid = false;

            
            while(!isValid)
            {   
                Console.Write("Enter the name of the account you would like to do transation: ");
                name = Console.ReadLine();
                account = bank.GetAccount(name);
                if(account != null)
                { 
                    isValid = true;
                    return account;
                }
                else
                {
                    Console.WriteLine("The system cannot find the account with your given name.");
                    Console.Write("Would you like to keep searching? (y/N) ");
                    answer = Console.ReadLine();
                    if(answer.ToLower() == "n" || answer.ToLower() == "no") isValid = true;
                }
            } 
            return null;
        }

        static void DoPrint(Bank bank)
        {
            Account account = FindAccount(bank);
            if(account != null){
                account.Print();
                Console.ReadLine();
            }
        }

        static void DoQuit()
        {
            Console.WriteLine("Stop doing transaction.");
            Console.WriteLine("Have a good day!");
        }

        static bool askContinue(String hole)
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

        static void DoPrintHistoryTransaction(Bank bank)
        {   
            bool stop = false;
            Account account = FindAccount(bank);
            while(!stop) {
                Console.Clear();
                if(bank.Transactions.Count == 0) {
                    Console.WriteLine("Oops! No transaction are executed yet!");
                    stop = true;
                    Console.ReadLine();
                }
                else {
                    bank.PrintTransactionHistory(account);
                    Console.WriteLine();

                    bool isValid = false;
                    string ans;
                    int transactionNumber;
                    while(!isValid)
                    {
                        try
                        {
                            if(bank.TempTransactionList.Count != 0) {
                                Console.Write("Would you like to roll any transaction? (y/N) ");
                                ans = Console.ReadLine();
                                if(ans.Substring(0,1).ToLower() == "y")
                                {
                                    Console.Write("Enter the transaction number: ");
                                    transactionNumber = Convert.ToInt32(Console.ReadLine().Trim());
                                    DoRollback(bank, transactionNumber);
                                     
                                    Console.Write("Would you like to roll more transaction? (y/N) ");
                                    ans = Console.ReadLine();
                                    if(ans.Substring(0,1).ToLower() == "n")
                                    {
                                        isValid = true;
                                        stop = true;
                                    } else if(ans.Substring(0,1).ToLower() == "y")
                                        isValid = true;
                                } else if(ans.Substring(0,1).ToLower() == "n") {
                                        isValid = true;
                                        stop = true;
                                }
                            } else {
                                Console.WriteLine("Oops! Your account has no transaction are executed yet!");
                                isValid = true;
                                stop = true;
                                Console.ReadLine();
                            }
                        } 
                        catch(ArgumentOutOfRangeException)
                        {
                            Console.WriteLine("Your transaction number cannot greater than {0}", bank.TempTransactionList.Count);
                        }
                        catch
                        {
                            Console.WriteLine("Your transaction number should be an integer");
                        }  
                    }
                }
            }

            Console.Clear();
        }

        static void DoRollback(Bank bank, int number)
        {
            bank.RollbackTransaction(bank.TempTransactionList[number - 1]);
        }

        static void Main(string[] args)
        {
            Console.Clear();
            Bank bank = new Bank();
            MenuOption option;
            bool continueTransaction = true;
            
            while(continueTransaction)
            {
                if(bank.Accounts.Count == 0)
                {
                    Console.WriteLine("Oops! The bank system has no accounts. Let's create one!");
                    DoAddNewAccount(bank);
                } else
                {
                    option = (MenuOption)ReadUserOption();
                    switch (option)
                    {
                        case (MenuOption)0: 
                            DoWithdraw(bank); 
                            break;
                        case (MenuOption)1: 
                            DoDeposite(bank); 
                            break; 
                        case (MenuOption)2:
                            DoTransfer(bank);
                            break;
                        case (MenuOption)3: 
                            DoPrint(bank); 
                            break;
                        case (MenuOption)4:
                            DoAddNewAccount(bank);
                            break;
                        case (MenuOption)5:
                            DoPrintHistoryTransaction(bank);
                            break;
                        case (MenuOption)6:
                            continueTransaction = false;
                            DoQuit();
                            break;
                    }
                }
            }
        }
    }
}
