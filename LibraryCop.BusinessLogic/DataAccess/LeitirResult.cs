using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.DataAccess
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable IDE1006 // Naming Styles
    public class LeitirResult
    {
        public string beaconO22 { get; set; }
        public Info info { get; set; }
        public Highlights highlights { get; set; }
        public Doc[] docs { get; set; }
        public Timelog timelog { get; set; }
        public object[] facets { get; set; }
    }

    public class Info
    {
        public int totalResultsLocal { get; set; }
        public int totalResultsPC { get; set; }
        public int total { get; set; }
        public int first { get; set; }
        public int last { get; set; }
    }

    public class Highlights
    {
        public object[] termsUnion { get; set; }
    }

    public class Timelog
    {
        public string BUILD_RESULTS_RETRIVE_FROM_DB { get; set; }
        public string CALL_SOLR_GET_IDS_LIST { get; set; }
        public string PRIMA_LOCAL_SEARCH_SET_AVALIABILITY { get; set; }
        public string RETRIVE_COLLECTION_DISCOVERY_INFO { get; set; }
        public string RETRIVE_FROM_DB_COURSE_INFO { get; set; }
        public string RETRIVE_FROM_DB_RECORDS { get; set; }
        public string RETRIVE_FROM_DB_RELATIONS { get; set; }
        public string SET_AVAILABILTY_GET_LIBRARY_DETAILS { get; set; }
        public string SET_AVAILABILTY_HOLDING_DEDUPS { get; set; }
        public string PRIMA_LOCAL_INFO_FACETS_BUILD_DOCS_HIGHLIGHTS { get; set; }
        public string PRIMA_LOCAL_SEARCH_TOTAL { get; set; }
        public int BUILD_BLEND_AND_CACHE_RESULTS { get; set; }
        public int BUILD_COMBINED_RESULTS_MAP { get; set; }
        public int COMBINED_SEARCH_TIME { get; set; }
        public int PROCESS_COMBINED_RESULTS { get; set; }
        public int FEATURED_SEARCH_TIME { get; set; }
    }

    public class Doc
    {
        public string context { get; set; }
        public string adaptor { get; set; }
        public string id { get; set; }
        public Pnx pnx { get; set; }
        public Delivery delivery { get; set; }
        public Enrichment enrichment { get; set; }
    }

    public class Pnx
    {
        public Display display { get; set; }
        public Control control { get; set; }
        public Addata addata { get; set; }
        public Sort sort { get; set; }
        public Facets facets { get; set; }
    }

    public class Display
    {
        public string[] source { get; set; }
        public string[] type { get; set; }
        public string[] language { get; set; }
        public string[] title { get; set; }
        public string[] subject { get; set; }
        public string[] format { get; set; }
        public string[] identifier { get; set; }
        public string[] creationdate { get; set; }
        public string[] lds62 { get; set; }
        public string[] creator { get; set; }
        public string[] publisher { get; set; }
        public string[] mms { get; set; }
        public string[] contributor { get; set; }
        public string[] addtitle { get; set; }
        public string[] series { get; set; }
        public string[] unititle { get; set; }
        public string[] includes { get; set; }
        public string[] place { get; set; }
        public string[] version { get; set; }
        public string[] lds01 { get; set; }
        public string[] lds03 { get; set; }
        public string[] lds05 { get; set; }
        public string[] lds49 { get; set; }
        public string[] lds51 { get; set; }
        public string[] lds60 { get; set; }
        public string[] lds63 { get; set; }
        public string[] lds76 { get; set; }
    }

    public class Control
    {
        public string[] sourcerecordid { get; set; }
        public string[] recordid { get; set; }
        public string sourceid { get; set; }
        public string[] originalsourceid { get; set; }
        public string[] sourcesystem { get; set; }
        public string[] sourceformat { get; set; }
        public string[] score { get; set; }
        public bool isDedup { get; set; }
    }

    public class Addata
    {
        public string[] aulast { get; set; }
        public string[] aufirst { get; set; }
        public string[] auinit { get; set; }
        public string[] au { get; set; }
        public string[] addau { get; set; }
        public string[] creatorfull { get; set; }
        public string[] contributorfull { get; set; }
        public string[] date { get; set; }
        public string[] isbn { get; set; }
        public string[] cop { get; set; }
        public string[] pub { get; set; }
        public string[] oclcid { get; set; }
        public string[] seriestitle { get; set; }
        public string[] format { get; set; }
        public string[] genre { get; set; }
        public string[] ristype { get; set; }
        public string[] btitle { get; set; }
    }

    public class Sort
    {
        public string[] title { get; set; }
        public string[] author { get; set; }
        public string[] creationdate { get; set; }
    }

    public class Facets
    {
        public string[] frbrtype { get; set; }
        public string[] frbrgroupid { get; set; }
    }

    public class Delivery
    {
        public object bestlocation { get; set; }
        public object holding { get; set; }
        public object electronicServices { get; set; }
        public object additionalElectronicServices { get; set; }
        public object filteredByGroupServices { get; set; }
        public object quickAccessService { get; set; }
        public string[] deliveryCategory { get; set; }
        public string[] serviceMode { get; set; }
        public string[] availability { get; set; }
        public string[] availabilityLinks { get; set; }
        public string[] availabilityLinksUrl { get; set; }
        public string displayedAvailability { get; set; }
        public object displayLocation { get; set; }
        public object additionalLocations { get; set; }
        public object physicalItemTextCodes { get; set; }
        public object feDisplayOtherLocations { get; set; }
        public Almainstitutionslist[] almaInstitutionsList { get; set; }
        public object recordInstitutionCode { get; set; }
        public string recordOwner { get; set; }
        public object hasFilteredServices { get; set; }
        public bool digitalAuxiliaryMode { get; set; }
        public bool hideResourceSharing { get; set; }
        public object sharedDigitalCandidates { get; set; }
        public object consolidatedCoverage { get; set; }
        public object electronicContextObjectId { get; set; }
        public Getit1[] GetIt1 { get; set; }
        public object physicalServiceId { get; set; }
        public Link1[] link { get; set; }
        public object hasD { get; set; }
    }

    public class Almainstitutionslist
    {
        public Getitlink[] getitLink { get; set; }
        public string instCode { get; set; }
        public string instName { get; set; }
        public string instId { get; set; }
        public string availabilityStatus { get; set; }
        public string envURL { get; set; }
    }

    public class Getitlink
    {
        public string displayText { get; set; }
        public string linkRecordId { get; set; }
    }

    public class Getit1
    {
        public string category { get; set; }
        public Link[] links { get; set; }
    }

    public class Link
    {
        public bool isLinktoOnline { get; set; }
        public string getItTabText { get; set; }
        public string adaptorid { get; set; }
        public string ilsApiId { get; set; }
        public string link { get; set; }
        public string inst4opac { get; set; }
        public object displayText { get; set; }
        public string id { get; set; }
    }

    public class Link1
    {
        public string id { get; set; }
        public string linkType { get; set; }
        public string linkURL { get; set; }
        public string displayLabel { get; set; }
    }

    public class Enrichment
    {
        public Virtualbrowseobject virtualBrowseObject { get; set; }
        public Bibvirtualbrowseobject bibVirtualBrowseObject { get; set; }
    }

    public class Virtualbrowseobject
    {
        public bool isVirtualBrowseEnabled { get; set; }
        public string callNumber { get; set; }
        public string callNumberBrowseField { get; set; }
    }

    public class Bibvirtualbrowseobject
    {
        public bool isVirtualBrowseEnabled { get; set; }
        public string callNumber { get; set; }
        public string callNumberBrowseField { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
