namespace SteelWare.Domain;

public class SteelRoll
{
    public int Id { get; set; }

    public float Length { get; set; }

    public float Weight { get; set; }

    public DateOnly AddedFrom { get; set; }

    public DateOnly AddedTo { get; set; }

    public DateOnly DeletedAt { get; set; }

    public DateOnly DeletedTo { get; set; }
}