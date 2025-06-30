namespace Terrasoft.Configuration.UsrPaymentTermNamespace
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

    // Tek tek DTO
    public class PaymentTermDto {
        public string ZTERM { get; set; }  // Ödeme Şartı Kodu
        public string TEXT1 { get; set; }  // Açıklama
    }

    // Listeyi saran DTO
    public class AddPaymentTermsRequest {
        public List<PaymentTermDto> terms { get; set; }
    }

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UsrPaymentTerm : BaseService
    {
        private SystemUserConnection _systemUserConnection;
        private SystemUserConnection SystemUserConnection {
            get {
                return _systemUserConnection ?? (_systemUserConnection = (SystemUserConnection)AppConnection.SystemUserConnection);
            }
        }

        [OperationContract]
       [WebInvoke(Method = "POST",
           UriTemplate = "/AddPaymentTerms",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.Bare)]
public List<string> AddPaymentTerms(AddPaymentTermsRequest request) {
            var resultList = new List<string>();

            foreach (var input in request.terms) {
                try {
                    SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);

                    var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("UsrPaymentTerm");
                    var entity = schema.CreateEntity(SystemUserConnection);

                    // ZTERM (Name) ile duplicate kontrol
                    if (!string.IsNullOrEmpty(input.ZTERM) && entity.FetchFromDB("UsrZTERM", input.ZTERM)) {
                        // varsa mevcut kayıt alınır
                    } else {
                        entity.SetDefColumnValues();
                    }

                    SetStringValue(entity, "UsrZTERM", input.ZTERM);
                    SetStringValue(entity, "UsrTEXT1", input.TEXT1);

                    resultList.Add(entity.Save()
                        ? entity.PrimaryColumnValue.ToString()
                        : "Save failed");
                } catch (Exception ex) {
                    resultList.Add($"Hata oluştu: {ex.Message}");
                }
            }

            return resultList;
        }

        private void SetStringValue(Entity entity, string columnName, string value) {
            if (!string.IsNullOrEmpty(value)) {
                entity.SetColumnValue(columnName, value);
            }
        }
    }
}
