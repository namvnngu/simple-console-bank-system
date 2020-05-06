using System;

namespace Bank
{
    class WithdrawTransaction : Transaction
    {
        private Account _account;
        private decimal amount;
        public override bool Success { get => _success;}
        public Account Account { get => _account; set => _account = value; }
        public decimal Amount { get => amount; set => amount = value; }

        public WithdrawTransaction(Account account, decimal amount) : base(amount)
        {
            Account = account;
            Amount = amount;
        }

        public override void Print()
        {
            if(Executed)
                Console.WriteLine("The withdraw operation has been executed.");
            else
                Console.Write("");    

            if(_success)
                Console.WriteLine("The withdraw operation has been successfully completed.");
            else 
                Console.Write("");

            if(Reversed)
                Console.WriteLine("The withdraw operation has been successfully reversed.");
            else
                Console.Write("");
            
            if((Executed == true) && (_success == true))
            {
                Console.WriteLine("The amount was withdrawn from the account: " + Amount);
                Account.Print();
            }
        }

        public override void Execute()
        {  
            DateStamp = DateTime.Now;
            base.Execute();
            _success = Account.Withdraw(Amount);
            Executed = true;
        }

        public override void Rollback()
        {
            DateStamp = DateTime.Now;
            base.Rollback();
            if((_success == true) && (Reversed == false)) Reversed = Account.Deposit(Amount);
        }

    }
}