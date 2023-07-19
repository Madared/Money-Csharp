namespace Money;
using Results;

public interface IMoney
{
    decimal Value { get; }
    Currency Currency { get; }
    string Representation();
    Result<Funds> Convert(ConversionRate rate);
}

public interface INonNegativeMoney : IMoney
{}

public interface IDebt : IMoney
{}

public abstract record Funds : IMoney
{
    public decimal Value { get; }
    public Currency Currency { get; }

    public Funds(decimal value, Currency currency)
    {
        Value = value;
        Currency = currency;
    }

    public string Representation() => Currency.Representation(Value);

    public static Funds Create(decimal value, Currency currency)
    {
        if (value == 0)
            return new NoMoney(currency);
        if (value < 0)
            return NegativeDecimal
                .Create(value)
                .Data
                .PipeNonNull(negative => new Debt(negative, currency));
        else
            return PositiveDecimal
                .Create(value)
                .Data
                .PipeNonNull(positive => new Money(positive, currency));
    }

    public abstract Result<Funds> Convert(ConversionRate rate); 
    public Result CheckCurrenciesMatch(Currency currency) =>
        Currency != currency
            ? Result.Fail(new MismatchCurrency(currency, Currency))
            : Result.Ok();
}

public record NoMoney : Funds, INonNegativeMoney
{
    public NoMoney(Currency currency) : base(0, currency) { }

    public override Result<Funds> Convert(ConversionRate rate) =>
        CheckCurrenciesMatch(rate.From)
            .Map(() => new NoMoney(rate.To) as Funds);
}

public record Debt : Funds
{
    public Debt(NegativeDecimal value, Currency currency) : base(value, currency) { }

    public override Result<Funds> Convert(ConversionRate rate) =>
            CheckCurrenciesMatch(rate.From)
                .Map(() => NegativeDecimal.Create(Value * rate.ConvertionRate))
                .Map(value => new Debt(value, rate.To) as Funds);
}

public record Money : Funds, INonNegativeMoney
{
    public Money(PositiveDecimal value, Currency currency) : base(value, currency) { }
    public override Result<Funds> Convert(ConversionRate rate) =>
            CheckCurrenciesMatch(rate.From)
                .Map(() => PositiveDecimal.Create(Value * rate.ConvertionRate))
                .Map(value => new Money(value, rate.To) as Funds);
}

public class MismatchCurrency : IError
{
    private string _message;
    public string Message => _message;

    public MismatchCurrency(Currency a, Currency b) =>
        _message = String.Format(
            "Currency {0} does not match currency {1}");

    public void Log() => Console.WriteLine(_message);
}
