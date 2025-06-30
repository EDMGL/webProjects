namespace Terrasoft.Configuration.UsrAddAccountNamespace
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.ServiceModel.Activation;
    using System.Web;
    using Terrasoft.Core;
    using Terrasoft.Core.Entities;
    using Terrasoft.Core.Entities.Extensions;
    using Terrasoft.Web.Common;
    using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Net;

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UsrAddAccount : BaseService
    {
        private SystemUserConnection _systemUserConnection;
        private SystemUserConnection SystemUserConnection {
            get {
                return _systemUserConnection ?? (_systemUserConnection = (SystemUserConnection)AppConnection.SystemUserConnection);
            }
        }

        

        public class CustomerSummaryDto {
            public string KUNNR { get; set; } // UsrCustomerNumber
            public decimal? OMENG { get; set; } // UsrOpenOrderTonnagee
            public decimal? SVEMT { get; set; } // UsrOpenReferralQuantity
            public decimal? ZMENG_GUNCELYIL { get; set; } // UsrClosedTonnageCurrent
            public decimal? ZMENG_GECENYIL { get; set; } // UsrClosedTonnagePrevious
            public decimal? ZTOPLAM_TONAJ { get; set; } // UsrTotalTonnagee
            public decimal? KALAB_032 { get; set; } // UsrStockQuantityy
            public decimal? KALAB_TOP { get; set; } // UsrTotalStockQuantityy
            public string ERDAT { get; set; } // UsrLastOrderDate
        }

        public class ContactDto {
            public string KUNNR { get; set; } // UsrKUNNR
            public string NAME1 { get; set; } // UsrNAME1
            public string NAME2 { get; set; } // UsrNAME2
            public string PAFKT { get; set; } // UsrPAFKT (lookup)
            public string TEL_NUMBER { get; set; } // UsrTEL_NUMBER
            public string FAX_NUMBER { get; set; } // UsrFAX_NUMBER
            public string SMTP_ADDRESS { get; set; } // UsrSMTP_ADDRESS
            public string ZZMUHATTAP { get; set; } // UsrZZMUHATTAP
        }

        public class AccountDto {
            public string CustomerNumber { get; set; }
            public string Name { get; set; }
            public string SecondName { get; set; }
            public string StreetHouseNumber { get; set; }
            public string Town { get; set; }
            public string Zip { get; set; }
            public string City { get; set; }
            public string Phone { get; set; }
            public string SectorSwitch { get; set; }
            public string CustomerAccountGroup { get; set; }
            public string District { get; set; }
            public string CentralRegistryBlocking { get; set; }
            public string CRMCustomerNumber { get; set; }
            public string CustomerZone { get; set; }
            public string BRTXT { get; set; }
            public string BZTXT { get; set; }
            public string TXT30 { get; set; }
        }

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/AddAccountV2",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped)]
        public List<string> AddAccountV2(List<AccountDto> accounts) {
            var resultList = new List<string>();

            foreach (var input in accounts) {
                try {
                    SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);

                    var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Account");
                    var account = schema.CreateEntity(SystemUserConnection);

                    bool recordFound = false;

                    if (!string.IsNullOrEmpty(input.CRMCustomerNumber)) {
                        recordFound = account.FetchFromDB("UsrCRMCustomerNumber", input.CRMCustomerNumber);
                    }

                    if (!recordFound && !string.IsNullOrEmpty(input.CustomerNumber)) {
                        recordFound = account.FetchFromDB("UsrCustomerNumber", input.CustomerNumber);
                    }

                    if (!recordFound) {
                        account.SetDefColumnValues();
                    }

                    SetStringValue(account, "UsrCustomerNumber", input.CustomerNumber);
                    SetStringValue(account, "Name", input.Name);
                    SetStringValue(account, "UsrSecondName", input.SecondName);
                    SetStringValue(account, "UsrStreetHouseNumber", input.StreetHouseNumber);
                    SetStringValue(account, "Zip", input.Zip);
                    SetStringValue(account, "Phone", input.Phone);
                    SetStringValue(account, "UsrCRMCustomerNumber", input.CRMCustomerNumber);
                    SetStringValue(account, "UsrCentralRegistryBlocking", input.CentralRegistryBlocking);
                    SetStringValue(account, "UsrBRTXT", input.BRTXT);
                    SetStringValue(account, "UsrTXT30", input.TXT30);

                    // Lookup alanları için işlemler
                    if (!string.IsNullOrEmpty(input.Town)) {
                        var townId = GetOrCreateLookupValue("UsrTown", input.Town);
                        if (townId != Guid.Empty) {
                            account.SetColumnValue("UsrTownId", townId);
                        }
                    }

                    if (!string.IsNullOrEmpty(input.City)) {
                        var cityId = GetOrCreateLookupValue("UsrCity", input.City);
                        if (cityId != Guid.Empty) {
                            account.SetColumnValue("UsrCityId", cityId);
                        }
                    }

                    if (!string.IsNullOrEmpty(input.District)) {
                        var districtId = GetOrCreateLookupValue("UsrDistrict", input.District);
                        if (districtId != Guid.Empty) {
                            account.SetColumnValue("UsrDistrictId", districtId);
                        }
                    }

                    if (!string.IsNullOrEmpty(input.SectorSwitch)) {
                        var sectorSwitchId = GetOrCreateLookupValue("UsrSectorSwitch", input.SectorSwitch);
                        if (sectorSwitchId != Guid.Empty) {
                            account.SetColumnValue("UsrSectorSwitchId", sectorSwitchId);
                        }
                    }

                    if (!string.IsNullOrEmpty(input.CustomerAccountGroup)) {
                        var customerAccountGroupId = GetOrCreateLookupValue("UsrCustomerAccountGroup", input.CustomerAccountGroup);
                        if (customerAccountGroupId != Guid.Empty) {
                            account.SetColumnValue("UsrCustomerAccountGroupId", customerAccountGroupId);
                        }
                    }

                    if (!string.IsNullOrEmpty(input.CustomerZone)) {
                        var customerZoneId = GetOrCreateLookupValue("UsrCustomerZone", input.CustomerZone);
                        if (customerZoneId != Guid.Empty) {
                            account.SetColumnValue("UsrCustomerZoneId", customerZoneId);
                        }
                    }

                    if (!string.IsNullOrEmpty(input.BZTXT)) {
                        var bztxtId = GetOrCreateLookupValue("UsrBZTXT", input.BZTXT);
                        if (bztxtId != Guid.Empty) {
                            account.SetColumnValue("UsrBZTXTId", bztxtId);
                        }
                    }

                    var isSaved = account.Save();
                    resultList.Add(isSaved ? account.PrimaryColumnValue.ToString() : "Save failed");

                } catch (Exception ex) {
                    resultList.Add($"Hata oluştu: {ex.Message}");
                }
            }

            return resultList;
        }



[OperationContract]
[WebInvoke(
Method = "POST", UriTemplate = "/SyncCustomerWithToken",
RequestFormat = WebMessageFormat.Json,
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.Wrapped
)]
public string SyncCustomerWithToken(Guid accountId)
{
    var userConnection = HttpContext.Current.Session["UserConnection"] as UserConnection;
 var accountSchema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Account");
    var account = accountSchema.CreateEntity(SystemUserConnection);
    if (!account.FetchFromDB(accountId)) {
        return "Account not found.";
    }
    var customerNo = account.GetTypedColumnValue<string>("UsrCustomerNumber");
    if (string.IsNullOrEmpty(customerNo)) {
        return "UsrCustomerNumber is empty.";
    }
    // Token al
    var tokenRequest = (HttpWebRequest)WebRequest.Create("https://api.colakoglu.com.tr/auth/api/authentication/authenticate");
    tokenRequest.Method = "POST";
    tokenRequest.ContentType = "application/json";
    var payload = new {
        Username = "crmuser",
        Password = "CRM..Uz3r!2025",
        appKey = "CRM_SVC"
    };
    using (var writer = new StreamWriter(tokenRequest.GetRequestStream())) {
        writer.Write(JsonConvert.SerializeObject(payload));
    }
    string token;
    using (var response = (HttpWebResponse)tokenRequest.GetResponse())
    using (var reader = new StreamReader(response.GetResponseStream())) {
        var result = reader.ReadToEnd();
        var tokenObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
        token = tokenObj["token"].ToString();
    }

    // Müşteri senkronizasyon servisini çağır
    var syncRequest = (HttpWebRequest)WebRequest.Create("https://api.colakoglu.com.tr/gateway/crm/SyncCustomerSummary");
    syncRequest.Method = "POST";
    syncRequest.ContentType = "application/json";
    syncRequest.Headers.Add("Authorization", "Bearer " + token);

    var syncPayload = new { customerNo = customerNo };
    using (var writer = new StreamWriter(syncRequest.GetRequestStream())) {
        writer.Write(JsonConvert.SerializeObject(syncPayload));
    }

 var syncRequest2 = (HttpWebRequest)WebRequest.Create("https://api.colakoglu.com.tr/gateway/crm/SyncCustomer");
    syncRequest2.Method = "POST";
    syncRequest2.ContentType = "application/json";
    syncRequest2.Headers.Add("Authorization", "Bearer " + token);

    var syncPayload2 = new { customerNo = customerNo };
    using (var writer = new StreamWriter(syncRequest2.GetRequestStream())) {
        writer.Write(JsonConvert.SerializeObject(syncPayload2));
    }

    using (var response = (HttpWebResponse)syncRequest.GetResponse())
    using (var reader = new StreamReader(response.GetResponseStream())) {
        return reader.ReadToEnd(); // gelen cevabı UI'ya döner
    }
}


[OperationContract]
[WebInvoke(
Method = "POST", UriTemplate = "/SyncAllCustomersWithToken",
RequestFormat = WebMessageFormat.Json,
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.Wrapped
)]
public string SyncAllCustomersWithToken()
{
    var userConnection = HttpContext.Current.Session["UserConnection"] as UserConnection;
 var accountSchema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Account");
    var account = accountSchema.CreateEntity(SystemUserConnection);
 
    // Token al
    var tokenRequest = (HttpWebRequest)WebRequest.Create("https://api.colakoglu.com.tr/auth/api/authentication/authenticate");
    tokenRequest.Method = "POST";
    tokenRequest.ContentType = "application/json";
    var payload = new {
        Username = "crmuser",
        Password = "CRM..Uz3r!2025",
        appKey = "CRM_SVC"
    };
    using (var writer = new StreamWriter(tokenRequest.GetRequestStream())) {
        writer.Write(JsonConvert.SerializeObject(payload));
    }
    string token;
    using (var response = (HttpWebResponse)tokenRequest.GetResponse())
    using (var reader = new StreamReader(response.GetResponseStream())) {
        var result = reader.ReadToEnd();
        var tokenObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
        token = tokenObj["token"].ToString();
    }

    // Müşteri senkronizasyon servisini çağır
    var syncRequest = (HttpWebRequest)WebRequest.Create("https://api.colakoglu.com.tr/gateway/crm/SyncCustomer");
    syncRequest.Method = "POST";
    syncRequest.ContentType = "application/json";
    syncRequest.Headers.Add("Authorization", "Bearer " + token);

    var syncPayload = new { customerNo = "" };
    using (var writer = new StreamWriter(syncRequest.GetRequestStream())) {
        writer.Write(JsonConvert.SerializeObject(syncPayload));
    }

   var syncRequest2 = (HttpWebRequest)WebRequest.Create("https://api.colakoglu.com.tr/gateway/crm/SyncCustomerSummary");
    syncRequest2.Method = "POST";
    syncRequest2.ContentType = "application/json";
    syncRequest2.Headers.Add("Authorization", "Bearer " + token);

    var syncPayload2 = new { customerNo = "" };
    using (var writer = new StreamWriter(syncRequest2.GetRequestStream())) {
        writer.Write(JsonConvert.SerializeObject(syncPayload2));
    }

    using (var response = (HttpWebResponse)syncRequest.GetResponse())
    using (var reader = new StreamReader(response.GetResponseStream())) {
        return reader.ReadToEnd(); // gelen cevabı UI'ya döner
    }
}


        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/AddCustomerSummaryList",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped)]
        public List<string> AddCustomerSummaryList(List<CustomerSummaryDto> customers) {
            var resultList = new List<string>();

            foreach (var input in customers) {
                try {
                    SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);

                    var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Account");
                    var account = schema.CreateEntity(SystemUserConnection);

                    bool recordFound = false;
                    if (!string.IsNullOrEmpty(input.KUNNR)) {
                        recordFound = account.FetchFromDB("UsrCustomerNumber", input.KUNNR);
                    }

                    if (!recordFound) {
                        resultList.Add($"Hata: KUNNR {input.KUNNR} ile kayıt bulunamadı.");
                        continue;
                    }

                    SetDecimalValue(account, "UsrOpenOrderTonnagee", input.OMENG);
                    SetDecimalValue(account, "UsrOpenReferralQuantity", input.SVEMT);
                    SetDecimalValue(account, "UsrClosedTonnageCurrent", input.ZMENG_GUNCELYIL);
                    SetDecimalValue(account, "UsrClosedTonnagePrevious", input.ZMENG_GECENYIL);
                    SetDecimalValue(account, "UsrTotalTonnagee", input.ZTOPLAM_TONAJ);
                    SetDecimalValue(account, "UsrStockQuantityy", input.KALAB_032);
                    SetDecimalValue(account, "UsrTotalStockQuantityy", input.KALAB_TOP);
                    SetDateValue(account, "UsrLastOrderDate", input.ERDAT);

                    var isSaved = account.Save();
                    resultList.Add(isSaved ? $"KUNNR {input.KUNNR} başarıyla güncellendi." : $"KUNNR {input.KUNNR} güncellenemedi.");
                } catch (Exception ex) {
                    resultList.Add($"Hata: KUNNR {input.KUNNR}, {ex.Message}");
                }
            }

            return resultList;
        }

 [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/AddContactList",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped)]
        public List<string> AddContactList(List<ContactDto> contacts) {
            var resultList = new List<string>();

            foreach (var input in contacts) {
                try {
                    SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);

                    var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Contact");
                    var contact = schema.CreateEntity(SystemUserConnection);

                    // Her seferinde yeni kayıt oluştur
                    contact.SetDefColumnValues();

                    // Temel alanları set et
                    SetStringValue(contact, "UsrKUNNR", input.KUNNR);
                    SetStringValue(contact, "UsrNAME1", input.NAME1);
                    SetStringValue(contact, "UsrNAME2", input.NAME2);
                    SetStringValue(contact, "UsrTEL_NUMBER", input.TEL_NUMBER);
                    SetStringValue(contact, "UsrFAX_NUMBER", input.FAX_NUMBER);
                    SetStringValue(contact, "UsrSMTP_ADDRESS", input.SMTP_ADDRESS);
                    SetStringValue(contact, "UsrZZMUHATTAP", input.ZZMUHATTAP);

                    // Name alanını NAME1 + NAME2 şeklinde set et
                    var fullName = string.Empty;
                    if (!string.IsNullOrEmpty(input.NAME1)) {
                        fullName = input.NAME1.Trim();
                    }
                    if (!string.IsNullOrEmpty(input.NAME2)) {
                        fullName += (!string.IsNullOrEmpty(fullName) ? " " : "") + input.NAME2.Trim();
                    }
                    if (!string.IsNullOrEmpty(fullName)) {
                        contact.SetColumnValue("Name", fullName);
                    }

                    // PAFKT lookup alanı için işlem
                    if (!string.IsNullOrEmpty(input.PAFKT)) {
                        var pafktId = GetOrCreateLookupValue("UsrPAFKT", input.PAFKT);
                        if (pafktId != Guid.Empty) {
                            contact.SetColumnValue("UsrPAFKTId", pafktId);
                        }
                    }

                    // KUNNR ile Account lookup'ını set et
                    if (!string.IsNullOrEmpty(input.KUNNR)) {
                        var accountSchema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Account");
                        var account = accountSchema.CreateEntity(SystemUserConnection);
                       
                        if (account.FetchFromDB("UsrCustomerNumber", input.KUNNR)) {
                            contact.SetColumnValue("AccountId", account.PrimaryColumnValue);
                        }
                    }

                    var isSaved = contact.Save();
                    resultList.Add(isSaved ? $"KUNNR {input.KUNNR} başarıyla kaydedildi." : $"KUNNR {input.KUNNR} kaydedilemedi.");

                } catch (Exception ex) {
                    resultList.Add($"Hata: KUNNR {input.KUNNR}, {ex.Message}");
                }
            }

            return resultList;
        }


        private void SetDateValue(Entity entity, string columnName, string value) {
            if (!string.IsNullOrEmpty(value)) {
              if(DateTime.TryParse(value, out DateTime parsedDate) && parsedDate >DateTime.MinValue){
 entity.SetColumnValue(columnName, parsedDate);
 }
            }
        }
        private void SetDecimalValue(Entity entity, string columnName, decimal? value) {
            if (value.HasValue) {
                entity.SetColumnValue(columnName, value.Value);
            }
        }

        private void SetStringValue(Entity entity, string columnName, string value) {
            if (!string.IsNullOrEmpty(value)) {
                entity.SetColumnValue(columnName, value);
            }
        }

        private Guid GetOrCreateLookupValue(string schemaName, string value) {
            if (string.IsNullOrEmpty(value)) {
                return Guid.Empty;
            }

            var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName(schemaName);
            var lookupEntity = schema.CreateEntity(SystemUserConnection);

            // Önce değeri ara
            if (lookupEntity.FetchFromDB("Name", value)) {
                return lookupEntity.PrimaryColumnValue;
            }

            // Değer bulunamadıysa yeni kayıt oluştur
            lookupEntity.SetDefColumnValues();
            lookupEntity.SetColumnValue("Name", value);
            if (lookupEntity.Save()) {
                return lookupEntity.PrimaryColumnValue;
            }

            return Guid.Empty;
        }
    }
}



