using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SemanticKernel101.Assistants;

public class SimpleChat(Kernel _kernel)
{
    public async Task RunAssistant()
    {
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
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

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("Please enter a valid message.");
                continue;
            }

            history.AddUserMessage(userInput);
            
            var executionSettings = new PromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            var response = chatCompletionService.GetStreamingChatMessageContentsAsync(
                chatHistory: history,
                executionSettings: executionSettings,
                kernel: _kernel
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