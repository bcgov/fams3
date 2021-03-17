namespace Fams3Adapter.Dynamics
{
    public static class Keys
    {
        public const string DYNAMICS_STATUS_CODE_FIELD = "statuscode";
        public const string DYNAMICS_STATE_CODE_FIELD = "statecode";
        public const string DYNAMICS_SEARCH_REQUEST_CANCEL_COMMENTS_FIELD = "ssg_cancellationcomment";

        public const string GLOBAL_OPTIONS_SET_DEFINTION_URL_TEMPLATE = "GlobalOptionSetDefinitions(Name='{0}')";

        public const string GLOBAL_STATUS_CODE_URL_TEMPLATE =
            "EntityDefinitions(LogicalName='{0}')/Attributes(LogicalName='statuscode')/Microsoft.Dynamics.CRM.StatusAttributeMetadata?$select=LogicalName&$expand=OptionSet";
        public const string DUPLICATE_DETECTED_ERROR_CODE = "0x80040333";

        public const string BANK_TYPE_OPTION_SET_URL_TEMPLATE =
            "EntityDefinitions(LogicalName='ssg_asset_bankinginformation')/Attributes/Microsoft.Dynamics.CRM.PicklistAttributeMetadata?$select=LogicalName&$filter=LogicalName eq 'ssg_bankaccounttype'&$expand=OptionSet";

    }
}