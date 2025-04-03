using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/armors")]
public class ArmorsController : ControllerBase
{
    private readonly GoogleSheetsService gss;
    public ArmorsController()
    {
        gss = new GoogleSheetsService();
    }

    [HttpGet("getArmors")]
    public async Task<IActionResult> GetData()
    {
        var armorsRaw = await gss.GetArmorsData();

        var armorsArray = new List<Armors>();
        foreach (var item in armorsRaw)
        {
            armorsArray.Add(new Armors
            {
                Id = item[0].ToString(),
                IdCampaign = item[1].ToString(),
                ArmorName = item[2].ToString(),
                AmountCrafted = int.TryParse(item[3].ToString(), out int aux) ? aux : 0
            });
        }

        return Ok(armorsArray);
    }

    [HttpPost("updateArmor")]
    public async Task<IActionResult> UpdateArmor([FromBody] UpdateArmorRequest request)
    {
        // Fetch raw data from both sheets
        var itemBoxData = await gss.GetItemboxData();
        var armorsData = await gss.GetArmorsData();

        Dictionary<string, int> materialStock = new Dictionary<string, int>();

        // Map itembox materials with their quantities
        foreach (var row in itemBoxData)
        {
            string materialName = row[2].ToString();
            int quantity = int.TryParse(row[3].ToString(), out int q) ? q : 0;
            materialStock[materialName] = quantity;
        }

        // Check if we have enough materials
        foreach (var material in request.ArmorJson.Materials)
        {
            if (!materialStock.ContainsKey(material.Material) || materialStock[material.Material] < material.Needed)
            {
                return BadRequest(new { message = $"Not enough {material.Material} to craft {request.ArmorJson.ArmorName}." });
            }
        }

        // Subtract materials from Itembox
        foreach (var material in request.ArmorJson.Materials)
        {
            int newQuantity = materialStock[material.Material] - material.Needed;

            for (int i = 0; i < itemBoxData.Count; i++)
            {
                if (itemBoxData[i][2].ToString() == material.Material)
                {
                    int rowIndex = i + 2;
                    await gss.UpdateCell($"Itembox!D{rowIndex}", newQuantity);
                    break;
                }
            }
        }

        // Update armor crafted count
        for (int i = 0; i < armorsData.Count; i++)
        {
            if (armorsData[i][2].ToString() == request.ArmorJson.ArmorName)
            {
                int rowIndex = i + 2;
                int currentCrafted = int.TryParse(armorsData[i][3].ToString(), out int crafted) ? crafted : 0;
                int newCrafted = currentCrafted + 1;

                await gss.UpdateCell($"armors!D{rowIndex}", newCrafted);
                return Ok(new { message = $"{request.ArmorJson.ArmorName} crafted successfully.", newCrafted });
            }
        }

        return NotFound(new { message = "Armor not found." });
    }

}