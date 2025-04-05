using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/weapons")]
public class WeaponsController : ControllerBase
{
    private readonly GoogleSheetsService gss;
    public WeaponsController()
    {
        gss = new GoogleSheetsService();
    }

    [HttpGet("getWeapons")]
    public async Task<IActionResult> GetData()
    {
        var weaponsRaw = await gss.GetWeaponsData();
        var returnData = new List<Weapons>();

        foreach (var item in weaponsRaw)
        {
            returnData.Add(new Weapons
            {
                Id = item[0].ToString(),
                IdCampaign = item[1].ToString(),
                WeaponName = item[2].ToString(),
                WeaponCrafted = bool.TryParse(item[3].ToString(), out bool aux) ? aux : false
            });
        }

        return Ok(returnData);
    }

    [HttpPost("updateWeapon")]
    public async Task<IActionResult> UpdateWeapon([FromBody] UpdateWeaponRequest request)
    {
        // Fetch raw data from both sheets
        var itemBoxData = await gss.GetItemboxData();
        var weaponsData = await gss.GetWeaponsData();

        Dictionary<string, int> materialStock = new Dictionary<string, int>();

        // Map itembox materials with their quantities
        foreach (var row in itemBoxData)
        {
            string materialName = row[2].ToString();
            int quantity = int.TryParse(row[3].ToString(), out int q) ? q : 0;
            materialStock[materialName] = quantity;
        }

        // Check if we have enough materials
        foreach (var material in request.WeaponJson.Materials)
        {
            if (!materialStock.ContainsKey(material.Material) || materialStock[material.Material] < material.Needed)
            {
                return BadRequest(new { message = $"Not enough {material.Material} to craft {request.WeaponJson.WeaponName}." });
            }
        }

        // Subtract materials from Itembox
        foreach (var material in request.WeaponJson.Materials)
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
        for (int i = 0; i < weaponsData.Count; i++)
        {
            if (weaponsData[i][2].ToString() == request.WeaponJson.WeaponName)
            {
                int rowIndex = i + 2;
                bool isCrafted = bool.TryParse(weaponsData[i][3].ToString(), out bool aux) ? true : false;
                await gss.UpdateCell($"Weapons!D{rowIndex}", isCrafted);
                break;
            }
        }


        return Ok(new { message = "Weapon updated successfully." });
    }

}