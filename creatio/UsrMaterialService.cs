namespace Terrasoft.Configuration.UsrMaterialServiceNamespace
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.ServiceModel.Activation;
    using System.Web;
    using System.Linq;
    using Terrasoft.Core;
    using Terrasoft.Core.Entities;
    using Terrasoft.Core.Entities.Extensions;
    using Terrasoft.Web.Common;

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UsrMaterialService : BaseService
    {
        private SystemUserConnection _systemUserConnection;
        private SystemUserConnection SystemUserConnection {
            get {
                return _systemUserConnection ?? (_systemUserConnection = (SystemUserConnection)AppConnection.SystemUserConnection);
            }
        }

        public class MaterialDto {
            public string MaterialNumber { get; set; }
            public string MaterialName { get; set; }
        }

        public class FlatMaterialDto {
            public string GradeId { get; set; }
            public string CustomerMaterial { get; set; }
            public string CastingQuality { get; set; }
            public string GradeDescription { get; set; }
            public string ThicknessMin { get; set; }
            public string ThicknessMax { get; set; }
            public string EAFBOF { get; set; }
            public string ApplicationQuality { get; set; }
        }
public class CharacteristicDto {
    public string CustomerMaterial { get; set; }
    public string CastingQuality { get; set; }
    public string GradeId { get; set; }
    public string GradeDescription { get; set; }
    public string CastingQualityUzun { get; set; }
    public string GradeIdUzun { get; set; }
    public string GradeDescriptionUzun { get; set; }
}

public class LicenseMaterialDto {
    public string LIFNR { get; set; }         // Muhatap
    public string MATNR { get; set; }         // Malzeme
    public string LISANS { get; set; }        // Lisans
    public string TARIH { get; set; }         // Belge Geçerlilik Tarihi (DATS - YYYYMMDD)
    public string MAKTX { get; set; }         // Malzeme Tanımı
    public string ZATIK { get; set; }         // Atık
    public string ZATIKTNM { get; set; }      // Atık Tanım
    public string ZALISTRH { get; set; }      // Alış Tarih (DATS - YYYYMMDD)
    public string ZLISANSADR { get; set; }    // Lisans Adresi
    public string ZLISANSBLG { get; set; }    // Lisans Belge
}

   

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/AddMaterial",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped)]
        public List<string> AddMaterial(List<MaterialDto> materials) {
            var resultList = new List<string>();

            foreach (var item in materials) {
                try {
                    SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);
                    var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("Product");
                    var material = schema.CreateEntity(SystemUserConnection);

                    if (!string.IsNullOrEmpty(item.MaterialNumber) && material.FetchFromDB("UsrMatnr", item.MaterialNumber)) {
                        // Kayıt bulundu, güncelle
                    } else {
                        material.SetDefColumnValues();
                    }

                    material.SetColumnValue("UsrMatnr", item.MaterialNumber);
                    material.SetColumnValue("UsrMaktx", item.MaterialName);
                    material.SetColumnValue("Name", item.MaterialName);

                    resultList.Add(material.Save()
                        ? material.PrimaryColumnValue.ToString()
                        : "Save failed");
                } catch (Exception ex) {
                    resultList.Add($"Hata oluştu: {ex.Message}");
                }
            }
            return resultList;
        }

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/AddFlatMaterial",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped)]
        public List<string> AddFlatMaterial(List<FlatMaterialDto> materials) {
            var resultList = new List<string>();

            foreach (var input in materials) {
                try {
                    SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);
                    var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("UsrAsset");
                    var material = schema.CreateEntity(SystemUserConnection);

                    if (!string.IsNullOrEmpty(input.GradeId) && material.FetchFromDB("UsrBKALI_NEW", input.GradeId)) {
                        // kayıt bulundu, güncelle
                    } else {
                        material.SetDefColumnValues();
                    }

                    SetStringValue(material, "UsrBKALI_NEW", input.GradeId);
SetStringValue(material, "UsrName", input.CustomerMaterial);
                    SetStringValue(material, "UsrBKALIT", input.GradeDescription);
                    SetStringValue(material, "UsrEAFBOF_Text", input.EAFBOF);
                    SetDecimalValue(material, "UsrMIKAL", input.ThicknessMin);
                    SetDecimalValue(material, "UsrMAKAL", input.ThicknessMax);
                    SetStringValue(material, "UsrKDMAT_Text", input.CustomerMaterial);
                    SetStringValue(material,"UsrDKALI_Text", input.CastingQuality);
                    SetStringValue(material, "UsrKALITE_Text", input.ApplicationQuality);
SetStringValue(material,"UsrMaterialType","Yassı Malzeme");

                    resultList.Add(material.Save()
                        ? material.PrimaryColumnValue.ToString()
                        : "Save failed");
                } catch (Exception ex) {
                    resultList.Add($"Hata oluştu: {ex.Message}");
                }
            }
            return resultList;
        }

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/AddLongMaterial",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped)]
        public List<string> AddLongMaterial(List<FlatMaterialDto> materials) {
            var resultList = new List<string>();

            foreach (var input in materials) {
                try {
                    SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);
                    var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("UsrAsset");
                    var material = schema.CreateEntity(SystemUserConnection);
 
if (!string.IsNullOrEmpty(input.GradeId) && material.FetchFromDB("UsrBKALI_NEW", input.GradeId)) {
                        // kayıt bulundu, güncelle
                    } else {
                        material.SetDefColumnValues();
                    }

                    SetStringValue(material, "UsrBKALI_NEW", input.GradeId);
SetStringValue(material, "UsrName", input.CustomerMaterial);
                    SetStringValue(material, "UsrBKALIT", input.GradeDescription);
                    SetStringValue(material, "UsrEAFBOF_Text", input.EAFBOF);
                    SetDecimalValue(material, "UsrMIKAL", input.ThicknessMin);
                    SetDecimalValue(material, "UsrMAKAL", input.ThicknessMax);
                    SetStringValue(material, "UsrKDMAT_Text", input.CustomerMaterial);
                    SetStringValue(material,"UsrDKALI_Text", input.CastingQuality);
                    SetStringValue(material, "UsrKALITE_Text", input.ApplicationQuality);
SetStringValue(material,"UsrMaterialType","Uzun Malzeme");

                    resultList.Add(material.Save()
                        ? material.PrimaryColumnValue.ToString()
                        : "Save failed");
                } catch (Exception ex) {
                    resultList.Add($"Hata oluştu: {ex.Message}");
                }
            }
            return resultList;
        }

[OperationContract]
[WebInvoke(Method = "POST",
           UriTemplate = "/AddCharacteristic",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.Wrapped)]
public List<string> AddCharacteristic(List<CharacteristicDto> characteristics) {
    var resultList = new List<string>();

    foreach (var input in characteristics) {
        try {
            SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);
            var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("UsrAsset");
            var asset = schema.CreateEntity(SystemUserConnection);

            asset.SetDefColumnValues();

            SetStringValue(asset, "UsrKDMAT_Text", input.CustomerMaterial);
            SetStringValue(asset, "UsrDKALI_HRC_Text", input.CastingQuality);
            SetStringValue(asset, "UsrBKALI_NEW_HRC", input.GradeId);
            SetStringValue(asset, "UsrBKALIT_HRC", input.GradeDescription);
            SetStringValue(asset, "UsrDKALI_UZUN_Text", input.CastingQualityUzun);
            SetStringValue(asset, "UsrBKALI_NEW_UZUN", input.GradeIdUzun);
            SetStringValue(asset, "BKALIT_UZUN", input.GradeDescriptionUzun);

            resultList.Add(asset.Save()
                ? asset.PrimaryColumnValue.ToString()
                : "Save failed");
        } catch (Exception ex) {
            resultList.Add($"Hata oluştu: {ex.Message}");
        }
    }

    return resultList;
}



[OperationContract]
[WebInvoke(Method = "POST",
           UriTemplate = "/AddLicenseMaterials",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.Wrapped)]
public List<string> AddLicenseMaterials(List<LicenseMaterialDto> materials) {
    var resultList = new List<string>();

    foreach (var input in materials) {
        try {
            SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);
            var schema = SystemUserConnection.EntitySchemaManager.GetInstanceByName("###OBJECT###");
            var entity = schema.CreateEntity(SystemUserConnection);

            // Duplicate kontrolü (örnek olarak MATNR baz alınmıştır, sen daha sonra detaylandırabilirsin)
            if (!string.IsNullOrEmpty(input.MATNR) && entity.FetchFromDB("###MATNR_FIELD###", input.MATNR)) {
                // kayıt bulundu, güncelle
            } else {
                entity.SetDefColumnValues();
            }

            SetStringValue(entity, "###LIFNR_FIELD###", input.LIFNR);
            SetStringValue(entity, "###MATNR_FIELD###", input.MATNR);
            SetStringValue(entity, "###LISANS_FIELD###", input.LISANS);
            SetDateValue(entity, "###TARIH_FIELD###", input.TARIH);
            SetStringValue(entity, "###MAKTX_FIELD###", input.MAKTX);
            SetStringValue(entity, "###ZATIK_FIELD###", input.ZATIK);
            SetStringValue(entity, "###ZATIKTNM_FIELD###", input.ZATIKTNM);
            SetDateValue(entity, "###ZALISTRH_FIELD###", input.ZALISTRH);
            SetStringValue(entity, "###ZLISANSADR_FIELD###", input.ZLISANSADR);
            SetStringValue(entity, "###ZLISANSBLG_FIELD###", input.ZLISANSBLG);

            resultList.Add(entity.Save()
                ? entity.PrimaryColumnValue.ToString()
                : "Save failed");
        } catch (Exception ex) {
            resultList.Add($"Hata oluştu: {ex.Message}");
        }
    }

    return resultList;
}

private void SetDateValue(Entity entity, string columnName, string dateStr) {
    if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParseExact(dateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var result)) {
        entity.SetColumnValue(columnName, result);
    }
}

   

        private void SetStringValue(Entity entity, string columnName, string value) {
            if (!string.IsNullOrEmpty(value)) {
                entity.SetColumnValue(columnName, value);
            }
        }

        private void SetDecimalValue(Entity entity, string columnName, string value) {
            if (!string.IsNullOrEmpty(value) && decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result)) {
                entity.SetColumnValue(columnName, result);
            }
        }

        private void SetLookupValue(Entity entity, string columnName, string lookupSchema, string lookupName) {
            Guid? lookupId = GetLookupIdByName(lookupSchema, lookupName);
            if (lookupId.HasValue) {
                entity.SetColumnValue(columnName, lookupId.Value);
            } else {
                entity.SetColumnValue(columnName, null);
            }
        }

        private Guid? GetLookupIdByName(string schemaName, string name) {
            if (string.IsNullOrEmpty(name)) return null;
            var esq = new EntitySchemaQuery(SystemUserConnection.EntitySchemaManager, schemaName);
            esq.AddColumn("Id");
            esq.AddColumn("Name");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", name));
            var entity = esq.GetEntityCollection(SystemUserConnection).FirstOrDefault();
            return entity?.PrimaryColumnValue;
        }
    }
}
