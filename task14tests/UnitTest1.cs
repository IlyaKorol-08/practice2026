using Xunit;
using task14;
using System;

public class DefiniteIntegralTests
{
    Func<double, double> X = (double x) => x;
    Func<double, double> SIN = (double x) => Math.Sin(x);

    [Fact]
    public void Integral_X_FromMinus1To1_ReturnsZero()
    {
        double result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void Integral_Sin_FromMinus1To1_ReturnsZero()
    {
        double result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void Integral_X_From0To5_Returns12Point5()
    {
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        Assert.Equal(12.5, result, 5);
    }

    [Fact]
    public void Integral_X_From0To5_With1Thread_Returns12Point5()
    {
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-4, 1);
        Assert.Equal(12.5, result, 3);
    }

    [Fact]
    public void Integral_X_From0To5_With4Threads_Returns12Point5()
    {
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-4, 4);
        Assert.Equal(12.5, result, 3);
    }

    [Fact]
    public void Integral_Sin_From0ToPi_Returns2()
    {
        double result = DefiniteIntegral.Solve(0, Math.PI, SIN, 1e-5, 4);
        Assert.Equal(2.0, result, 3);
    }
}