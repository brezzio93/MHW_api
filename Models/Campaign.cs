public class Campaign
{
    public int IdCampaign { get; set; }
    public string CampaignName { get; set; }
}

public class MissionLog
{
    public int IdCampaign { get; set; }
    public string CampaignName { get; set; }
    public int Days { get; set; }
    public string Mission { get; set; }
    public string MissionStatus { get; set; }
}