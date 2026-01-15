public class BankAccount
{
    // static field
    public static int TotalAccounts { get; private set; }

    // readonly field
    public readonly string AccountNumber;

    // private field (encapsulation)
    private decimal _balance;

    // property
    public string Owner { get; init; }
    public decimal Balance => _balance; // read-only property

    // constructor
    public BankAccount(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        _balance = initialBalance;
        TotalAccounts++;
    }

    // method: setor
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Jumlah setor harus lebih dari 0");

        _balance += amount;
    }

    // method: tarik
    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Jumlah tarik harus lebih dari 0");

        if (amount > _balance)
            throw new InvalidOperationException("Saldo tidak mencukupi");

        _balance -= amount;
    }

    // deconstructor
    public void Deconstruct(out string owner, out decimal balance)
    {
        owner = Owner;
        balance = _balance;
    }
}