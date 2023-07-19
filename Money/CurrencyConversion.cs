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

public class ConversionNotFound : IError
{
    private string _message;
    public string Message => _message;
    public ConversionNotFound(Currency from, Currency to) =>
        _message = String.Format(
            "Could not find conversion between {0} and {1}");

    public void Log() => throw new NotImplementedException();
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

public interface IConversionRateParser
{
    List<ConversionRate> Parse(object someThing);
}

public interface IConversionGetter
{
    List<ConversionRate> GetConversionRates();
}

public class ConversionRateApiHandler : IConversionGetter
{
    private HttpClient _client;
    private string _url;
    private IConversionRateParser _parser;

    public ConversionRateApiHandler(string url, IConversionRateParser parser)
    {
        _client = new();
        _url = url;
        _parser = parser;
    }

    public List<ConversionRate> GetConversionRates() =>
        GetData()
            .PipeNonNull(jsonData => _parser.Parse(jsonData));

    private string GetData()
    {
        var task = _client.GetAsync(_url);
        task.Wait();

        var reader = task
            .Result
            .Content
            .ReadAsStringAsync();
        reader.Wait();
        return reader.Result;
    }

}

public class CurrencyConverterFactory
{
    private CurrencyConverter _currentConverter;
    private DateTime _dateOfConverterCreation;
    private IConversionGetter _apiHandler;

    public CurrencyConverterFactory(ConversionRateApiHandler apiHandler)
    {
        _apiHandler = apiHandler;
        _currentConverter = Create();
    }

    public CurrencyConverter Get() =>
        _currentConverter is null
            ? Create()
            : _currentConverter;

    private CurrencyConverter Create() =>
        ConverterExpired()
            ? _apiHandler.GetConversionRates()
                .PipeNonNull(rates => new CurrencyConverter(rates))
            : _currentConverter;

    private bool ConverterExpired() =>
        _dateOfConverterCreation - DateTime.Now > TimeSpan.FromHours(6);

}

