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