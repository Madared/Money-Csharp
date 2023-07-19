namespace Money;
using Results;

///<summary>
///Base abstraction and wrapper around decimal values,
///acts as a store for the Value itself and provides factory methods to create non-negative and non-positive decimals
///</summary>
public abstract record Decimal
{
    public abstract decimal Value { get; }
    /// <summary>
    /// Creates a non-negative decimal value.
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <returns>A Result containing the created non-negative decimal or an error.</returns>
    public static Result<INonNegativeDecimal> CreateNonNegative(decimal value)
    {
        if (value < 0)
            return Result<INonNegativeDecimal>.Fail(new UnknownError());
        else if (value == 0)
            return Result<INonNegativeDecimal>.Ok(new Zero());
        else
            return PositiveDecimal.Create(value)
                .Map(positive => Result<INonNegativeDecimal>.Ok(positive));
    }

    /// <summary>
    /// Creates a non-positive decimal value.
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <returns>A Result containing the created non-positive decimal or an error.</returns>
    public static Result<INonPositiveDecimal> CreateNonPositive(decimal value)
    {
        if (value > 0)
            return Result<INonPositiveDecimal>.Fail(new UnknownError());
        else if (value == 0)
            return Result<INonPositiveDecimal>.Ok(new Zero());
        else
            return NegativeDecimal.Create(value)
                .Map(negative => Result<INonPositiveDecimal>.Ok(negative));
    }

    public static implicit operator decimal(Decimal value) => value.Value;
}

///<summary>
///An interface to work with PositiveDecimal and Zero
///</summary>
public interface INonNegativeDecimal
{
    decimal Value { get; }
}

///<summary>
///An interface to work with NegativeDecimal and Zero
///</summary>
public interface INonPositiveDecimal
{
    decimal Value { get; }
}

///<summary>
///The decimal representation of zero
///</summary>
public record Zero : Decimal, INonNegativeDecimal, INonPositiveDecimal
{
    public override decimal Value => 0;
}

///<summary>
/// Represents a positive decimal value (more than zero)
///</summary>
public record PositiveDecimal : Decimal, INonNegativeDecimal
{
    public override decimal Value { get; }

    protected PositiveDecimal(decimal value) => Value = 
        value <= 0
            ? throw new ArgumentOutOfRangeException()
            : value;

    /// <summary>
    /// Creates a positive decimal value.
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <returns>A Result containing the created positive decimal or an error.</returns>
    public static Result<PositiveDecimal> Create(decimal value) =>
        value <= 0
            ? Result<PositiveDecimal>.Fail(new UnknownError())
            : Result<PositiveDecimal>.Ok(new PositiveDecimal(value));

    //public static implicit operator decimal(PositiveDecimal value) => value.Value;
}

///<summary>
/// Represents a negative decimal value (less than zero)
///</summary>
public record NegativeDecimal : Decimal, INonPositiveDecimal
{
    public override decimal Value { get; }
    protected NegativeDecimal(decimal value) => Value =
        value >= 0
            ? throw new ArgumentOutOfRangeException()
            : value;

    /// <summary>
    /// Creates a negative decimal value.
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <returns>A Result containing the created negative decimal or an error.</returns>
    public static Result<NegativeDecimal> Create(decimal value) =>
        value >= 0
            ? Result<NegativeDecimal>.Fail(new UnknownError())
            : Result<NegativeDecimal>.Ok(new NegativeDecimal(value));

    //public static implicit operator decimal(NegativeDecimal value) => value.Value;
}
