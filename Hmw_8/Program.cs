using System;

public enum Type
{
    Checking,
    Savings
}

public class BankAccount
{
    private static int counterId = 0;

    private int number;
    private double balance;
    private Type accountType;

    private static int GenerateNumber()
    {
        return ++counterId;
    }

    // Конструктор по умолчанию
    public BankAccount() : this(0.0, Type.Checking) { }

    // Конструктор для заполнения баланса
    public BankAccount(double balance) : this(balance, Type.Checking) { }

    // Конструктор для заполнения типа счёта
    public BankAccount(Type accountType) : this(0.0, accountType) { }

    // Конструктор для заполнения баланса и типа
    public BankAccount(double balance, Type accountType)
    {
        this.number = GenerateNumber();
        this.balance = balance;
        this.accountType = accountType;
    }

    public void AddMoney(double amount)
    {
        this.balance += amount;
    }

    public void WithdrawMoney(double amount)
    {
        if (amount <= this.balance)
        {
            this.balance -= amount;
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

        this.balance -= amount;
        recipient.balance += amount;

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