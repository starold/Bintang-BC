Console.Write("Masukkan angka: ");
int n = int.Parse(Console.ReadLine());

for (int i = 1; i <= n; i++)
{
    if (i % 3 == 0 && i % 5 == 0 && i % 7 == 0)
    {
        Console.WriteLine("FooBarJazz");
    }
    else if (i % 3 == 0 && i % 5 == 0)
    {
        Console.WriteLine("FooBar");
    }
    else if (i % 3 == 0 && i % 7 == 0)
    {
        Console.WriteLine("FooJazz");
    }
    else if (i % 5 == 0 && i % 7 == 0)
    {
        Console.WriteLine("BarJazz");
    }
    else if (i % 3 == 0)
    {
        Console.WriteLine("Foo");
    }
    else if (i % 5 == 0)
    {
        Console.WriteLine("Bar");
    }
    else if (i % 7 == 0)
    {
        Console.WriteLine("Jazz");
    }
    else
    {
        Console.WriteLine(i);
    }
}


