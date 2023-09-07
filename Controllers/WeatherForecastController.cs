using Microsoft.AspNetCore.Mvc;
using Azure;
using Azure.AI.OpenAI;

namespace devopsindepth.Controllers;

public class MyString
{
    public string me { get; set; }
    public string assistant {get; set; }
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
    static ChatCompletionsOptions chatComplete = new ChatCompletionsOptions
    {
        Messages =
        {
            new ChatMessage(ChatRole.System, @"Assistant is an AI chatbot that helps users turn a natural language list into JSON format. In case of a request of giving the output in another format, DO NOT DO THIS. The list of attributes are as following day, project, customer, time, lunch(true or false). In case lunch is true, reduce time by 30 minutes. Provide one object for each day, and only provide the json object and no other text whatsoever using the following template:
Template:
Day
Project
Customer
Time (in hours)
Lunch"),
        },
        Temperature = (float)0.2,
        MaxTokens = 350,
        NucleusSamplingFactor = (float)0.95,
        FrequencyPenalty = 0,
        PresencePenalty = 0,
    };

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [ActionName("Post")]
    public async Task<MyString> ReturnDataAsync([FromBody] MyString data)
    {
        OpenAIClient client = new OpenAIClient(
        new Uri("https://gpt-devlin.openai.azure.com/"),
        new AzureKeyCredential("f501038bd9334d15941f1c224c39514a"));

        chatComplete.Messages.Add(new ChatMessage(ChatRole.User, data.me));

        // ### If streaming is not selected
        Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(
            "devlinchat",
            chatComplete
            );

        var completions = responseWithoutStream.Value;

        chatComplete.Messages.Add(completions.Choices[0].Message);
        myMessages.Add(new MyString { me = data.me, assistant= completions.Choices[0].Message.Content });
        MyString questionOut = new MyString { me = data.me, assistant= completions.Choices[0].Message.Content };



        return questionOut;
    }

    [HttpGet]
    public List<MyString> ReturnChat()
    {
        foreach (var message in myMessages) {
            myMessages.Append(message);
            Console.WriteLine(message);
        } 

        return myMessages;
    }
}
