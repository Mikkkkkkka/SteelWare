namespace SteelWare.Domain;

public class SteelRoll
{
    public int Id { get; set; }

    public float Length { get; set; }

    public float Weight { get; set; }

    public DateTime AddedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}