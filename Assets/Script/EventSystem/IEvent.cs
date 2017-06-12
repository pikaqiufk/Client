namespace EventSystem
{
    public interface IEvent
    {
        object Target { get; set; }
        string Type { get; set; }
    }
}