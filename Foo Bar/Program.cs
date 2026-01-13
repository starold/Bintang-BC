Console.Write("Masukkan angka: ");
int n = int.Parse(Console.ReadLine());

for (int i = 1; i <= n; i++)
{
    if (i % 3 == 0 && i % 5 == 0)
    {
        Console.WriteLine("FooBar");
    }
    else if (i % 3 == 0)
    {
        Console.WriteLine("Foo");
    }
    else if (i % 5 == 0)
    {
        Console.WriteLine("Bar");
    }
    else
    {
        Console.WriteLine(i);
    }
}


