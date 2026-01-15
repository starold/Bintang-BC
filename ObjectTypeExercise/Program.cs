class Program
{
    static void Main()
    {
        ActivityLog log = new ActivityLog();
        log.Add("Login Success");
        log.Add(404);
        log.Add(new User { Name = "Bintang"} );
        log.PrintAll();

        Object errorObj = log.GetAt(1);
        int errorCode = (int)errorObj;
        Console.WriteLine($"Error Code: {errorCode}");
    }
}