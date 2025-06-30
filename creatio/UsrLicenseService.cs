namespace Terrasoft.Configuration.UsrLicenseServiceNamespace
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.ServiceModel.Activation;
    using System.Web;
    using Terrasoft.Core;
    using Terrasoft.Core.Entities;
    using Terrasoft.Web.Common;
    using Terrasoft.Core.Entities.Extensions;

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UsrLicenseService : BaseService
    {
        private SystemUserConnection _systemUserConnection;
        private SystemUserConnection SystemUserConnection {
            get {
                return _systemUserConnection ?? (_systemUserConnection = (SystemUserConnection)AppConnection.SystemUserConnection);
            }
        }

        public class LicenseDto {
            public string LIFNR { get; set; }         // UsrLIFNR
            public string MATNR { get; set; }         // UsrMATNR
            public string LISANS { get; set; }        // UsrLISANS
            public string TARIH { get; set; }         // UsrTARIH (ISO string)
            public string MAKTX { get; set; }         // UsrMAKTX
            public string ZATIK { get; set; }         // UsrZATIK
            public string ZATIKTNM { get; set; }      // UsrZATIKTNM
            public string ZALISTRH { get; set; }      // UsrZALISTRH (ISO string)
            public string ZLISANSADR { get; set; }    // UsrZLISANSADR
            public string ZLISANSBLG { get; set; }    // UsrZLISANSBLG
        }

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/AddLicenseList",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped)]
        public List<string> AddLicenseList(List<LicenseDto> licenses) {
            var resultList = new List<string>();

            foreach (var input in licenses) {
                try {
                    SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);

                    var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("UsrLicense");
                    var license = schema.CreateEntity(SystemUserConnection);
                    license.SetDefColumnValues();

                    // LIFNR -> Account lookup bağlantısı
                    if (!string.IsNullOrEmpty(input.LIFNR)) {
                        var accountSchema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Account");
                        var accountEntity = accountSchema.CreateEntity(SystemUserConnection);

                        if (accountEntity.FetchFromDB("UsrCustomerNumber", input.LIFNR)) {
                            license.SetColumnValue("UsrAccountId", accountEntity.PrimaryColumnValue);
                        }
                    }

                    SetStringValue(license, "UsrLIFNR", input.LIFNR);
                    SetStringValue(license, "UsrMATNR", input.MATNR);
                    SetStringValue(license, "UsrLISANS", input.LISANS);
                    SetStringValue(license, "UsrMAKTX", input.MAKTX);
                    SetStringValue(license, "UsrZATIK", input.ZATIK);
                    SetStringValue(license, "UsrZATIKTNM", input.ZATIKTNM);
                    SetStringValue(license, "UsrZLISANSADR", input.ZLISANSADR);
                    SetStringValue(license, "UsrZLISANSBLG", input.ZLISANSBLG);
                    SetDateValue(license, "UsrTARIH", input.TARIH);
                    SetDateValue(license, "UsrZALISTRH", input.ZALISTRH);

                    var isSaved = license.Save();
                    resultList.Add(isSaved ? license.PrimaryColumnValue.ToString() : "Save failed");
                } catch (Exception ex) {
                    resultList.Add($"Hata oluştu: {ex.Message}");
                }
            }

            return resultList;
        }

        private void SetStringValue(Entity entity, string columnName, string value) {
            if (!string.IsNullOrWhiteSpace(value)) {
                entity.SetColumnValue(columnName, value);
            }
        }

        private void SetDateValue(Entity entity, string columnName, string dateStr) {
            if (!string.IsNullOrWhiteSpace(dateStr) && DateTime.TryParse(dateStr, out DateTime dt)) {
                entity.SetColumnValue(columnName, dt);
            }
        }
    }
}




