namespace EScooter.Control.Application
{
    public record UploadDesiredDto(bool Locked, string UpdateFrequency, double MaxSpeed);
}
