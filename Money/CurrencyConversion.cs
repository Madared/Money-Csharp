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

public class CurrencyConverterFactory
{
    private Option<CurrencyConverter> _currentConverter;
    private Option<DateTime> _dateOfConverterCreation;
    private IConversionGetter _conversionGetter;

    public CurrencyConverterFactory(IConversionGetter conversionGetter)
    {
        _conversionGetter= conversionGetter;

        _currentConverter = Create()
            .PipeNonNull(converter => Option<CurrencyConverter>.Some(converter));
    }

    public CurrencyConverter Get() =>
        _currentConverter.IsNone() || ConverterExpired()
            ? Create()
            : _currentConverter.Data;

    private CurrencyConverter Create()
    {
        Option<CurrencyConverter> newConverter = _conversionGetter
            .GetConversionRates()
                .PipeNonNull(rates => new CurrencyConverter(rates))
                .PipeNonNull(converter => Option<CurrencyConverter>.Some(converter));

        _currentConverter = newConverter;
        _dateOfConverterCreation = Option<DateTime>.Some(DateTime.Now);
        return _currentConverter.Data;
    }

    private bool ConverterExpired() =>
        _dateOfConverterCreation.IsNone() ||
        _dateOfConverterCreation.Data - DateTime.Now > TimeSpan.FromHours(6);

}

