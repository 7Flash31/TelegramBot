//using Newtonsoft.Json;
//using System.Net.Http.Json;
//using System.Text;

//class Kandinsky
//{
//    private const string ApiUrl = "https://api-key.fusionbrain.ai/";
//    private const string ApiKey = "B6D4FF68696AA68B9209AE777FFDE539";
//    private const string SecretKey = "FDA28CD77E172C032D736FD68E110068";

//    private readonly HttpClient client;

//    public Kandinsky()
//    {
//        client = new HttpClient();
//        client.DefaultRequestHeaders.Add("X-Key", $"Key {ApiKey}");
//        client.DefaultRequestHeaders.Add("X-Secret", $"Secret {SecretKey}");
//    }

//    public async Task<int> GetModelIdAsync()
//    {
//        var response = await client.GetAsync($"{ApiUrl}key/api/v1/models");
//        response.EnsureSuccessStatusCode();

//        var modelData = await response.Content.ReadFromJsonAsync<ModelData[]>();
//        return modelData[0].Id;
//    }

//    public async Task<byte[]> GenerateImageAsync(string prompt, int modelId, int width = 1024, int height = 1024, int numImages = 1)
//    {
//        var uuid = await GenerateImageUuidAsync(prompt, modelId, width, height, numImages);
//        if(uuid == null)
//        {
//            Console.WriteLine("Generation failed or timed out.");
//            return null;
//        }

//        return await GetImageAsync(uuid);
//    }

//    private async Task<string> GenerateImageUuidAsync(string prompt, int modelId, int width = 1024, int height = 1024, int numImages = 1)
//    {
//        var generateParams = new
//        {
//            type = "GENERATE",
//            numImages,
//            width,
//            height,
//            generateParams = new
//            {
//                query = prompt
//            }
//        };

//        var requestData = new
//        {
//            model_id = modelId,
//            @params = generateParams
//        };

//        var content = new MultipartFormDataContent();
//        content.Add(new StringContent(modelId.ToString()), "model_id");
//        content.Add(new StringContent(JsonConvert.SerializeObject(generateParams), Encoding.UTF8, "application/json"), "params");

//        var response = await client.PostAsync($"{ApiUrl}key/api/v1/text2image/run", content);
//        response.EnsureSuccessStatusCode();

//        var responseData = await response.Content.ReadFromJsonAsync<GenerateResponse>();
//        return responseData?.Uuid;
//    }

//    private async Task<byte[]> GetImageAsync(string uuid)
//    {
//        var attempts = 10;
//        var delayInSeconds = 10;

//        while(attempts > 0)
//        {
//            var response = await client.GetAsync($"{ApiUrl}key/api/v1/text2image/status/{uuid}");
//            response.EnsureSuccessStatusCode();

//            var data = await response.Content.ReadFromJsonAsync<StatusResponse>();
//            if(data.Status == "DONE")
//            {
//                return Convert.FromBase64String(data.Images[0]);
//            }

//            attempts--;
//            await Task.Delay(delayInSeconds * 1000);
//        }

//        Console.WriteLine("Image retrieval failed or timed out.");
//        return null;
//    }

//    class ModelData
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public double Version { get; set; }
//        public string Type { get; set; }
//    }

//    class GenerateResponse
//    {
//        public string Uuid { get; set; }
//        public string Status { get; set; }
//    }

//    class StatusResponse
//    {
//        public string Uuid { get; set; }
//        public string Status { get; set; }
//        public string[] Images { get; set; }
//        public string ErrorDescription { get; set; }
//        public bool Censored { get; set; }
//    }
//}

//class Program2
//{
//    static async Task Main()
//    {
//        var fusionBrainApiClient = new Kandinsky();

//        var modelId = await fusionBrainApiClient.GetModelIdAsync();
//        var imageBytes = await fusionBrainApiClient.GenerateImageAsync("Sun in sky", modelId);

//        if(imageBytes != null)
//        {
//            Console.WriteLine("Image retrieved successfully.");
//            string filePath = "C:\\Users\\arter\\Desktop\\BotLogs\\image.png";

//            File.WriteAllBytes(filePath, imageBytes);
//            Console.WriteLine($"Image saved to {filePath}");
//        }
//    }
//}
