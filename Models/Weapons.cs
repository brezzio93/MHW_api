using System.Diagnostics.CodeAnalysis;

public class Weapons
{
    public string Id { get; set; }
    public string IdCampaign { get; set; }
    public string WeaponName { get; set; }
    public bool WeaponCrafted { get; set; }
}

public class UpdateWeaponRequest
{
    public string IdCampaign { get; set; }
    public WeaponJson WeaponJson { get; set; }
}

public class WeaponJson
{
    public string WeaponName { get; set; }
    public List<MaterialJson> Materials { get; set; }
}