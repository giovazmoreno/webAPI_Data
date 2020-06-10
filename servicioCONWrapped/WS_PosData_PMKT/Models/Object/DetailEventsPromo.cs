using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using WS_PosData_PMKT.Models.Base;

namespace WS_PosData_PMKT.Models.Object
{
    [DataContract]
    public class DetailEventsPromo
    {
        [DataMember]
        public NoVisitMotiveWrapped ListNoVisitMotive { get; set; }
        [DataMember]
        public SurveyWrapped ListSurvey { get; set; }
        [DataMember]
        public PhotographicEvidenceWrapped ListTypeEvidence { get; set; }
        [DataMember]
        public ProductsWrapped ListProductsByCategory { get; set; }
        [DataMember]
        public TypeMaterialWrapped ListTypeMaterial { get; set; }
        [DataMember]
        public UbicationProductWrapped ListUbication { get; set; }
        [DataMember]
        public KPIWrapped ListKPI { get; set; }

        
    }

    public class Modules
    {
        public string IdModule { get; set; }
        public string NameModule { get; set; }
    }

    public class NoVisitMotive
    {
        public string IdMotive { get; set; }
        public string NameMotive { get; set; }
    }

    public class Survey
    {
        public string IdSurvey { get; set; }
        public string NameSurvey { get; set; }
        public string Question1 { get; set; }
        public string Question2 { get; set; }
        public string Question3 { get; set; }
        public string Question4 { get; set; }
        public string Question5 { get; set; }
    }

    public class PhotographicEvidence
    {
        public string IdTypeEvidence { get; set; }
        public string NameTypeEvidence { get; set; }
    }

    public class ProductsCategories
    {
        public string IdCategory { get; set; }
        public string NameCategory { get; set; }
        public List<Products> ListProducts {get; set;}
    }

    public class Products
    {
        public string IdProduct { get; set; }
        public string NameProduct { get; set; }
        public string IdCategory { get; set; }
        public string NameCategory { get; set; }
    }

    public class TypeMaterial
    {
        public string IdTypeMaterial { get; set; }
        public string NameTypeMaterial { get; set; }
    }

    public class UbicationProduct
    {
        public string IdUbication { get; set; }
        public string NameUbication { get; set; }
    }

    public class KPI
    {
        public string IdKPI { get; set; }
        public string NameKPI { get; set; }
    }

    public class NoVisitMotiveWrapped : EmptyDetailEventBase
    {
        public List<NoVisitMotive> ListNoVisitMotive { get; set; }

    }

    public class SurveyWrapped : EmptyDetailEventBase
    {
        public List<Survey> ListSurvey { get; set; }

    }

    public class PhotographicEvidenceWrapped : EmptyDetailEventBase
    {
        public List<PhotographicEvidence> ListTypeEvidence { get; set; }
    }
    
    public class ProductsWrapped : EmptyDetailEventBase
    {
        public List<Products> ListProductsByCategory { get; set; }

    }
    
    public class TypeMaterialWrapped : EmptyDetailEventBase
    {
        public List<TypeMaterial> ListTypeMaterial { get; set; }
    }
    
    public class UbicationProductWrapped : EmptyDetailEventBase
    {
        public List<UbicationProduct> ListUbication { get; set; }
    }
    
    public class KPIWrapped : EmptyDetailEventBase
    {
        public List<KPI> ListKPI { get; set; }

    }
    
}