namespace Terrasoft.Configuration.UsrOrderServiceNamespace
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

    using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Net;

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UsrOrderService : BaseService
    {
        private SystemUserConnection _systemUserConnection;
        private SystemUserConnection SystemUserConnection
        {
            get
            {
                if (_systemUserConnection == null)
                {
                    _systemUserConnection = (SystemUserConnection)AppConnection.SystemUserConnection;
                }
                return _systemUserConnection;
            }
        }

        public class OrderProductDto
        {
            public string KUNNR { get; set; }
            public string VBELN { get; set; }
            public string POSNR { get; set; }

            public string AUGRU { get; set; }
            public string AUART { get; set; }
            public string KVGR5 { get; set; }
            public string KVGR2 { get; set; }
            public string ZZ_SEKTOR { get; set; }
            public string VTVEG { get; set; }
            public string PRSDT { get; set; }
            public string VENDDAT { get; set; }
            public string INCO1 { get; set; }
            public string INCO2_L { get; set; }
            public string ZTERM { get; set; }
            public string NETWR { get; set; }

            // Yeni eklenen text alanları
            public string BEZEI_AUGRU { get; set; }
            public string BEZEI_AUART { get; set; }
            public string BEZEI_KVGR5 { get; set; }
            public string BEZEI_KVGR2 { get; set; }
            public string BRTXT { get; set; }
            public string VTEXT { get; set; }
            public string ZZ_OZELISTEK_TANIM { get; set; }
            public string ZZ_SERTIFIKA_TANIM { get; set; }
            public string BEZEI_ABGRU { get; set; }
            public string Text1 { get; set; }

            public string CLK_09010 { get; set; }
            public string CLK_00040 { get; set; }
            public string CLK_00050 { get; set; }
            public string CLK_00060 { get; set; }
            public string CLK_00070 { get; set; }
            public string CLK_01651 { get; set; }
            public string CLK_01662 { get; set; }
            public string CLK_00130 { get; set; }
            public string CLK_08070 { get; set; }
            public string ZZ_OZELISTEK { get; set; }
            public string ZZ_SERTIFIKA { get; set; }
            public string ZZGIDECEGIULKEKLM { get; set; }
            public string LANDX { get; set; }
            public string ABGRU { get; set; }
            public string MATNR { get; set; }
            public string MAKTX { get; set; }
            public string ZMENG { get; set; }
            public string LFIMG { get; set; }
        }

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/AddOrder",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped)]
        public List<string> AddOrder(List<OrderProductDto> inputList)
        {
            var resultList = new List<string>();

            foreach (var input in inputList)
            {
                try
                {
                    SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);

                    var accountId = GetAccountIdByCustomerNumber(input.KUNNR);
                    if (!accountId.HasValue)
                    {
                        resultList.Add($"Account bulunamadı (KUNNR: {input.KUNNR})");
                        continue;
                    }

                    var orderSchema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Order");
                    var order = orderSchema.CreateEntity(SystemUserConnection);
                    order.SetDefColumnValues();

                    order.SetColumnValue("AccountId", accountId.Value);
                    SetString(order, "UsrKUNNR", input.KUNNR);
SetDecimal(order,"UsrPOSNR",input.POSNR);

                    SetDate(order, "UsrPRSDT", input.PRSDT);
                    SetDate(order, "UsrVENDDAT", input.VENDDAT);
SetString(order,"UsrVBELN",input.VBELN);
                    SetString(order, "UsrINCO1", input.INCO1);
                    SetString(order, "UsrINCO2_L", input.INCO2_L);
                    SetString(order, "UsrZTERM", input.ZTERM);
                    SetDecimal(order, "UsrNETWR", input.NETWR);

                    // Yeni eklenen text alanları
                    SetString(order, "UsrBEZEI_AUGRU", input.BEZEI_AUGRU);
                    SetString(order, "UsrBEZEI_AUART", input.BEZEI_AUART);
                    SetString(order, "UsrBEZEI_KVGR5", input.BEZEI_KVGR5);
                    SetString(order, "UsrBEZEI_KVGR2", input.BEZEI_KVGR2);
                    SetString(order, "UsrBRTXT", input.BRTXT);
                    SetString(order, "UsrVTEXT", input.VTEXT);
                    SetString(order, "UsrZZ_OZELISTEK_TANIM", input.ZZ_OZELISTEK_TANIM);
                    SetString(order, "UsrZZ_SERTIFIKA_TANIM", input.ZZ_SERTIFIKA_TANIM);
                    SetString(order, "UsrBEZEI_AUBGRU", input.BEZEI_ABGRU);
                    SetString(order, "UsrTEXT1", input.Text1);

                    //Lookup Alanlar
                    if (!string.IsNullOrEmpty(input.AUART))
                    {
                        var auartId = GetOrCreateLookupValue("UsrAUART", input.AUART);
                        if (auartId != Guid.Empty)
                        {
                            order.SetColumnValue("UsrAUARTId", auartId);
                        }
                    }
if (!string.IsNullOrEmpty(input.ABGRU))
                    {
                        var abgruId = GetOrCreateLookupValue("UsrABGRU", input.ABGRU);
                        if (abgruId != Guid.Empty)
                        {
                            order.SetColumnValue("UsrABGRU", abgruId);
                        }
                    }

                    if (!string.IsNullOrEmpty(input.AUGRU))
                    {
                        var augruId = GetOrCreateLookupValue("UsrAUGRU", input.AUGRU);
                        if (augruId != Guid.Empty)
                        {
                            order.SetColumnValue("UsrAUGRUId", augruId);
                        }
                    }

                    if (!string.IsNullOrEmpty(input.KVGR2))
                    {
                        var kvgr2Id = GetOrCreateLookupValue("UsrKVGR2", input.KVGR2);
                        if (kvgr2Id != Guid.Empty)
                        {
                            order.SetColumnValue("UsrKVGR2Id", kvgr2Id);
                        }
                    }

                    if (!string.IsNullOrEmpty(input.KVGR5))
                    {
                        var kvgr5Id = GetOrCreateLookupValue("UsrKVGR5", input.KVGR5);
                        if (kvgr5Id != Guid.Empty)
                        {
                            order.SetColumnValue("UsrKVGR5Id", kvgr5Id);
                        }
                    }

                    if (!string.IsNullOrEmpty(input.ZZ_SEKTOR))
                    {
                        var zzSektorId = GetOrCreateLookupValue("UsrZZ_Sektor", input.ZZ_SEKTOR);
                        if (zzSektorId != Guid.Empty)
                        {
                            order.SetColumnValue("UsrZZ_SektorId", zzSektorId);
                        }
                    }

                    if (!string.IsNullOrEmpty(input.VTVEG))
                    {
                        var vtvegId = GetOrCreateLookupValue("UsrVTWEG", input.VTVEG);
                        if (vtvegId != Guid.Empty)
                        {
                            order.SetColumnValue("UsrVTWEGId", vtvegId);
                        }
                    }


                    order.Save();

                    var orderProductSchema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("OrderProduct");
                    var orderProduct = orderProductSchema.CreateEntity(SystemUserConnection);
                    orderProduct.SetDefColumnValues();

                    orderProduct.SetColumnValue("OrderId", order.PrimaryColumnValue);
                   
                    // Product lookup alanını MATNR ile set et
                    if (!string.IsNullOrEmpty(input.MATNR))
                    {
                        var productId = GetProductIdByMaterialNumber(input.MATNR);
                        if (productId.HasValue)
                        {
                            orderProduct.SetColumnValue("ProductId", productId.Value);
                        }
                    }
                   
                    SetString(orderProduct, "UsrCLK_09010", input.CLK_09010);
                    SetString(orderProduct, "UsrCLK_00040", input.CLK_00040);
                    SetString(orderProduct, "UsrCLK_00050", input.CLK_00050);
                    SetString(orderProduct, "UsrCLK_00060", input.CLK_00060);
                    SetString(orderProduct, "UsrCLK_00070", input.CLK_00070);
                    SetString(orderProduct, "UsrCLK_01651", input.CLK_01651);
                    SetString(orderProduct, "UsrCLK_01662", input.CLK_01662);
                    SetString(orderProduct, "UsrCLK_00130", input.CLK_00130);
                    SetDate(orderProduct, "UsrCLK_08070", input.CLK_08070);
                    SetString(order, "UsrZZ_OZELISTEK", input.ZZ_OZELISTEK);
                    SetString(order, "UsrZZ_SERTIFIKA", input.ZZ_SERTIFIKA);
                    SetString(orderProduct, "UsrZZGIDECEGIULKEKLM", input.ZZGIDECEGIULKEKLM);
                    SetString(orderProduct, "UsrLANDX", input.LANDX);
                    SetString(orderProduct, "UsrABGRU", input.ABGRU);
                    SetString(orderProduct, "UsrMAKTX", input.MAKTX);
                    SetDecimal(orderProduct, "Amount", input.ZMENG);
                    SetDecimal(orderProduct, "UsrLFIMG", input.LFIMG);

                    orderProduct.Save();

                    resultList.Add($"Sipariş oluşturuldu (KUNNR: {input.KUNNR})");
                }
                catch (Exception ex)
                {
                    resultList.Add($"Hata: {ex.Message}");
                }
            }

            return resultList;
        }

        private void SetDecimal(Entity entity, string column, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && decimal.TryParse(value, out decimal result))
            {
                entity.SetColumnValue(column, result);
            }
        }

        private Guid? GetAccountIdByCustomerNumber(string customerNumber)
        {
            if (string.IsNullOrWhiteSpace(customerNumber)) return null;

            var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Account");
            var entity = schema.CreateEntity(SystemUserConnection);
            return entity.FetchFromDB("UsrCustomerNumber", customerNumber) ? entity.PrimaryColumnValue : (Guid?)null;
        }

        private void SetString(Entity entity, string column, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                entity.SetColumnValue(column, value);
            }
        }

        private void SetDate(Entity entity, string column, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && DateTime.TryParse(value, out DateTime dt))
            {
                entity.SetColumnValue(column, dt);
            }
        }

        private Guid GetOrCreateLookupValue(string schemaName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Guid.Empty;
            }

            var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName(schemaName);
            var lookupEntity = schema.CreateEntity(SystemUserConnection);

            // Önce değeri ara
            if (lookupEntity.FetchFromDB("Name", value))
            {
                return lookupEntity.PrimaryColumnValue;
            }

            // Değer bulunamadıysa yeni kayıt oluştur
            lookupEntity.SetDefColumnValues();
            lookupEntity.SetColumnValue("Name", value);
            if (lookupEntity.Save())
            {
                return lookupEntity.PrimaryColumnValue;
            }

            return Guid.Empty;
        }

[OperationContract]
[WebInvoke(
Method = "POST", UriTemplate = "/SyncAllOrderssWithToken",
RequestFormat = WebMessageFormat.Json,
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.Wrapped
)]
public string SyncAllOrderssWithToken()
{
    var userConnection = HttpContext.Current.Session["UserConnection"] as UserConnection;
 var orderSchema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Order");
    var order = orderSchema.CreateEntity(SystemUserConnection);
 
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

    // Sipariş senkronizasyon servisini çağır
    var syncRequest = (HttpWebRequest)WebRequest.Create("https://api.colakoglu.com.tr/gateway/crm/SyncOrder");
    syncRequest.Method = "POST";
    syncRequest.ContentType = "application/json";
    syncRequest.Headers.Add("Authorization", "Bearer " + token);

    var syncPayload = new { customerNo = "",salesDocument="" };
    using (var writer = new StreamWriter(syncRequest.GetRequestStream())) {
        writer.Write(JsonConvert.SerializeObject(syncPayload));
    }



    using (var response = (HttpWebResponse)syncRequest.GetResponse())
    using (var reader = new StreamReader(response.GetResponseStream())) {
        return reader.ReadToEnd(); // gelen cevabı UI'ya döner
    }
}

        private Guid? GetProductIdByMaterialNumber(string materialNumber)
        {
            if (string.IsNullOrWhiteSpace(materialNumber)) return null;

            var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Product");
            var entity = schema.CreateEntity(SystemUserConnection);
            return entity.FetchFromDB("UsrMatnr", materialNumber) ? entity.PrimaryColumnValue : (Guid?)null;
        }
    }
}
