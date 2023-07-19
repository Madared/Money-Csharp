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
{ }

public interface IDebt : IMoney
{ }

