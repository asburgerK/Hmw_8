using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;


public enum Type
{
    Checking,
    Savings
}

public enum MoneyOperation
{
    Add,
    Withdraw,
    Transfer
}

public class BankTransaction
{
    public readonly DateTime time;
    public readonly double amount;
    public readonly MoneyOperation type;

    public readonly int recipientNumber;
    public readonly int senderNumber;


    public BankTransaction(double amount, MoneyOperation type)
    {
        this.time = DateTime.Now;
        this.amount = amount;
        this.type = type;
    }
    public BankTransaction(double amount, MoneyOperation type, int recipientNumber, int senderNumber)
    {
        this.time = DateTime.Now;
        this.amount = amount;
        this.type = type;
        this.senderNumber = senderNumber;
        this.recipientNumber = recipientNumber;
    }



}

public class BankAccount
{
    private static int counterId = 0;

    private int number;
    private double balance;
    private Type accountType;
    private Queue<BankTransaction> transactions = new Queue<BankTransaction>();
    private MoneyOperation type;
    private static int GenerateNumber()
    {
        return ++counterId;
    }


    public BankAccount() : this(0.0, Type.Checking) { }

    public BankAccount(double balance) : this(balance, Type.Checking) { }


    public BankAccount(Type accountType) : this(0.0, accountType) { }


    public BankAccount(double balance, Type accountType)
    {
        this.number = GenerateNumber();
        this.balance = balance;
        this.accountType = accountType;
    }



    public void AddMoney(double amount)
    {
        type = MoneyOperation.Add;
        this.balance += amount;
        transactions.Enqueue(new BankTransaction(amount, type));
    }

    public void WithdrawMoney(double amount)
    {
        if (amount <= this.balance)
        {
            type = MoneyOperation.Withdraw;
            this.balance -= amount;
            transactions.Enqueue(new BankTransaction(amount, type));
        }
        else
        {
            Console.WriteLine("Вы нищий :(");
        }
    }

    public void Print()
    {
        Console.WriteLine("Account Number: " + this.number);
        Console.WriteLine("Account Balance: " + this.balance);
        Console.WriteLine("Account Type: " + this.accountType);
    }

    public bool Transfer(BankAccount recipient, double amount)
    {

        if (recipient == null)
        {
            Console.WriteLine("Счёт получателя не существует.");
            return false;
        }

        if (recipient == this)
        {
            Console.WriteLine("Нельзя перевести деньги на тот же счёт.");
            return false;
        }

        if (amount <= 0)
        {
            Console.WriteLine("Сумма перевода должна быть положительной.");
            return false;
        }

        if (amount > this.balance)
        {
            Console.WriteLine("Недостаточно средств для перевода. Вы нищий :(");
            return false;
        }
        type = MoneyOperation.Transfer;
        transactions.Enqueue(new BankTransaction(amount, type, recipient.number, this.number));

        this.balance -= amount;
        recipient.balance += amount;
        recipient.transactions.Enqueue(new BankTransaction(amount, type, recipient.number, this.number));
        Console.WriteLine($"Перевод {amount} со счёта #{this.number} на счёт #{recipient.number} выполнен успешно.");
        return true;
    }


}

public class Program
{
    public static void Main(string[] args)
    {
        BankAccount account1 = new BankAccount(1000.0, Type.Checking);
        account1.Print();
        Console.WriteLine();

        BankAccount account2 = new BankAccount(5000.0, Type.Savings);
        account2.Print();

        account1.AddMoney(500.0);
        account1.Print();

        account2.WithdrawMoney(10000.0);
    }
}
