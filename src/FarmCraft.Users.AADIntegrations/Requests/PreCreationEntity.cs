using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmCraft.Users.AADIntegrations.Requests
{
    /*
        Example Request:
        {
             "email": "johnsmith@fabrikam.onmicrosoft.com",
             "identities": [
                 {
                 "signInType":"federated",
                 "issuer":"facebook.com",
                 "issuerAssignedId":"0123456789"
                 }
             ],
             "displayName": "John Smith",
             "objectId": "11111111-0000-0000-0000-000000000000",
             "givenName":"John",
             "surname":"Smith",
             "jobTitle":"Supplier",
             "streetAddress":"1000 Microsoft Way",
             "city":"Seattle",
             "postalCode": "12345",
             "state":"Washington",
             "country":"United States",
             "extension_<extensions-app-id>_CustomAttribute1": "custom attribute value",
             "extension_<extensions-app-id>_CustomAttribute2": "custom attribute value",
             "step": "PostAttributeCollection",
             "client_id":"93fd07aa-333c-409d-955d-96008fd08dd9",
             "ui_locales":"en-US"
        }
    */

    public class PreCreationEntity
    {
        public string ObjectId { get; set; }
    }
}
