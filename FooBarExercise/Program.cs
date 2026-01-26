
var rules = new List<(int divisor, string text)>
{
    (5, "Bar"),
    (3, "Foo"),
    (4, "Baz"),
    (9, "Huzz"),
    (7, "Buzz")
};

Console.Write("Masukkan angka: ");
int n = int.Parse(Console.ReadLine());

for (int i = 1; i <= n; i++)
{
    string result = "";
    
    
    foreach (var (divisor, text) in rules)
    {
        if (i % divisor == 0)
        {
            result += text;
        }
    }
    
    Console.WriteLine(string.IsNullOrEmpty(result) ? i.ToString() : result);
}


