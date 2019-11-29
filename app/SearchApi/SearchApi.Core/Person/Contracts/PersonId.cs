namespace SearchApi.Core.Person.Contracts
{

    public enum PersonIDKind
    {
        DriverLicense
    }

    public interface PersonId
    {
        PersonIDKind Kind { get; }
        string Issuer { get; }
        string Number { get; }
    }
}