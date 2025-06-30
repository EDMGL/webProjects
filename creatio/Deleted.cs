public class CharacteristicDto {
    public string CustomerMaterial { get; set; }
    public string CastingQuality { get; set; }
    public string GradeId { get; set; }
    public string GradeDescription { get; set; }
    public string CastingQualityUzun { get; set; }
    public string GradeIdUzun { get; set; }
    public string GradeDescriptionUzun { get; set; }
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