# Setup

## App Registrations

Since FarmCraft.Users integrates with Azure AD and Azure B2C, we need to create
the App Registrations within the B2C tenant.

### FarmCraft Users - Core

The FarmCraft Users - Core registration will represent the actor system
(FarmCraft.Users.Core) and allow it to connect to Azure. It also enables
creating a Microsoft Graph subscription to monitor for user changes.

Create an application secret that will be used by a GraphServiceClient to create
a Microsoft Graph Subscription.

Add the Directory.ReadWrite.All application permission to create a Graph
subscription to the /users endpoint

### FarmCraft Users - API

The FarmCraft Users - API registration represents the web api
(FarmCraft.Users.Api) and allows us to leverage Azure AD for authentication.
