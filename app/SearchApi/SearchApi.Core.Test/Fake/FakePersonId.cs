using SearchApi.Core.Contracts;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonId : PersonId
    {
        public PersonIDKind Kind { get; } = PersonIDKind.DriverLicense;

        public string Issuer { get; } = nameof(Issuer);

        public string Number { get; } = nameof(Number);
    }
}