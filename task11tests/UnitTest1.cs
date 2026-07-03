using task11;

public class CalculatorTests
{
    private readonly ICalculator _calculator;

    public CalculatorTests()
    {
        var generator = new CalculatorGenerator();
        var assembly = generator.GenerateAndCompile();
        var calculatorType = assembly.GetType("GeneratedNamespace.Calculator")!;
        _calculator = (ICalculator)Activator.CreateInstance(calculatorType)!;
    }

    [Fact]
    public void Calculator_Add_ReturnsCorrectResult()
    {
        var result = _calculator.Add(5, 3);
        Assert.Equal(8, result);
    }

    [Fact]
    public void Calculator_Minus_ReturnsCorrectResult()
    {
        var result = _calculator.Minus(10, 4);
        Assert.Equal(6, result);
    }

    [Fact]
    public void Calculator_Mul_ReturnsCorrectResult()
    {
        var result = _calculator.Mul(6, 7);
        Assert.Equal(42, result);
    }

    [Fact]
    public void Calculator_Div_ReturnsCorrectResult()
    {
        var result = _calculator.Div(20, 4);
        Assert.Equal(5, result);
    }

    [Fact]
    public void Calculator_IsNotNull()
    {
        Assert.NotNull(_calculator);
    }
}