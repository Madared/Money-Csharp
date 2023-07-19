namespace Money.Tests;
using Results;
using Money;

public class DecimalTests
{
    [Fact]
    public void PositiveDecimal_Creation_Fails_With_Negative_Value()
    {
        //Given
        Result<PositiveDecimal> positive = PositiveDecimal.Create(-10);
        //Then
        Assert.True(positive.Failed);
    }

    [Fact]
    public void Zero_Is_Not_PositiveDecimal()
    {
        //Given
        Result<PositiveDecimal> positive = PositiveDecimal.Create(0);
        //Then
        Assert.True(positive.Failed);
    }

    [Fact]
    public void NegativeDecimal_Creation_Fails_With_Positive_Value()
    {
        //Given
        Result<NegativeDecimal> negative = NegativeDecimal.Create(10);
        //Then
        Assert.True(negative.Failed);
    }

    [Fact]
    public void Zero_Is_Not_NegativeDecimal()
    {
        //Given
        Result<NegativeDecimal> negative = NegativeDecimal.Create(0);
        //Then
        Assert.True(negative.Failed);
    }

    [Fact]
    public void Zero_Is_NonNegative()
    {
        //Given
        Result<INonNegativeDecimal> zero = Decimal.CreateNonNegative(0);
        //Then
        Assert.True(zero.Succeeded);
    }

    [Fact]
    public void Zero_Is_NonPositive()
    {
        //Given
        Result<INonPositiveDecimal> zero = Decimal.CreateNonPositive(0);
        //Then
        Assert.True(zero.Succeeded);
    }
}
