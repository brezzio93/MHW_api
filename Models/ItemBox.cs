public class ItemBox
{
    public string Id { get; set; }
    public string IdCampaign { get; set; }
    // public string IdMaterial { get; set; }
    public string MaterialName { get; set; }
    public int MaterialQuantity { get; set; }
}
public class UpdateItemboxRequest
{
    public string IdCampaign { get; set; }
    public string MaterialName { get; set; }
    public int AddedAmount { get; set; }
}
public class MaterialJson
{
    public string Material { get; set; }
    public int Needed { get; set; }
}