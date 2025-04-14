using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/campaign")]
public class CampaignController : ControllerBase
{
    private readonly GoogleSheetsService gss;
    public CampaignController(){
        gss = new GoogleSheetsService();
    }
    [HttpGet("getCampaign/{id}")]
    public async Task<IActionResult> GetData(){
        return Ok("Beep");
    }
}