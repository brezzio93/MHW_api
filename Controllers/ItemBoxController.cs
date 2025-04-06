using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/itemBox")]
public class ItemBoxController : ControllerBase
{
    private readonly GoogleSheetsService gss;

    public ItemBoxController()
    {
        gss = new GoogleSheetsService();
    }

    [HttpGet("getItemBox")]
    public async Task<IActionResult> GetData()
    {
        try{
        // Fetch raw data from both sheets
        var itemboxData = await gss.GetItemboxData();
        var returnData = new List<ItemBox>();

        foreach (var row in itemboxData)
        {
            returnData.Add(new ItemBox
            {
                Id = row[0].ToString(),
                IdCampaign = row[1].ToString(),
                MaterialName = row[2]?.ToString(),
                MaterialQuantity = int.TryParse(row[3].ToString(), out int quantity) ? quantity : 0,
            });
        }

        return Ok(returnData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching data from Google Sheets.", error = ex.Message });
        }
    }

    [HttpPost("updateItembox")]
    public async Task<IActionResult> UpdateItembox([FromBody] UpdateItemboxRequest request)
    {
        var itemboxData = await gss.GetItemboxData();
        int rowIndex = -1;

        for (int i = 0; i < itemboxData.Count; i++)
        {
            var row = itemboxData[i];

            if (row[1]?.ToString() == request.IdCampaign && row[2]?.ToString() == request.MaterialName)
            {
                rowIndex = i + 2; // Convert to sheet row (A2 -> row 2)
                break;
            }
        }

        if (rowIndex == -1)
        {
            return NotFound(new { message = "Item not found." });
        }

        int currentQuantity = int.TryParse(itemboxData[rowIndex - 2][3]?.ToString(), out int quantity) ? quantity : 0;
        int newQuantity = currentQuantity + request.AddedAmount;

        bool success = await gss.UpdateCell($"Itembox!D{rowIndex}", newQuantity);

        if (success)
        {
            return Ok(new { message = "Itembox updated successfully.", newQuantity });
        }

        return StatusCode(500, new { message = "Failed to update Itembox." });
    }
}
