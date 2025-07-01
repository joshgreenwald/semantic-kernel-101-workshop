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

## Exercise 4: Run a Local Model and Create Another Kernel

Semantic Kernel also supports running local models. This is useful for scenarios where you want to run a model locally 
without relying on an external service. Any data sent to and from these models is not sent to the cloud, so you can use them for sensitive data.

We will use Azure Foundry Local to download and run modals locally. This is a free service that does not require an Azure subscription.

1. First, install the Azure Foundry Local CLI by following the instructions [here](https://learn.microsoft.com/en-us/azure/ai-foundry/foundry-local/get-started).

On Windows, you can install it via the command line:
```bash
winget install Microsoft.FoundryLocal
```

On macOS, you can install it via Homebrew:
```bash
brew tap microsoft/foundrylocal
brew install foundrylocal
```

Once installed, you can verify it by running:
```bash
foundry --help
```

2. Download phi-3.5-mini-1.5 model using the Azure Foundry Local CLI:
```bash
foundry model download phi-3.5-mini
```

3. Next, load the model:
```bash
foundry model load phi-3.5-mini
```

4. Verify the model is loaded by running:
```bash
foundry service list
```

5. Get the local URL for your models:
```bash
foundry service status
```

It should be http://localhost:5273 in most cases.

6. Add the following Nuget package to your project:
```bash
dotnet add package Microsoft.SemanticKernel.Connectors.AzureAIInference --prerelease
```

7. Now add a keyed singleton for our second Kernel:
```csharp
#pragma warning disable SKEXP0070
// Register a keyed singleton for a Semantic Kernel Kernel
services.AddKeyedSingleton("local", (_, _) =>
{
    var builder = Kernel.CreateBuilder();
    builder.AddAzureAIInferenceChatCompletion(
        modelId: "Phi-3.5-mini-instruct-generic-gpu",
        endpoint: new Uri("http://localhost:5273/v1") 
    );
    return builder.Build();
});
#pragma warning restore SKEXP0070
```

8. You can test your locally running model by changing the kernel that is injected into SimpleChat:

```csharp
public class SimpleChat([FromKeyedServices("local")] Kernel _kernel) { ... }
```

Notice how fast this model is compared to cloud-hosted models.

## Exercise 5: GitHub Copilot Instructions

Remember that, in general, models work better when they have more information. With GitHub Copilot Instructions files,
you can provide context to the the agent to help it understand your codebase and standards better. 
This will help it produce code for you that you will be more likely to accept.

Note that we will be using Visual Studio Code for this exercise.

1. Create a new directory in the root of your project called `.github`.
2. Add a new file called `copilot-instructions.md` in the `.github` directory.
3. Add the following content to the file:

```markdown
# Copilot Instructions for semantic-kernel-101-workshop

## Project Overview
- This is a C#/.NET 9 console application demonstrating Semantic Kernel usage, Azure OpenAI integration, and plugin/function calling.
- Main code is in the `SemanticKernel101` project. Key files: `Program.cs`, `Assistants/SimpleChat.cs`, `Plugins/TimePlugin.cs`.

## Architecture & Patterns
- **Dependency Injection**: Uses `Microsoft.Extensions.DependencyInjection` for all services, including the Semantic Kernel and assistants. Kernels can be registered as singletons or keyed singletons (for local/cloud models).
- **Assistants**: Place assistant logic in `Assistants/` (e.g., `SimpleChat`). Assistants are registered as transient services and injected with a `Kernel`.
- **Plugins**: Place Semantic Kernel plugins in `Plugins/`. Decorate plugin methods with `[KernelFunction]` and a `[Description]` for LLM function calling.
- **Kernel Registration**: Register the main kernel in DI, adding chat completion and plugins. Example:
  ```csharp
  services.AddSingleton<Kernel>(_ => {
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(model, endpoint, apiKey);
    builder.Plugins.AddFromType<TimePlugin>();
    return builder.Build();
  });
- **Local Model Support**: Register a local model kernel using `AddKeyedSingleton` (see `Program.cs`).
- **Function Calling**: Use `PromptExecutionSettings` with `FunctionChoiceBehavior.Auto()` to enable LLM function calling.

## Developer Workflows
- **Build**: Run `dotnet build` in the project root or `SemanticKernel101` folder.
- **Run**: Use `dotnet run --project SemanticKernel101`.
- **User Secrets**: Store API keys and endpoints using `dotnet user-secrets` (see README for details).
- **NuGet Packages**: Add dependencies with `dotnet add package <name>`.
- **Local Model**: Install and run Azure Foundry Local for local model support (see README Exercise 4).

## Conventions
- All assistant and plugin classes are in their respective folders and namespaces.
- Use dependency injection for all services/components.
- Register new plugins in the kernel builder in DI.
- Use `[KernelFunction]` and `[Description]` for all plugin methods intended for LLM use.
- Prefer streaming chat completions for interactive experiences.

## Examples
- See `Assistants/SimpleChat.cs` for assistant pattern and function calling.
- See `Plugins/TimePlugin.cs` for plugin structure.
- See `Program.cs` for DI setup, kernel registration, and local model support.

## External Integrations
- Azure OpenAI (cloud model)
- Azure Foundry Local (local model)
- Microsoft.SemanticKernel and related NuGet packages
```

Note how we're telling GitHub Copilot the purpose of the project, how it's structured, and how we like our code to 
be written. It will include these instructions with every request.

4. Let's test this out by creating a new plugin that does math (something else that LLMs are not great at).

`
Create a new Semantic Kernel plugin called MathPlugin. This will contain functions for adding, subtracting, multiplying, and dividing numbers.
`

5. The result should be a new file called `MathPlugin.cs` in the `Plugins` folder with the following content:

```csharp
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernel101.Plugins;

public class MathPlugin
{
    [KernelFunction]
    [Description("Add two numbers and return the result.")]
    public double Add(double a, double b) => a + b;

    [KernelFunction]
    [Description("Subtract the second number from the first and return the result.")]
    public double Subtract(double a, double b) => a - b;

    [KernelFunction]
    [Description("Multiply two numbers and return the result.")]
    public double Multiply(double a, double b) => a * b;

    [KernelFunction]
    [Description("Divide the first number by the second and return the result. Returns double.PositiveInfinity or double.NegativeInfinity if dividing by zero.")]
    public double Divide(double a, double b) => a / b;
}
```

Given the information it had in the instructions file, it knew exactly how to create the plugin, where to put it, and 
what methods to include.

It's outside the scope of this workshop, but you can add more instructions files that apply to specific directories 
or file types. It's very powerful!

## Exercise 6: GitHub Copilot Prompts

You might be familiar with slash commands in GitHub Copilot like /explain, /fix, and /refactor. These are great for quickly getting help with your code.

However, you can also create your own custom prompts that can be used across your codebase. This is done with the `.
copilot/prompts` directory. These prompts can use powerful toolsets that read your code. You can even use MCP 
toolsets (that's a whole other workshop).

1. Let's use the VS Code command palette to create a new prompt. Open the command palette (Ctrl+Shift+P) and type 
   "Chat: New Prompt File". Accept the default directory and name the prompt file `createdoc`. This will create a 
   new file in the `.copilot/prompts` directory called `createdoc.prompt.md`. This naming convention is very important.
2. Add the following content to the `createdoc.prompt.md` file:

```markdown
---
mode: agent
tools: ['codebase', 'editFiles', 'problems', 'runCommands', 'search', 'usages']
description: Create markdown documentation for the codebase
---
Create a well-structured markdown documentation for the codebase. The documentation should include:
- Overview of the project
- Key components and their purposes
- Architecture and design patterns used
- Developer workflows and conventions
- Examples of usage
- External integrations and dependencies

Make sure to use proper headings, bullet points, and code snippets where appropriate. The documentation should be clear and concise, suitable for developers who are new to the project.

The markdown file should be named `documentation.md` and placed in the root of the codebase. If a `documentation.md` file already exists, update it with the new documentation content while preserving any existing information that is still relevant.
```

3. To run it, simply type `/createdoc` in the chat window in **Agent** mode. It will read your codebase and create a new `documentation.md` file in the root of your project with the documentation.

Has creating documentation ever been this easy?


