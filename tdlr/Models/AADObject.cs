using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace tdlr
{
	public class AADObject
	{
		[JsonProperty("odata.type")]
		public string type { get; set; }
		public string objectType { get; set; }
		public string objectId { get; set; }
		public object deletionTimestamp { get; set; }
		public bool? accountEnabled { get; set; }
		public List<object> assignedLicenses { get; set; }
		public List<object> assignedPlans { get; set; }
		public object city { get; set; }
		public object companyName { get; set; }
		public object country { get; set; }
		public object creationType { get; set; }
		public object department { get; set; }
		public object dirSyncEnabled { get; set; }
		public string displayName { get; set; }
		public object facsimileTelephoneNumber { get; set; }
		public string givenName { get; set; }
		public object immutableId { get; set; }
		public object jobTitle { get; set; }
		public object lastDirSyncTime { get; set; }
		public object mail { get; set; }
		public string mailNickname { get; set; }
		public string mobile { get; set; }
		public object onPremisesSecurityIdentifier { get; set; }
		public List<object> otherMails { get; set; }
		public string passwordPolicies { get; set; }
		public object passwordProfile { get; set; }
		public object physicalDeliveryOfficeName { get; set; }
		public object postalCode { get; set; }
		public string preferredLanguage { get; set; }
		public List<object> provisionedPlans { get; set; }
		public List<object> provisioningErrors { get; set; }
		public List<object> proxyAddresses { get; set; }
		public object sipProxyAddress { get; set; }
		public object state { get; set; }
		public object streetAddress { get; set; }
		public string surname { get; set; }
		public object telephoneNumber { get; set; }
		public object usageLocation { get; set; }
		public string userPrincipalName { get; set; }
		public string userType { get; set; }
	}

	public class GraphResponse
	{
		[JsonProperty("odata.metadata")]
		public string metadata { get; set; }
		public List<AADObject> value { get; set; }
		[JsonProperty("odata.nextLink")]
		public string nextLink { get; set; }
	}
}

