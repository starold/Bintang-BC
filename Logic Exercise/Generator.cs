public class RuleGenerator
{
    private Dictionary<int, string> _rules;

    public RuleGenerator()
    {
        _rules = new Dictionary<int, string>();
    }

    public void AddNewRule(int number, string output)
    {
        _rules.Add(number, output);
    }

    public string GenerateText(int number)
    {
        string result = "";
        foreach (var item in _rules)
        {
            if (number % item.Key == 0)
            {
                result += item.Value;
            }
        }

        if (result == "")
        {
            return number.ToString();
        }
        return result;
    }

    public void GenerateRange(int input)
    {
        for (int start = 1; start <= input; start++)
        {
            string result = GenerateText(start);
            Console.WriteLine(result);
        }
    }

    public void InputNumber(){
        Console.WriteLine("Masukkan number: ");
        int userInput = Convert.ToInt32(Console.ReadLine());
        GenerateRange(userInput);
    }


}