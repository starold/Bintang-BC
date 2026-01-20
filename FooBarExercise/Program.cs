// Define rules: divisor dan output text
var rules = new List<(int divisor, string text)>
{
    (5, "Foo"),
    (3, "Bar"),
    (11, "Jazz"),
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
    
    // Jika tidak ada rule yang cocok, tampilkan angkanya
    Console.WriteLine(string.IsNullOrEmpty(result) ? i.ToString() : result);
}


