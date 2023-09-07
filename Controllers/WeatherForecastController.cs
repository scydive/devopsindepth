using Microsoft.AspNetCore.Mvc;
using Azure;
using Azure.AI.OpenAI;
using System;
using System.IO;
using System.Text;
namespace devopsindepth.Controllers;

public class MyString
{
    public string me { get; set; }
    public string assistant { get; set; }
}

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    static List<MyString> myMessages = new List<MyString>();
    private string line = System.IO.File.ReadAllText("prompt/prompt.txt", Encoding.UTF8);
    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [ActionName("Post")]
    public async Task<MyString> ReturnDataAsync([FromBody] MyString data)
    {
        var envVars = EnvReader.Load(".env");
        var connectionString = Environment.GetEnvironmentVariable("AZURE_OPENAI_URI");
        var apiKey = Environment.GetEnvironmentVariable("AZURE_KEY_CREDENTIAL");

        ChatCompletionsOptions chatComplete = new ChatCompletionsOptions
        {
            Messages =
            {
                new ChatMessage(ChatRole.System, @""+ line),
            },
            Temperature = (float)0.2,
            MaxTokens = 350,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };
        OpenAIClient client = new OpenAIClient(
        new Uri(connectionString),
        new AzureKeyCredential(apiKey));

        chatComplete.Messages.Add(new ChatMessage(ChatRole.User, data.me));

        // ### If streaming is not selected
        Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(
            "devlinchat",
            chatComplete
            );

        var completions = responseWithoutStream.Value;

        chatComplete.Messages.Add(completions.Choices[0].Message);
        myMessages.Add(new MyString { me = data.me, assistant = completions.Choices[0].Message.Content });
        MyString questionOut = new MyString { me = data.me, assistant = completions.Choices[0].Message.Content };



        return questionOut;
    }

    [HttpGet]
    public List<MyString> ReturnChat()
    {
        foreach (var message in myMessages)
        {
            myMessages.Append(message);
        }

        return myMessages;
    }
}
