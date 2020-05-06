using System;

namespace Bank 
{
    class Account {
        private String _name;
        private decimal _balance;

        public String Name { get => _name;}
        public decimal Balance { get => _balance;}

        public Account(String name, decimal balance) {
            _name = name;
            _balance = balance;
        }

        public bool Deposit(decimal amount) {
            if(amount > 0)
            {
                _balance += amount;
                return true;
            } else 
            {
                return false;
            }
        }
        
        public bool Withdraw(decimal amount) {
            if((_balance < amount) || (amount <= 0))
            {
                return false;
            } else
            {
                _balance -= amount;
                return true;
            }
        }

        public void Print() {
            Console.WriteLine("The account name: " + _name);
            Console.WriteLine("The account balance " + _balance.ToString("C"));
        }
    }
}
