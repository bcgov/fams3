namespace BcGov.Fams3.SearchApi.Contracts.IA
{
    public   interface IASearchEvent
    {
        Person.Person Person { get; }
        string BatchNo { get; }

        string RequestorTimeStamp { get; }
    }
}
