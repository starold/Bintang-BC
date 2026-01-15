public class User
{
    public string Name { get; set;}
    public override string ToString()
    {
        return $"User: {Name}";

    }
    
}