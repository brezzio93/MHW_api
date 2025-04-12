using System.Diagnostics.CodeAnalysis;

public class Armors
{
    public string Id { get; set; }
    public int IdCampaign { get; set; }
    public string ArmorName { get; set; }
    public int AmountCrafted { get; set; }
}

public class UpdateArmorRequest
{
    public string IdCampaign { get; set; }
    public ArmorJson ArmorJson { get; set; }
}
public class ArmorJson
{
    public string ArmorName { get; set; }
    public List<MaterialJson> Materials { get; set; }
}
