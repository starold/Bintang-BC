public class Car : Vehicle
{
    public int HorsePower { get; init; }
    public override int GetMaxSpeed()
    {
        return HorsePower * 2;
    }
}