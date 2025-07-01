# Semantic Kernel 101 and GitHub Copilot Workshop

## Pre-requisites

* .NET 9 SDK
* Visual Studio Code, Visual Studio, or Rider (VS Code preferred)
* Access to an Azure OpenAI Service with a deployed model
* GitHub Copilot enabled in your IDE

Before starting, make sure you have the latest versions of your IDE and extensions installed. The GitHub Copilot extension in VS Code updates quite often.

## Overview

This workshop will introduce you to creating applications with Semantic Kernel and implementing updates to it with GitHub Copilot.

We will start with an empty C# console application. Open the **SemanticKernel101** C# project in your IDE of choice. If you are using Visual Studio Code, open the folder containing the project.

## Exercise 1: Create your first Kernel

First, we're going to create the most simple version of kernel.

1. Create a User Secrets file to store the Azure OpenAI key and endpoint.
   - Run the command `dotnet user-secrets init` in the terminal in the project folder.
   - Add your Azure OpenAI key and endpoint to the secrets file, providing the endpoint and key to your Azure OpenAI instance:
     ```json
     {
       "AzureOpenAI:Model": "gpt-4.1",
       "AzureOpenAI:Key": "78b2dd0b5a9b4526ae83be81601422ab",
       "AzureOpenAI:Endpoint": "https://openai-copilotbuilder.openai.azure.com/"
     }
     ```
     Alternatively, you can use the dotnet CLI to add these secrets directly:
     ```bash
     dotnet user-secrets set "AzureOpenAI:Endpoint" "<your-endpoint>"
     dotnet user-secrets set "AzureOpenAI:Key" "<your-key>"
     dotnet user-secrets set "AzureOpenAI:Model" "gpt-4.1"
        ```


2. Install the following NuGet packages:
   - `Microsoft.SemanticKernel`
   - `Microsoft.SemanticKernel.Connectors.AzureOpenAI`
   - `Microsoft.Extensions.Configuration.UserSecrets`

   You can do this in a package manager console or by running the following command in the terminal:
   ```bash
   dotnet add package Microsoft.SemanticKernel
   dotnet add package Microsoft.SemanticKernel.Connectors.AzureOpenAI
   dotnet add package Microsoft.Extensions.Configuration.UserSecrets
   ```
   
3. In Program.cs, add the following code to create a simple kernel that uses your Azure OpenAI instance:

```csharp
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
```

4. Now, let's create a simple chat completion agent to test our kernel. Add the following code after the kernel creation:

```csharp
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
```

5. Run the application. You should be able to chat with the assistant using your Azure OpenAI model.

## Exercise 2: Using GitHub Copilot to Implement Dependency Injection

So far, so good. But, what if we want to reuse our kernel in throughout the application? 

1. First, let's separate our existing assistant into a separate class. Create a new directory called `Assistants` and then an empty `SimpleChat.cs` file inside it.
2. The code of this class should look like this:

```csharp
    public SimpleChat(Kernel kernel)
    {
        this.kernel = kernel;
    }

    public async Task RunAssistant()
    {
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
    }
}
```

3. Refactor Program.cs to use this class:

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using SemanticKernel101.Assistants;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var endpoint = config["AzureOpenAI:Endpoint"] ?? "No secret found";
var model = config["AzureOpenAI:Model"] ?? "gpt-4.1";
var apiKey = config["AzureOpenAI:Key"] ?? "No key found";

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(model, endpoint, apiKey);
var kernel = builder.Build();

var chat = new SimpleChat(kernel);
await chat.RunAssistant();
```

That makes things more concise, but as our application grows, we'll want to use dependency injection to manage our kernel and other services.
We could write this code by ourselves, but let's use GitHub Copilot to help us implement dependency injection.

4. While Program.cs is open, let's use **Ask** mode with GPT 4.1 to see how it would approach this. Ask it "How do I implement dependency injection for my Kernel?". 
What does this look like? Was it a good answer? Now ask it with Claude. How does it compare? Was one model faster over the other?
5. **Ask** mode is great, but wouldn't it be nice if we could just have GitHub Copilot make changes for us across the entire codebase? Let's switch to **Agent** mode and try this prompt:

`I would like to have dependency injection support. Implement this for my Kernel. Also make sure that the SimpleChat utilizes dependency injection and can be injected itself.`

6. Notice how **Agent** mode works. You can see what it is thinking and what changes it's making. When complete, it will tell you what it did and have you accept or deny its changes.
7. Your Program.cs should now look like this:

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SemanticKernel101.Assistants;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var endpoint = config["AzureOpenAI:Endpoint"] ?? "No secret found";
var model = config["AzureOpenAI:Model"] ?? "gpt-4.1";
var apiKey = config["AzureOpenAI:Key"] ?? "No key found";

// Set up dependency injection container
var services = new ServiceCollection();

// Register configuration
services.AddSingleton<IConfiguration>(config);

// Register Kernel as a singleton
services.AddSingleton<Kernel>(serviceProvider =>
{
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(model, endpoint, apiKey);
    return builder.Build();
});

// Register your assistant classes
services.AddTransient<SimpleChat>();

// Build the service provider
var serviceProvider = services.BuildServiceProvider();

try
{
    // Resolve and run your application
    var chat = serviceProvider.GetRequiredService<SimpleChat>();
    await chat.RunAssistant();
}
finally
{
    // Dispose of the service provider to clean up resources
    await serviceProvider.DisposeAsync();
}
```
8. Run the application to see if things are still working as before.

## Exercise 3: Using Semantic Kernel Plugins and Functions

One of the most powerful features of Semantic Kernel is its ability to run C# code from the LLM. This allows you to 
implement custom functions that can be called from the LLM, enabling you to create more complex applications. For example,
you could create a function that retrieves data from a database or calls an external API. This is achieved via a mechanism
called **function calling** in which the LLM is able to determine when to utilize a function and what parameters to pass to it.

It may shock you to learn that LLMs do not know the current date or time. Why? Their knowledge source is static and does not change over time.
You can try this out by running the chat we've already built and asking it "What is the current date and time?". Let's fix that.

1. Create a new directory called `Plugins` and an empty `TimePlugin.cs` file inside it.

```csharp
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernel101.Plugins;

public class TimePlugin
{
    [KernelFunction]
    [Description("Get the current date and time")]
    public DateTime GetCurrentDateTime()
    {
        return DateTime.Now;
    }
}
```

Notice how we decorated the method with `[KernelFunction]` and provided a description. This is how Semantic Kernel knows that this is a function that can be called from the LLM.

2. Now we need to register this plugin with our kernel. Open `Program.cs` and change the kernel registration code to include the plugin:

```csharp
services.AddSingleton<Kernel>(serviceProvider =>
{
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(model, endpoint, apiKey);
    builder.Plugins.AddFromType<TimePlugin>();
    return builder.Build();
});
```

3. Finally, we need to expose the plugin to the LLM. This is done with automatic function calling. 

In the `SimpleChat` class pass a `PromptExecutionSettings` object to the call to the chat completion service:

```csharp
var executionSettings = new PromptExecutionSettings()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var response = chatCompletionService.GetStreamingChatMessageContentsAsync(
    chatHistory: history,
    executionSettings: executionSettings,
    kernel: _kernel
);
```

4. Run the app and ask it "What is the current date and time?". It should now be able to call the `GetCurrentDateTime` function 
and return the current date and time. You can also debug and set breakpoints to prove it's doing so.

While this is a simple example, you can create more complex plugins that can interact with external systems, databases, or APIs.

