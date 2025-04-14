using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/campaign")]
public class CampaignController : ControllerBase
{
    private readonly GoogleSheetsService gss;
    public CampaignController()
    {
        gss = new GoogleSheetsService();
    }
    
    [HttpGet("getCampaign/{id}")]
    public async Task<IActionResult> GetData(int id)
    {

        var rawData = await gss.GetCampaignData();
        var campaigns = new List<Campaign>();

        foreach (var item in rawData)
        {
            if (id == int.Parse(item[0].ToString()))
            {
                campaigns.Add(new Campaign
                {
                    IdCampaign = int.Parse(item[0].ToString()),
                    CampaginName = item[1].ToString(),
                });
            }
        }

        return Ok(campaigns);
    }
}