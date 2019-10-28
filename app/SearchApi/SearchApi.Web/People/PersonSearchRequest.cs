using System;
using System.ComponentModel;
using Newtonsoft.Json;
using NSwag;

namespace SearchApi.Web.Controllers
{
    /// <summary>
    /// The PersonSearchRequest represents the information known about a subject before executing a search.
    /// </summary>
    [Description("Represents a set of information to execute a search on a person")]
    public class PersonSearchRequest
    {
        [JsonConstructor]
        public PersonSearchRequest(string firstName, string lastName, DateTime? dateOfBirth)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
        }

        [Description("The first name of the subject.")]
        public string FirstName { get; }
        [Description("The last name of the subject.")]
        public string LastName { get; }
        [Description("The date of birth of the subject.")]
        public DateTime? DateOfBirth { get; }
    }
}