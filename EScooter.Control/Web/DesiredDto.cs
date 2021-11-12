namespace EScooter.Control.Application
{
    public record DesiredDto(bool Locked, string UpdateFrequency, double MaxSpeed);
}
