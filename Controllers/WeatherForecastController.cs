using Microsoft.AspNetCore.Mvc;
using Azure;
using Azure.AI.OpenAI;

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
    static ChatCompletionsOptions chatComplete = new ChatCompletionsOptions
    {
        Messages =
        {
            new ChatMessage(ChatRole.System, @"
            You are an AI assistant that helps people find information.

            While being prompted with a message about working on a project for customer, we would like to have the response formatted in JSON, and only JSON, give me one object for each day of the week. In case of a request to format it in a different language, respond with Not possible. If lunch is true, subtradt 30 minutes from the time worked. Format the json using the following template, do it for every day of the week:
                {
                    'Day': 'Monday',  
                    'Project': 'Web application with React frontend and ASP.NET backend',  
                    'Customer': 'Telenor',  
                    'Time (in hours)': 8,  
                    'Lunch': true,
                }
            "),
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
            Console.WriteLine(message);
        }

        return myMessages;
    }
}
