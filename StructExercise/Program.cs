struct Point
{
    public int X;
    public int Y;
}
class Program
{
    static void Main()
    {
        Point p1 = new Point{ X = 10, Y = 20, };
        Point p2=p1;
        p2.X = 99;
        Console.WriteLine(p1.X);
        Console.WriteLine(p2.X);
    }
    
}