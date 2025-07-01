# Semantic Kernel 101 Workshop Documentation

## Overview

This project is a C#/.NET 9 console application that demonstrates the use of [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel), Azure OpenAI integration, and plugin/function calling. It is designed as a hands-on workshop for developers to learn about building AI-powered assistants, integrating with Azure OpenAI, and extending functionality with custom plugins.

## Key Components

- **Program.cs**: Entry point of the application. Sets up dependency injection (DI), registers kernels, assistants, and plugins.
- **Assistants/**: Contains assistant logic. Example: `SimpleChat.cs` demonstrates chat and function calling patterns.
- **Plugins/**: Contains Semantic Kernel plugins. Example: `TimePlugin.cs` and `MathPlugin.cs` provide functions callable by the LLM.
- **DI Setup**: Uses `Microsoft.Extensions.DependencyInjection` for all services, including kernels and assistants.

## Architecture & Design Patterns

- **Dependency Injection (DI)**: All services (kernels, assistants, plugins) are registered and resolved via DI for modularity and testability.
- **Kernel Registration**:
  - Main kernel is registered as a singleton, configured with Azure OpenAI chat completion and plugins.
  - Local model support is provided via `AddKeyedSingleton` for scenarios using Azure Foundry Local.
- **Assistants**: Placed in `Assistants/`, registered as transient services, and injected with a `Kernel`.
- **Plugins**: Placed in `Plugins/`, methods decorated with `[KernelFunction]` and `[Description]` for LLM function calling.
- **Function Calling**: Uses `PromptExecutionSettings` with `FunctionChoiceBehavior.Auto()` to enable LLM function calling.
- **Streaming Chat**: Prefers streaming chat completions for interactive user experiences.

## Developer Workflows

- **Build**: Run `dotnet build` in the project root or `SemanticKernel101` folder.
- **Run**: Use `dotnet run --project SemanticKernel101` to start the console app.
- **User Secrets**: Store API keys and endpoints securely using `dotnet user-secrets` (see README for setup).
- **NuGet Packages**: Add dependencies with `dotnet add package <name>`.
- **Local Model**: For local model support, install and run Azure Foundry Local (see README Exercise 4).

## Conventions

- All assistant and plugin classes are placed in their respective folders and namespaces.
- Use dependency injection for all services/components.
- Register new plugins in the kernel builder in DI.
- Use `[KernelFunction]` and `[Description]` for all plugin methods intended for LLM use.
- Prefer streaming chat completions for interactive experiences.

## Examples

### Assistant Pattern & Function Calling

See `Assistants/SimpleChat.cs`:
```csharp
public class SimpleChat
{
    private readonly Kernel _kernel;
    public SimpleChat(Kernel kernel) => _kernel = kernel;
    // ... chat logic and function calling ...
}
```

### Plugin Structure

See `Plugins/TimePlugin.cs`:
```csharp
public class TimePlugin
{
    [KernelFunction, Description("Get the current time")]
    public string GetCurrentTime() => DateTime.Now.ToString("T");
}
```

### Kernel Registration in DI

See `Program.cs`:
```csharp
services.AddSingleton<Kernel>(_ => {
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(model, endpoint, apiKey);
    builder.Plugins.AddFromType<TimePlugin>();
    return builder.Build();
});
```

## External Integrations & Dependencies

- **Azure OpenAI**: For cloud-based LLM chat completions.
- **Azure Foundry Local**: For local model support (see README for setup).
- **Microsoft.SemanticKernel**: Core SDK for building AI assistants and plugins.
- **Microsoft.Extensions.DependencyInjection**: For DI and service management.
- **Other NuGet Packages**: See `.csproj` for full dependency list.

---

For more details, see the README and explore the codebase. Contributions and feedback are welcome!
