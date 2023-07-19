namespace Money;
using Results;

public interface IWalletHolder
{
    IWallet Wallet { get; }
    Result Pay(Money amount, IWalletHolder payedTo);
    Result GetPaid(Money amount, IWalletHolder paidBy);
}

public interface IWallet
{
    List<DebtRegister> Debts { get; }
    Funds TotalFunds { get; }
    Result GetPaid(Money money);
    Result Pay(Money money);
}

public class HardLimitWallet : IWallet
{
    public List<DebtRegister> Debts { get; private set; }
    public Funds TotalFunds { get; private set; }
    private CurrencyConverter _converter;
    private Currency _walletCurrency;

    public HardLimitWallet(CurrencyConverter converter, Currency walletCurrency)
    {
        _converter = converter;
        _walletCurrency = walletCurrency;
        TotalFunds = Funds.Create(0, _walletCurrency);
        Debts = new();
    }

    public Result GetPaid(Money amount) => 
        ConvertToWalletCurrency(amount)
            .Map(newAmount => Funds.Create(newAmount.Value + TotalFunds.Value, _walletCurrency))
                .IfSucceeded(totalResult => TotalFunds = totalResult.Data)
            .ToSimpleResult();


    public Result Pay(Money amount) =>
        ConvertToWalletCurrency(amount)
            .Map(newAmount => Funds.Create(TotalFunds.Value - newAmount.Value, _walletCurrency))
            .Map(newTotal => newTotal.Value < 0
                    ? Result<Funds>.Fail(new UnknownError())
                    : Result<Funds>.Ok(newTotal))
                .IfSucceeded(newTotal => TotalFunds = newTotal.Data)
            .ToSimpleResult();

    private Result<Funds> ConvertToWalletCurrency(Funds amount) =>
        amount.Currency != _walletCurrency
            ? _converter.Convert(amount, _walletCurrency)
            : Result<Funds>.Ok(amount);

    private bool HasEnoughFunds(Funds amount) =>
        TotalFunds.Value >= amount.Value;
}

public class DebtRegister
{
    public Debt Debt { get; }
    public IWalletHolder OwedTo { get; }

    public DebtRegister(Debt debt, IWalletHolder owedTo)
    {
        Debt = debt;
        OwedTo = owedTo;
    }
}
