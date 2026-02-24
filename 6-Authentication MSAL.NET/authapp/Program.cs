using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using dotenv.net;

// Loading env from file
DotEnv.Load();
var envVars = DotEnv.Read();

// Retrieve Azure ID and tenant ID from env
string _clientId = envVars["CLIENT_ID"];
string _tenantId = envVars["TENANT_ID"];

// Define the scopes required for authentication
string[] _scopes = { "User.Read" };

// Build the MSAL public client application with authority and redirect URI
var app = PublicClientApplicationBuilder.Create(_clientId)
    .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
    .WithDefaultRedirectUri()
    .Build();

// Attempt to acquire an access token silently or interactively.
// If the interactive browser is unavailable (e.g., headless Linux),
// fall back to device-code flow and retry on expiration.
AuthenticationResult result = null;
try
{
    var accounts = await app.GetAccountsAsync();
    result = await app.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
}
catch (MsalUiRequiredException)
{
    try
    {
        result = await app.AcquireTokenInteractive(_scopes).ExecuteAsync();
    }
    catch (MsalClientException ex) when (ex.ErrorCode == "linux_xdg_open_failed")
    {
        Console.WriteLine("Interactive browser unavailable; using device code flow.");

        const int maxAttempts = 3;
        int attempt = 0;
        while (attempt < maxAttempts)
        {
            attempt++;
            try
            {
                result = await app.AcquireTokenWithDeviceCode(_scopes, deviceCodeResult =>
                {
                    Console.WriteLine(deviceCodeResult.Message);
                    return Task.CompletedTask;
                }).ExecuteAsync();

                // success
                break;
            }
            catch (MsalClientException dex) when (dex.ErrorCode == "code_expired")
            {
                Console.WriteLine($"Device code expired (attempt {attempt}/{maxAttempts}). Retrying...");
                if (attempt >= maxAttempts)
                {
                    throw;
                }
                await Task.Delay(1000);
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error during token acquisition: {ex.Message}");
    Environment.Exit(1);
}

if (result == null)
{
    Console.WriteLine("Failed to acquire a token.");
    Environment.Exit(1);
}

// Output the acquired access token to the console
Console.WriteLine($"Access Token:\n{result.AccessToken}");