namespace StringAndTextHandling
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-- String And Text Handling-");

            DemonstrateCharacterType();
            DemonstrateStringBasics();
            DemonstrateStringSearching();
            DemonstrateStringManipulation();
            DemonstrateStringSplittingAndJoining();
            DemonstrateStringInterpolationAndFormatting();
            // DemonstrateStringComparison();
            // DemonstrateStringBuilder();
            // DemonstrateTetxEncoding();

        }
        static void DemonstrateCharacterType()
        {
            
        }
        static void DemonstrateStringBasics()
        {
            // string empty = "";
            // string alsoEmpty = string.Empty;
            // string? nullString = null; // Explicitly nullable for modern C# nullable reference types

            // Console.WriteLine($"Empty string == \"\": {empty == ""}");
            // Console.WriteLine($"Empty string == string.Empty: {empty == string.Empty}");
            // Console.WriteLine($"Empty string length: {empty.Length}");

            // // Safe null checking - prevents NullReferenceException
            // Console.WriteLine($"Is null string null? {nullString == null}");
            // Console.WriteLine($"Is null string empty? {string.IsNullOrEmpty(nullString)}");
            // Console.WriteLine($"Is empty string null or empty? {string.IsNullOrEmpty(empty)}");

            // // Accessing characters within strings
            // string sample = "Programming";
            // Console.WriteLine($"Character at index 0: {sample[0]}");
            // Console.WriteLine($"Character at index 4: {sample[4]}");

            // // Iterating through string characters
            // Console.Write("Characters in '123': ");
            // foreach (char c in "")
            // {
            //     Console.Write($"{c},");
            // }
            // Console.WriteLine();
            // Console.WriteLine();
        }

        static void DemonstrateStringSearching()
        {
            //  // Remember: strings are immutable - each operation creates a new string
            // string original = "Hello World";

            // // Substring extraction
            // string left5 = original.Substring(0, 8);
            // string right5 = original.Substring(7);
            // Console.WriteLine($"Original: '{original}'");
            // Console.WriteLine($"Left 5 characters: '{left5}'");
            // Console.WriteLine($"From index 6 to end: '{right5}'");

            // // Insert and remove operations
            // string inserted = original.Insert(5, ",");
            // string removed = inserted.Remove(5, 1);
            // Console.WriteLine($"After inserting comma: '{inserted}'");
            // Console.WriteLine($"After removing comma: '{removed}'");

            // // Padding - useful for formatting output
            // string number = "123";
            // Console.WriteLine($"Right-padded: '{number.PadRight(10, '*')}'");
            // Console.WriteLine($"Left-padded: '{number.PadLeft(10, '0')}'");

            // // Trimming whitespace - essential for user input processing
            // string messy = "   Hello World   \t\r\n";
            // Console.WriteLine($"Original length: {messy.Length}");
            // Console.WriteLine($"Trimmed length: {messy.Trim().Length}");
            // Console.WriteLine($"Trimmed result: '{messy.Trim()}'");

            // // String replacement
            // string sentence = "I like cats and cats like me";
            // string replaced = sentence.Replace("cats", "dogs");
            // Console.WriteLine($"Original: '{sentence}'");
            // Console.WriteLine($"Replaced: '{replaced}'");

            // Console.WriteLine();
        }

        static void DemonstrateStringManipulation()
        {
            
        }
        static void DemonstrateStringSplittingAndJoining()
        {
            // Console.WriteLine("5. STRING SPLITTING AND JOINING DEMONSTRATION");
            // Console.WriteLine("=============================================");

            // // Splitting strings - fundamental for data processing
            // string sentence = "The quick brown fox jumps";
            // string[] words = sentence.Split();
            
            // Console.WriteLine($"Original sentence: '{sentence}'");
            // Console.Write("Words: ");
            // foreach (string word in words)
            // {
            //     Console.Write($"'{word}' ");
            // }
            // Console.WriteLine();

            // // Splitting with custom delimiters
            // string csvData = "apple,banana,cherry,date";
            // string[] fruits = csvData.Split(',');
            // Console.WriteLine($"CSV data: '{csvData}'");
            // Console.WriteLine($"Number of fruits: {fruits.Length}");

            // // Joining strings back together
            // string rejoined = string.Join(" ", words);
            // string csvRejoined = string.Join(" | ", fruits);
            
            // Console.WriteLine($"Rejoined with spaces: '{rejoined}'");
            // Console.WriteLine($"Fruits joined with pipes: '{csvRejoined}'");

            // Console.WriteLine();
        }
        static void DemonstrateStringInterpolationAndFormatting()
        {
            // // String interpolation - modern and readable way to build strings
            // string name = "Alice";
            // int age = 25;
            // DateTime today = DateTime.Now;

            // string interpolated = $"Hello, my name is {name} and I'm {age} years old.";
            // string withDate = $"Today is {today.DayOfWeek}, {today:yyyy-MM-dd}";
            
            // Console.WriteLine(interpolated);
            // Console.WriteLine(withDate);

            // // Traditional string formatting - still useful for complex scenarios
            // string template = "It's {0} degrees in {1} on this {2} morning";
            // string formatted = string.Format(template, 25, "Jakarta", today.DayOfWeek);
            // Console.WriteLine(formatted);

            // // Format specifiers for numbers and dates
            // double price = 19.99;
            // Console.WriteLine($"Price: {price:C}"); // Currency format
            // Console.WriteLine($"Percentage: {0.85:P}"); // Percentage format
            // Console.WriteLine($"Date: {today:dddd, MMMM dd, yyyy}"); // Long date format

            // Console.WriteLine();
        }
        static void DemonstrateStringComparison()
        {
               Console.WriteLine("7. STRING COMPARISON DEMONSTRATION");
            Console.WriteLine("==================================");

            string str1 = "Hello";
            string str2 = "hello";
            string str3 = "Hello";

            // Default equality comparison - ordinal, case-sensitive
            Console.WriteLine("=== EQUALITY COMPARISON ===");
            Console.WriteLine($"'{str1}' == '{str3}': {str1 == str3}");
            Console.WriteLine($"'{str1}' == '{str2}': {str1 == str2}");
            Console.WriteLine($"'{str1}'.Equals('{str2}'): {str1.Equals(str2)}");

            // StringComparison enum - gives you full control over comparison behavior
            Console.WriteLine("\n=== STRING COMPARISON OPTIONS ===");
            Console.WriteLine($"Ordinal (default): {string.Equals(str1, str2, StringComparison.Ordinal)}");
            Console.WriteLine($"OrdinalIgnoreCase: {string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase)}");
            Console.WriteLine($"CurrentCulture: {string.Equals(str1, str2, StringComparison.CurrentCulture)}");
            Console.WriteLine($"CurrentCultureIgnoreCase: {string.Equals(str1, str2, StringComparison.CurrentCultureIgnoreCase)}");
            Console.WriteLine($"InvariantCulture: {string.Equals(str1, str2, StringComparison.InvariantCulture)}");
            Console.WriteLine($"InvariantCultureIgnoreCase: {string.Equals(str1, str2, StringComparison.InvariantCultureIgnoreCase)}");

            // Order comparison - for sorting and alphabetical ordering
            Console.WriteLine("\n=== ORDER COMPARISON ===");
            string[] words = { "apple", "Banana", "cherry", "Date" };
            Console.WriteLine("Original order: " + string.Join(", ", words));

            // Default culture-sensitive comparison
            Array.Sort(words, string.Compare);
            Console.WriteLine("Culture sort: " + string.Join(", ", words));

            // Reset array
            words = new[] { "apple", "Banana", "cherry", "Date" };
            
            // Ordinal comparison - treats characters as their numeric Unicode values
            Array.Sort(words, StringComparer.Ordinal);
            Console.WriteLine("Ordinal sort: " + string.Join(", ", words));

            // Case-insensitive ordinal comparison
            Array.Sort(words, StringComparer.OrdinalIgnoreCase);
            Console.WriteLine("Ordinal ignore case: " + string.Join(", ", words));

            // CompareTo examples - returns negative, zero, or positive
            Console.WriteLine("\n=== COMPARETO EXAMPLES ===");
            Console.WriteLine($"'Boston'.CompareTo('Austin'): {string.Compare("Boston", "Austin")}");
            Console.WriteLine($"'Boston'.CompareTo('Boston'): {string.Compare("Boston", "Boston")}");
            Console.WriteLine($"'Boston'.CompareTo('Chicago'): {string.Compare("Boston", "Chicago")}");
            
            // Ordinal vs Culture demonstration
            Console.WriteLine("\n=== ORDINAL VS CULTURE COMPARISON ===");
            string a = "Atom";
            string b = "atom";
            Console.WriteLine($"Ordinal: '{a}' vs '{b}' = {string.Compare(a, b, StringComparison.Ordinal)}");
            Console.WriteLine($"Culture: '{a}' vs '{b}' = {string.Compare(a, b, StringComparison.CurrentCulture)}");
            Console.WriteLine("Note: Ordinal treats 'A' (65) and 'a' (97) by Unicode values");
            Console.WriteLine("Culture comparison considers language rules for proper alphabetical ordering");

            Console.WriteLine();
        }
    }
}