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
    private CurrencyConverter _currentConverter;
    private DateTime _dateOfConverterCreation;
    private IConversionGetter _conversionGetter;

    public CurrencyConverterFactory(IConversionGetter conversionGetter)
    {
        _conversionGetter= conversionGetter;
        _currentConverter = Create();
    }

    public CurrencyConverter Get() =>
        _currentConverter is null
            ? Create()
            : _currentConverter;

    private CurrencyConverter Create() =>
        ConverterExpired()
            ? _conversionGetter.GetConversionRates()
                .PipeNonNull(rates => new CurrencyConverter(rates))
            : _currentConverter;

    private bool ConverterExpired() =>
        _dateOfConverterCreation - DateTime.Now > TimeSpan.FromHours(6);

}

