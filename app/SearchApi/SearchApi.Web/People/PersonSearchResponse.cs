using System;
using System.ComponentModel;

namespace SearchApi.Web.Controllers
{
    /// <summary>
    /// The PersonSearchResponse represents the acknowledgement that the search will be conducted.
    /// </summary>
    [Description("Represents the acknowledgement that the search will be conducted")]
    public class PersonSearchResponse
    {

        public PersonSearchResponse(Guid id)
        {
            if(id == default(Guid)) throw new ArgumentNullException(nameof(id));
            this.id = id;
        }

        [Description("The unique identifier of the search request")]
        public Guid id { get; }
    }
}