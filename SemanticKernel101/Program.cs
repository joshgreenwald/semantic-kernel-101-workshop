using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var endpoint = config["AzureOpenAI:Endpoint"] ?? "No secret found";
var model = config["AzureOpenAI:Model"] ?? "gpt-4.1";
var apiKey = config["AzureOpenAI:Key"] ?? "No key found";

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(model, endpoint, apiKey);
var kernel = builder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
history.AddSystemMessage("You are a helpful assistant.");

Console.WriteLine("Welcome to the Semantic Kernel Chatbot!");
Console.WriteLine("To exit the chat, type 'exit'.");
Console.WriteLine();

var continueChat = true;

while (continueChat)
{
    Console.Write("You: ");
    var userInput = Console.ReadLine();
    if (userInput == "exit")
    {
        continueChat = false;
        break;
    }

    history.AddUserMessage(userInput);
    
    var response = chatCompletionService.GetStreamingChatMessageContentsAsync(
        chatHistory: history,
        kernel: kernel
    );
    
    Console.Write("Assistant: ");

    await foreach (var chunk in response)
    {
        Console.Write(chunk);
    }
    
    Console.WriteLine();
    Console.WriteLine();
}
