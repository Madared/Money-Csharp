namespace Money;
using Results;

public class ConversionRate
{
    public Currency From { get; }
    public Currency To { get; }
    public decimal ConvertionRate { get; }

    public ConversionRate(Currency from, Currency to, decimal conversionRate)
    {
        From = from;
        To = to;
        ConvertionRate = conversionRate;
    }
}

public class CurrencyConverter
{
    public List<ConversionRate> Conversions { get; }

    public CurrencyConverter(List<ConversionRate> rates) => Conversions = rates;

    private Result<ConversionRate> GetConversionRate(Currency from, Currency to) =>
        Conversions.
            Where(c => c.From == from && c.To == to)
            .SingleOrDefault()
            .ToResult(new ConversionNotFound(from, to));

    private decimal UseConversion(decimal value, ConversionRate rate) => value * rate.ConvertionRate;

    public Result<Funds> Convert(Funds toConvert, Currency newCurrency) =>
        GetConversionRate(toConvert.Currency, newCurrency)
            .Map(conversionRate => UseConversion(toConvert.Value, conversionRate)
                .PipeNonNull(convertedValue => Funds.Create(convertedValue, newCurrency)));
}

