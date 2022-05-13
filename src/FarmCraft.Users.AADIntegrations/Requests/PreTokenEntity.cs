/*
    https://docs.microsoft.com/en-us/azure/active-directory-b2c/add-api-connector?pivots=b2c-user-flow
    Example Request:
    {
         "clientId": "231c70e8-8424-48ac-9b5d-5623b9e4ccf3",
         "step": "PreTokenApplicationClaims",
         "ui_locales":"en-US"
         "email": "johnsmith@fabrikam.onmicrosoft.com",
         "identities": [
             {
             "signInType":"federated",
             "issuer":"facebook.com",
             "issuerAssignedId":"0123456789"
             }
         ],
         "displayName": "John Smith",
         "extension_<extensions-app-id>_CustomAttribute1": "custom attribute value",
         "extension_<extensions-app-id>_CustomAttribute2": "custom attribute value",
    }
*/

namespace FarmCraft.Users.AADIntegrations.Requests
{
    public class PreTokenEntity
    {
        public string ObjectId { get; set; }
    }
}
