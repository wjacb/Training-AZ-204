using Microsoft.Graph;
using Azure.Identity;
using Azure.Core;
using dotenv.net;

// Load environment variables from .env file (if present)
DotEnv.Load();
var envVars = DotEnv.Read();

// Read Azure AD app registration values from environment
string clientId = envVars["CLIENT_ID"];
string tenantId = envVars["TENANT_ID"];

// Validate that required environment variables are set
if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(tenantId))
{
    Console.WriteLine("Please set CLIENT_ID and TENANT_ID environment variables.");
    return;
}

// ADD CODE TO DEFINE SCOPE AND CONFIGURE AUTHENTICATION

// Define the Microsoft Graph permission scopes required by this app
var scopes = new[] { "User.Read" };

// Configure interactive browser authentication for the user
var options = new InteractiveBrowserCredentialOptions
{
    ClientId = clientId, // Azure AD app client ID
    TenantId = tenantId, // Azure AD tenant ID
    RedirectUri = new Uri("http://localhost") // Redirect URI for auth flow
};
TokenCredential credential = new InteractiveBrowserCredential(options);

// Create a Microsoft Graph client using the credential
var graphClient = new GraphServiceClient(credential);

// Retrieve and display the user's profile information. If interactive browser
// auth fails (common in headless containers), fall back to device code flow.
Console.WriteLine("Retrieving user profile...");
try
{
    await GetUserProfile(graphClient);
}
catch (Exception ex) when (
    ex.Message.Contains("Unable to open a web page") ||
    (ex.InnerException != null && ex.InnerException.Message.Contains("Unable to open a web page")))
{
    Console.WriteLine("Interactive browser auth failed — falling back to Device Code flow.");

    var deviceOptions = new DeviceCodeCredentialOptions
    {
        ClientId = clientId,
        TenantId = tenantId,
        DeviceCodeCallback = (info, cancellationToken) =>
        {
            Console.WriteLine(info.Message);
            return Task.CompletedTask;
        }
    };

    var deviceCredential = new DeviceCodeCredential(deviceOptions);
    var deviceClient = new GraphServiceClient(deviceCredential);
    await GetUserProfile(deviceClient);
}
catch (Exception ex)
{
    Console.WriteLine($"Error retrieving profile: {ex.Message}");
}

// Function to get and print the signed-in user's profile
async Task GetUserProfile(GraphServiceClient graphClient)
{
    // Call Microsoft Graph /me endpoint to get user info
    var me = await graphClient.Me.GetAsync();
    Console.WriteLine($"Display Name: {me?.DisplayName}");
    Console.WriteLine($"Principal Name: {me?.UserPrincipalName}");
    Console.WriteLine($"User Id: {me?.Id}");
}