namespace Money;
using Results;


public class ConversionNotFound : IError
{
    private string _message;
    public string Message => _message;
    public ConversionNotFound(Currency from, Currency to) =>
        _message = String.Format(
            "Could not find conversion between {0} and {1}");

    public void Log() => throw new NotImplementedException();
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
