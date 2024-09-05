namespace DryCleaning.Domain
{
    public class OpenedInterval // si no fuese singleton sino que hubiese un backend, sería mejor definirlo como "public readonly record struct OpenedInterval"
    {
        public TimeOnly Open { get; set; }
        public TimeOnly Close { get; set; }
    }
}
