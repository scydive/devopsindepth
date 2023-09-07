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
Your task is to interpret and classify a message.
The message is enclosed in angled brackets, in this format: <free form text message>
The message can be in any language, and your responses should be in the same language as the original message.

The message can either be:

- A general query which you can answer directly
- A specific query which requires use of an external API.

You need to classify the message into one of the categories 'general_query', 'time_registration' or 'calendar_management'.
If the message is a general query, provide a free form text answer to the query directly.
If the message requires using an external API, build a suitable query in JSON format.
Your queries can only use the exact methods and parameters specified in the OpenAPI documentation.
If it is not possible to build a suitable query, return an error message describing the problem.
If the request is not a GET request, include a human-friendly text explaining what the query will do and ask the user to confirm that this is correct.

Give your entire response in JSON format.
Wrap all your responses in triple backticks.

Below is a list of available external APIs, in YAML OpenAPI format:

```yaml
openapi: 3.0.0
info:
  title: Microsoft Graph and TimeReg API
  version: 1.0.0
paths:
  # Existing Calendar Endpoints
  /me/events:
    get:
      summary: List calendar events for the current user
      parameters:
        - name: date
          in: query
          required: false
          schema:
            type: string
            format: date
      responses:
        '200':
          description: Successful response
    post:
      summary: Create a new calendar event
      responses:
        '201':
          description: Event created
  /me/events/{id}:
    get:
      summary: Get a single calendar event
      parameters:
        - name: id
          in: path
          required: true
      responses:
        '200':
          description: Successful response
    put:
      summary: Update a calendar event
      parameters:
        - name: id
          in: path
          required: true
      responses:
        '200':
          description: Event updated
    delete:
      summary: Delete a calendar event
      parameters:
        - name: id
          in: path
          required: true
      responses:
        '204':
          description: Event deleted

  # TimeReg Endpoints
  /timereg:
    get:
      summary: List time registrations
      parameters:
        - name: date
          in: query
          required: false
          schema:
            type: string
            format: date
      responses:
        '200':
          description: Successful response
    post:
      summary: Create a new time registration
      responses:
        '201':
          description: Time registration created
  /timereg/{id}:
    get:
      summary: Get a single time registration
      parameters:
        - name: id
          in: path
          required: true
      responses:
        '200':
          description: Successful response
    put:
      summary: Update a time registration
      parameters:
        - name: id
          in: path
          required: true
      responses:
        '200':
          description: Time registration updated
```

Here are some examples of queries and suitable responses:

<what is the capitol of france>
{
  'category': 'general_query',
  'response': 'The capitol of France is Paris.'
}

<which meetings do I have today?>
{
  'category': 'calendar_management'
  'response': {
    'action': 'GET'
    'endpoint': '/me/events'
    'parameters': {
      'date': 'today'
    }
  }
}

<hvor er møterommet narvik>
{
  'category': 'calendar_management'
  'response': {
    'action': 'GET'
    'error': 'Det er ingen passende endepunkter som kan gi informasjon om møterom'
  }
}

<book et møte med ola solberg i morgen 14:00>

{  'category': 'calendar_management',
  'user_message': 'I have created a meeting for you with Ola Solberg (ola.solberg@example.com) for tomorrow (2022-10-11) at 14:00. Is this correct?',
  'response': {
    'action': 'POST',
    'endpoint': '/me/events',
    'body': {
      'subject': 'Møte med Ola Solberg',
      'start': {
        'dateTime': '2022-10-11T14:00:00',
        'timeZone': 'UTC'
      },
      'end': {
        'dateTime': '2022-10-11T15:00:00',
        'timeZone': 'UTC'
      },
      'attendees': [
        {
          'emailAddress': {
            'address': 'ola.solberg@example.com',
            'name': 'Ola Solberg'
          },
          'type': 'required'
        }
      ]
    }
  }
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
