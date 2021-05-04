using BcGov.Fams3.Redis.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using SearchApi.Web.Search;
using System.Collections.Generic;
using System.Linq;

namespace SearchApi.Web.Test.Search
{
    public class DataPartnerStoreExtensionTest
    {
        string jsonData = "{\"SearchRequestId\":\"4d2ece27-9570-407c-acd9-a1d14ed72dbf\",\"Person\":{\"dataProviders\":[{\"Name\":\"ICBC\"},{\"Name\":\"BCHydro\"}],\"FirstName\":\"string\",\"LastName\":\"string\",\"MiddleName\":\"string\",\"OtherName\":\"string\",\"DateOfBirth\":\"2020-04-07T15:09:38.768Z\",\"DateOfDeath\":\"2020-04-07T15:09:38.768Z\",\"Gender\":\"string\",\"DateDeathConfirmed\":true,\"Incacerated\":\"string\",\"Height\":\"string\",\"Weight\":\"string\",\"HairColour\":\"string\",\"EyeColour\":\"string\",\"Complexion\":\"string\",\"DistinguishingFeatures\":\"string\",\"WearGlasses\":\"string\",\"Identifiers\":[{\"Value\":\"string\",\"Type\":0,\"TypeCode\":\"string\",\"IssuedBy\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.768Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Addresses\":[{\"Type\":\"string\",\"AddressLine1\":\"string\",\"AddressLine2\":\"string\",\"AddressLine3\":\"string\",\"StateProvince\":\"string\",\"City\":\"string\",\"CountryRegion\":\"string\",\"ZipPostalCode\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.768Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Phones\":[{\"PhoneNumber\":\"string\",\"Extension\":\"string\",\"Type\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.769Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Names\":[{\"FirstName\":\"string\",\"LastName\":\"string\",\"MiddleName\":\"string\",\"OtherName\":\"string\",\"Type\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.769Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"RelatedPersons\":[{\"FirstName\":\"string\",\"LastName\":\"string\",\"MiddleName\":\"string\",\"OtherName\":\"string\",\"Gender\":\"string\",\"DateOfBirth\":\"2020-04-07T15:09:38.769\",\"Type\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.769Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Employments\":[{\"EmploymentConfirmed\":true,\"IncomeAssistance\":true,\"IncomeAssistanceStatus\":\"string\",\"Employer\":{\"Name\":\"string\",\"OwnerName\":\"string\",\"Phones\":[{\"PhoneNumber\":\"string\",\"Extension\":\"string\",\"Type\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.769Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Address\":{\"Type\":\"string\",\"AddressLine1\":\"string\",\"AddressLine2\":\"string\",\"AddressLine3\":\"string\",\"StateProvince\":\"string\",\"City\":\"string\",\"CountryRegion\":\"string\",\"ZipPostalCode\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.77Z\"}],\"Description\":\"string\",\"Notes\":\"string\"},\"ContactPerson\":\"string\"},\"Occupation\":\"string\",\"Website\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.769Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Notes\":\"string\"},\"DataPartners\":[{\"Name\":\"ICBC\",\"Completed\":false},{\"Name\":\"BCHydro\",\"Completed\":false,\"SearchSpeed\":1}]}";
        string jsonDataJCA = "{\"SearchRequestId\":\"4d2ece27-9570-407c-acd9-a1d14ed72dbf\",\"Person\":{\"dataProviders\":[{\"Name\":\"ICBC\"},{\"Name\":\"BCHydro\"}],\"FirstName\":\"string\",\"LastName\":\"string\",\"MiddleName\":\"string\",\"OtherName\":\"string\",\"DateOfBirth\":\"2020-04-07T15:09:38.768Z\",\"DateOfDeath\":\"2020-04-07T15:09:38.768Z\",\"Gender\":\"string\",\"DateDeathConfirmed\":true,\"Incacerated\":\"string\",\"Height\":\"string\",\"Weight\":\"string\",\"HairColour\":\"string\",\"EyeColour\":\"string\",\"Complexion\":\"string\",\"DistinguishingFeatures\":\"string\",\"WearGlasses\":\"string\",\"Identifiers\":[{\"Value\":\"string\",\"Type\":0,\"TypeCode\":\"string\",\"IssuedBy\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.768Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Addresses\":[{\"Type\":\"string\",\"AddressLine1\":\"string\",\"AddressLine2\":\"string\",\"AddressLine3\":\"string\",\"StateProvince\":\"string\",\"City\":\"string\",\"CountryRegion\":\"string\",\"ZipPostalCode\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.768Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Phones\":[{\"PhoneNumber\":\"string\",\"Extension\":\"string\",\"Type\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.769Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Names\":[{\"FirstName\":\"string\",\"LastName\":\"string\",\"MiddleName\":\"string\",\"OtherName\":\"string\",\"Type\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.769Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"RelatedPersons\":[{\"FirstName\":\"string\",\"LastName\":\"string\",\"MiddleName\":\"string\",\"OtherName\":\"string\",\"Gender\":\"string\",\"DateOfBirth\":\"2020-04-07T15:09:38.769\",\"Type\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.769Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Employments\":[{\"EmploymentConfirmed\":true,\"IncomeAssistance\":true,\"IncomeAssistanceStatus\":\"string\",\"Employer\":{\"Name\":\"string\",\"OwnerName\":\"string\",\"Phones\":[{\"PhoneNumber\":\"string\",\"Extension\":\"string\",\"Type\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.769Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Address\":{\"Type\":\"string\",\"AddressLine1\":\"string\",\"AddressLine2\":\"string\",\"AddressLine3\":\"string\",\"StateProvince\":\"string\",\"City\":\"string\",\"CountryRegion\":\"string\",\"ZipPostalCode\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.77Z\"}],\"Description\":\"string\",\"Notes\":\"string\"},\"ContactPerson\":\"string\"},\"Occupation\":\"string\",\"Website\":\"string\",\"ReferenceDates\":[{\"Index\":0,\"Key\":\"string\",\"Value\":\"2020-04-07T15:09:38.769Z\"}],\"Description\":\"string\",\"Notes\":\"string\"}],\"Notes\":\"string\"},\"DataPartners\":[{\"Name\":\"ICBC\",\"Completed\":false, \"SearchSpeed\":0},{\"Name\":\"BCHydro\",\"Completed\":false,\"SearchSpeed\":0},{\"Name\":\"JCA\",\"Completed\":false,\"SearchSpeed\":1}]}";

        [Test]
        public void json_path_should_return_object()
        {
            Assert.IsInstanceOf(typeof(List<DataPartner>), jsonData.GetDataPartnerSection());
        }

    

        [Test]
        public void json_path_should_return_two_object()
        {
            Assert.AreEqual(2, jsonData.GetDataPartnerSection().Count());
        }

        //[Test]
        //public void json_path_should_update_icbc()
        //{
        //    var sr = jsonData.UpdateDataPartner("ICBC");
        //    Assert.AreEqual(true, sr.DataPartners.ToList().Find(x => x.Name == "ICBC").Completed);
        //    Assert.AreEqual(false, sr.DataPartners.ToList().Find(x => x.Name == "BCHydro").Completed);
        //}

        //[Test]
        //public void json_path_should_update_bchydro()
        //{
        //    var sr = jsonData.UpdateDataPartner("BCHydro");
        //    Assert.AreEqual(true, sr.DataPartners.ToList().Find(x => x.Name == "BCHydro").Completed);
        //    Assert.AreEqual(false, sr.DataPartners.ToList().Find(x => x.Name == "ICBC").Completed);
        //}

        //[Test]
        //public void update_all_should_return_true_all_complete()
        //{
        //    Assert.AreEqual(false, jsonData.AllFastSearchPartnerCompleted());
        //    var sr = jsonData.UpdateDataPartner("BCHydro");
        //    sr = JsonConvert.SerializeObject(sr).UpdateDataPartner("ICBC");
        //    Assert.AreEqual(true, JsonConvert.SerializeObject(sr).AllPartnerCompleted());
          
        //}
        //[Test]
        //public void update_all_should_return_true_all_completed_if_jca_false()
        //{
        //    Assert.AreEqual(false, jsonDataJCA.AllPartnerCompleted());
        //    var sr = jsonDataJCA.UpdateDataPartner("BCHydro");
        //    sr = JsonConvert.SerializeObject(sr).UpdateDataPartner("ICBC");
  
        //    Assert.AreEqual(true, JsonConvert.SerializeObject(sr).AllFastSearchPartnerCompleted());
        //    Assert.AreEqual(false, JsonConvert.SerializeObject(sr).AllPartnerCompleted());

        //}

        //[Test]
        //public void update_all_should_return_true_all_completed_even_if_jca_false()
        //{
        //    Assert.AreEqual(false, jsonDataJCA.AllFastSearchPartnerCompleted());
        //    var sr = jsonDataJCA.UpdateDataPartner("BCHydro");
        //    sr = JsonConvert.SerializeObject(sr).UpdateDataPartner("ICBC");

        //    Assert.AreEqual(true, JsonConvert.SerializeObject(sr).AllFastSearchPartnerCompleted());

        //}

        //[Test]
        //public void update_all_should_return_false_all_complete()
        //{
        //    var sr = jsonData.UpdateDataPartner("BCHydro");
        //    Assert.AreEqual(false, JsonConvert.SerializeObject(sr).AllPartnerCompleted());
        //}
        [Test]
        public void should_return_wave_search_key_format()
        {
            Assert.AreEqual("0000001".DeepSearchKey("ICBC"), "deepsearch-0000001-ICBC");
        }
    }
}
