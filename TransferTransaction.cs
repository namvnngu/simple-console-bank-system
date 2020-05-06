using System;

namespace Bank
{
    class TransferTransaction : Transaction
    {
        private Account _fromAccount, _toAccount;
        private DepositTransaction _deposit;
        private WithdrawTransaction _withdraw;
        private decimal amount;

        public Account FromAccount { get => _fromAccount; set => _fromAccount = value; }
        public Account ToAccount { get => _toAccount; set => _toAccount = value; }
        public override bool Success {get => _success;}
        public decimal Amount { get => amount; set => amount = value; }

        public TransferTransaction(Account fromAccount, Account toAccount, decimal amount) : base(amount)
        {
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
            _withdraw = new WithdrawTransaction(fromAccount, amount);
            _deposit = new DepositTransaction(toAccount, amount);
        }
        
        public override void Print()
        {
            if(Executed)
                Console.WriteLine("The transfer operation has been executed.");
            else
                Console.Write("");    

            if(_success)
                Console.WriteLine("The transfer operation has been successfully completed.");
            else 
                Console.Write("");

            if(Reversed)
                Console.WriteLine("The transfer operation has been successfully reversed.");
            else
                Console.Write("");
            
            if((Executed == true) && (_success == true))
            {
                Console.WriteLine();
                Console.WriteLine("Transfer {0} from {1}'s account to {2}'s account", Amount.ToString("C"), FromAccount.Name, ToAccount.Name);

                Console.WriteLine();
                Console.WriteLine("{0}'s account: (Sender)", FromAccount.Name);
                _withdraw.Print();

                Console.WriteLine();
                Console.WriteLine("{0}'s account: (Receiver)", ToAccount.Name);
                _deposit.Print();
            }
        }
        
        public override void Execute()
        {
            DateStamp = DateTime.Now;
            base.Execute(); 
            _withdraw.Amount = Amount;
            _deposit.Amount = Amount;

            if(FromAccount.Balance >= Amount)
            {
                _withdraw.Execute();
                if(_withdraw.Executed && _withdraw.Success)
                {
                    _deposit.Execute();
                    if(_deposit.Executed && _deposit.Success)
                    {
                        Executed = true;
                        _success = true;
                    } else
                    {    
                        _deposit.Rollback();
                        _withdraw.Rollback();
                        Console.WriteLine("Transfer failed.");
                    }
                } else
                {
                    _withdraw.Rollback();
                    Console.WriteLine("Transfer failed");
                }

            } else
                throw new InvalidOperationException("Transfer failed: The transfer amount cannot be larger than your balance");
        }

        public override void Rollback()
        {
            DateStamp = DateTime.Now;
            base.Rollback();
            if(ToAccount.Balance < Amount) throw new InvalidOperationException("The sender has insufficient funds to do rollback.");
            if((_success == true) && (Reversed == false))
            {
                _deposit.Rollback();
                _withdraw.Rollback();
                Reversed = true;
            }
        }

    }
    // class TransferTransaction
    // {
    //     private Account _fromAccount, _toAccount;
    //     private decimal Amount;
    //     private DepositTransaction _deposit;
    //     private WithdrawTransaction _withdraw;
    //     private bool _executed, _success, _reversed;

    //     public bool Executed { get => _executed;}
    //     public bool Success { get => _success;}
    //     public bool Reversed { get => _reversed;}
    //     public decimal Amount { get => Amount; set => Amount = value; }
    //     internal Account FromAccount { get => _fromAccount; set => _fromAccount = value; }
    //     internal Account ToAccount { get => _toAccount; set => _toAccount = value; }

    //     public TransferTransaction(Account fromAccount, Account toAccount, decimal amount)
    //     {
    //         FromAccount = fromAccount;
    //         ToAccount = toAccount;
    //         Amount = amount;
    //         _withdraw = new withdrawtransaction(fromaccount, amount);
    //         _deposit = new deposittransaction(toaccount, amount);
    //     }

    //     public void Print()
    //     {
    //         if(_executed)
    //             Console.WriteLine("The transfer operation has been executed.");
    //         else
    //             Console.Write("");    

    //         if(_success)
    //             Console.WriteLine("The transfer operation has been successfully completed.");
    //         else 
    //             Console.Write("");

    //         if(_reversed)
    //             Console.WriteLine("The transfer operation has been successfully reversed.");
    //         else
    //             Console.Write("");
            
    //         if((_executed == true) && (_success == true))
    //         {
    //             Console.WriteLine();
    //             Console.WriteLine("Transfer {0} from {1}'s account to {2}'s account", Amount.ToString("C"), FromAccount.Name, ToAccount.Name);

    //             Console.WriteLine();
    //             Console.WriteLine("{0}'s account: (Sender)", FromAccount.Name);
    //             _withdraw.Print();

    //             Console.WriteLine();
    //             Console.WriteLine("{0}'s account: (Receiver)", ToAccount.Name);
    //             _deposit.Print();
    //         }
    //     }
        
    //     public void Execute()
    //     {
    //         _withdraw.Amount = Amount;
    //         _deposit.Amount = Amount;

    //         if(_executed == true) throw new InvalidOperationException("This transfer operation has been already attempted.");
    //         if(FromAccount.Balance >= Amount)
    //         {
    //             _withdraw.Execute();
    //             if(_withdraw.Executed && _withdraw.Success)
    //             {
    //                 _deposit.Execute();
    //                 if(_deposit.Executed && _deposit.Success)
    //                 {
    //                     _executed = true;
    //                     _success = true;
    //                 } else
    //                 {    
    //                     _deposit.Rollback();
    //                     _withdraw.Rollback();
    //                     Console.WriteLine("Transfer failed.");
    //                 }
    //             } else
    //             {
    //                 _withdraw.Rollback();
    //                 Console.WriteLine("Transfer failed");
    //             }

    //         } else
    //             throw new InvalidOperationException("Transfer failed: The transfer amount cannot be larger than your balance");
    //     }

    //     public void Rollback()
    //     {
    //         if(_executed == false) throw new InvalidOperationException("The transfer operation has not been finalized");
    //         if(_reversed == true) throw new InvalidOperationException("The transfer operation has been already reversed.");
    //         if(ToAccount.Balance < Amount) throw new InvalidOperationException("The sender has insufficient funds to do rollback.");
    //         if((_success == true) && (_reversed == false))
    //         {
    //             _deposit.Rollback();
    //             _withdraw.Rollback();
    //             _reversed = true;
    //         }
    //     }
    // }
}