using System.ComponentModel;

namespace HomePal.Infrastructure.AI.AgentChat.Tools;

public class CalculatorTools
{
    [Description("Executes a mathematical calculation on two numbers using specified operation (add, subtract, multiply, divide, power, modulo).")]
    public double Calculate(
        [Description("The first number in the operation")] double number1,
        [Description("The second number in the operation")] double number2,
        [Description("The operation to perform: 'add' (+), 'subtract' (-), 'multiply' (*), 'divide' (/), 'power' (^), 'modulo' (%)")] string operation)
    {
        var op = operation?.Trim().ToLowerInvariant();
        return op switch
        {
            "add" or "+" or "plus" => number1 + number2,
            "subtract" or "-" or "minus" => number1 - number2,
            "multiply" or "*" or "times" => number1 * number2,
            "divide" or "/" or "by" => number2 != 0 ? number1 / number2 : throw new DivideByZeroException("Cannot divide by zero."),
            "power" or "^" or "pow" => Math.Pow(number1, number2),
            "modulo" or "%" or "mod" => number1 % number2,
            _ => throw new ArgumentException($"Unsupported operation '{operation}'. Supported operations: add, subtract, multiply, divide, power, modulo.")
        };
    }
}
