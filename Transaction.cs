using System;

namespace Bank
{
    abstract class Transaction
    {
        protected decimal _amount;
        protected Boolean _success = false;
        private Boolean _executed;
        private Boolean _reversed;
        private DateTime _dateStamp;

        abstract public bool Success{get;}
        public bool Executed { get => _executed; set => _executed = value; }
        public bool Reversed { get => _reversed; set => _reversed = value; }
        public DateTime DateStamp { get => _dateStamp; set => _dateStamp = value; }

        public Transaction(decimal amount)
        {
            _amount = amount;
        }

        abstract public void Print();

        public virtual void Execute()
        {
            if(Executed == true) throw new InvalidOperationException("This operation has been already attempted.");
        }

        public virtual void Rollback()
        {
            if(Executed == false) throw new InvalidOperationException("This operation has not been finalized");
            if(Reversed == true) throw new InvalidOperationException("This operation has been already reversed.");
        }
    }
}