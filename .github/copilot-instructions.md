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