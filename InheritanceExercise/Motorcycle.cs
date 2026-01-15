public class Motorcycle : Vehicle
{
    public int EngineCapacity { get; init; }
    public override int GetMaxSpeed()
    {
        return EngineCapacity / 10;
    }
}