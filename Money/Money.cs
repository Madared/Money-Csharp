namespace Money;
using Results;
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
