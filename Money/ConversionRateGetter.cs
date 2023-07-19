namespace Money;
using Results;

public interface IConversionGetter
{
    List<ConversionRate> GetConversionRates();
}

public class JsonToConversionRateParser
{
    public List<ConversionRate> Parse(string jsonData) => throw new NotImplementedException();
}

public class ConversionRateApiHandler : IConversionGetter
{
    private HttpClient _client;
    private string _url;
    private JsonToConversionRateParser _parser;

    public ConversionRateApiHandler(string url)
    {
        _client = new();
        _parser = new();
        _url = url;
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

