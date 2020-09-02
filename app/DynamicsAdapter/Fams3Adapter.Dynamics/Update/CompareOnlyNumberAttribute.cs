namespace Fams3Adapter.Dynamics.Update
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class CompareOnlyNumberAttribute : System.Attribute
    {
        public double version;

        public CompareOnlyNumberAttribute()
        {
            version = 1.0;
        }
    }
}