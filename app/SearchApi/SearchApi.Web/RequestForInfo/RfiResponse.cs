using System;
using System.ComponentModel;

namespace SearchApi.Web.Controllers
{
    [Description("Represents the acknowledgement that the request for information will be conducted")]
    public class RfiResponse
    {

        public RfiResponse(Guid id)
        {
            if(id == default(Guid)) throw new ArgumentNullException(nameof(id));
            this.Id = id;
        }

        [Description("The unique identifier of the request for information")]
        public Guid Id { get; }
    }
}