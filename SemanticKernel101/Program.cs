using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SemanticKernel101.Assistants;
using SemanticKernel101.Plugins;

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
    builder.Plugins.AddFromType<TimePlugin>();
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