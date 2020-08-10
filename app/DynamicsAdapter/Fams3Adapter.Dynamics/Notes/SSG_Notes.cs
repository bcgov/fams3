using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.Notes
{
    //We have to use SSG_Notese 

    public class NotesEntity : DynamicsEntity
    {
        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }


        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }


        [JsonProperty("ssg_description")]
        public string Description { get; set; }
    }

    public class SSG_Notese : NotesEntity
    {
        [JsonProperty("ssg_notesid")]
        public Guid NotesId { get; set; }
    }
}
