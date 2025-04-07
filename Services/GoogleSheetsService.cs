using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

public class GoogleSheetsService
{
    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private static readonly string ApplicationName = "MHW_api";
    private static readonly string SpreadsheetId = "1NJwEZ0YgfYzvQl0MV9qwqeHv2QfRruj3zrgbp2iJ0AY";
    private readonly SheetsService _service;

    public GoogleSheetsService()
    {
        var jsonCredentials = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIALS_JSON");

        if (string.IsNullOrWhiteSpace(jsonCredentials))
        {
            throw new Exception("No se encontró la variable de entorno 'GOOGLE_CREDENTIALS_JSON'.");
        }

        // Crear un archivo temporal con las credenciales
        var tempPath = Path.GetTempFileName();
        File.WriteAllText(tempPath, jsonCredentials);

        var credential = GoogleCredential.FromFile(tempPath).CreateScoped(Scopes);

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

    public Task<IList<IList<object>>> GetItemboxData() => GetSheetData("Itembox!A2:D100");
    public Task<IList<IList<object>>> GetMaterialsData() => GetSheetData("Materials!A2:C100");
    public Task<IList<IList<object>>> GetArmorsData() => GetSheetData("Armors!A2:D100");
    public Task<IList<IList<object>>> GetWeaponsData() => GetSheetData("Weapons!A2:D100");
    public Task<IList<IList<object>>> GetCampaignData() => GetSheetData("Campaign!A2:E100");

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
