# Semantic Kernel 101 and GitHub Copilot Workshop

## Pre-requisites

* .NET 9 SDK
* Visual Studio Code, Visual Studio, or Rider (VS Code preferred)
* Access to an Azure OpenAI Service with a deployed model

## Overview

This workshop will introduce you to creating applications with Semantic Kernel and implementing updates to it with GitHub Copilot.

We will start with an empty C# console application.

## Exercise 1: Create your first Kernel

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