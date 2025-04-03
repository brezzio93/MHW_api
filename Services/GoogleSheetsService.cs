using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class GoogleSheetsService
{
    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

    private static readonly string ApplicationName = "MHW_api";
    private static readonly string SpreadsheetId = "1NJwEZ0YgfYzvQl0MV9qwqeHv2QfRruj3zrgbp2iJ0AY";
    private static readonly string CredentialPath = "mhw-db-455000-7628e6edfaa5.json";
    private readonly SheetsService _service;

    public GoogleSheetsService()
    {
        GoogleCredential credential;
        using (var stream = new FileStream(CredentialPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        _service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }
    public async Task<IList<IList<object>>> GetSheetData(string range)
    {
        var request = _service.Spreadsheets.Values.Get(SpreadsheetId, range);
        ValueRange response = await request.ExecuteAsync();
        return response.Values ?? new List<IList<object>>();
    }
    public async Task<IList<IList<object>>> GetItemboxData()
    {
        return await GetSheetData("Itembox!A2:D100"); // Fetch only Itembox
    }
    public async Task<IList<IList<object>>> GetMaterialsData()
    {
        return await GetSheetData("Materials!A2:B100"); // Fetch only Materials
    }
    public async Task<IList<IList<object>>> GetArmorsData()
    {
        return await GetSheetData("Armors!A2:D100"); // Fetch only Armors
    }
    public async Task<IList<IList<object>>> GetWeaponsData()
    {
        return await GetSheetData("Weapons!A2:D100"); // Fetch only Weapons
    }
    public async Task<IList<IList<object>>> GetCampaignData()
    {
        return await GetSheetData("Campaign!A2:E100"); // Fetch only Campaign
    }
    public async Task<bool> UpdateCell(string range, object newValue)
    {
        var valueRange = new ValueRange
        {
            Values = new List<IList<object>> { new List<object> { newValue } }
        };

        var updateRequest = _service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

        var updateResponse = await updateRequest.ExecuteAsync();
        return updateResponse.UpdatedCells > 0;
    }

}
