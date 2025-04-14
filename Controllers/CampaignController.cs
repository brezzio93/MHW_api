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
                    CampaignName = item[1].ToString(),
                });
            }
        }

        return Ok(campaigns);
    }

    [HttpGet("getMissionLogs/{id}")]
    public async Task<IActionResult> getMissionLogs(int id)
    {

        var rawData = await gss.GetMissionLogData();
        var missionLog = new List<MissionLog>();

        foreach (var item in rawData)
        {
            if (id == int.Parse(item[0].ToString()))
            {
                missionLog.Add(new MissionLog
                {
                    Days = int.Parse(item[2].ToString()),
                    Mission = item[3].ToString(),
                    MissionStatus = item[4].ToString(),
                });
            }
        }

        return Ok(missionLog);
    }
}