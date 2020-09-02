namespace Fams3Adapter.Dynamics.Update
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class UpdateIgnoreAttribute : System.Attribute
    {
        public double version;

        public UpdateIgnoreAttribute()
        {
            version = 1.0;
        }
    }
}