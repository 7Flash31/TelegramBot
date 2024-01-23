using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

class Text2ImageAPI
{
    //static async Task Main()
    //{
    //    string url = "https://stablediffusionapi.com/api/v3/text2img";

    //    string payload = @"{
    //        ""key"": ""JJUseY6pwiEi9Bx3GPpg8zcSBaSt3gZhWGFA3mO3i9O607euKyZ0VX7JBjTk"",
    //        ""prompt"": ""Шрек"",
    //        ""negative_prompt"": null,
    //        ""width"": ""512"",
    //        ""height"": ""512"",
    //        ""samples"": ""1"",
    //        ""num_inference_steps"": ""20"",
    //        ""seed"": null,
    //        ""guidance_scale"": 7.5,
    //        ""safety_checker"": ""yes"",
    //        ""multi_lingual"": ""no"",
    //        ""panorama"": ""no"",
    //        ""self_attention"": ""no"",
    //        ""upscale"": ""no"",
    //        ""embeddings_model"": null,
    //        ""webhook"": null,
    //        ""track_id"": null
    //    }";

    //    using(HttpClient client = new HttpClient())
    //    {
    //        var content = new StringContent(payload, Encoding.UTF8, "application/json");
    //        var response = await client.PostAsync(url, content);

    //        if(response.IsSuccessStatusCode)
    //        {
    //            string jsonResponse = await response.Content.ReadAsStringAsync();
    //            Console.WriteLine(jsonResponse);
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Error: {response.StatusCode}");
    //        }
    //    }
    //}
}