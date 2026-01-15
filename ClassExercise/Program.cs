class Program
{
    static void Main()
    {
        var account = new BankAccount("123-456", 1_000_000m)
        {
            Owner = "Bintang"
        };

        account.Deposit(500_000m);
        account.Withdraw(200_000m);

        Console.WriteLine($"Saldo akhir: {account.Balance}");

        // Deconstruct
        var (owner, balance) = account;
        Console.WriteLine($"{owner} memiliki saldo {balance}");

        Console.WriteLine($"Total akun: {BankAccount.TotalAccounts}");
    }
}