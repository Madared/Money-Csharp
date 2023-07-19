namespace Money;

public interface IMoney
{
    decimal Value { get; }
    Currency Currency { get; }
    string Representation();
}

public interface INonNegativeMoney : IMoney
{ }

public interface IDebt : IMoney
{ }

