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
