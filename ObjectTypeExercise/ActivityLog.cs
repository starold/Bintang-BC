public class ActivityLog
{
    private object[] data = new object[10];
    private int index = 0;
    public void Add(object item)
    {
        data[index++] = item;

    }
    public void PrintAll()
    {
        foreach (object item in data)
        {
            if (item == null) continue;

            Console.WriteLine($"Value : {item}");
            Console.WriteLine($"Type : {item.GetType().Name}");
            Console.WriteLine();
        }
    }
    public object GetAt(int i)
    {
        return data[i];
    }
}