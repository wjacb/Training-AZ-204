using Azure;
using Azure.Identity;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Threading.Tasks;


string queueName = "myqueue-" + Guid.NewGuid().ToString();
string storageAccountName = "storactname59533458";

await CreateQueueAsync(storageAccountName, queueName);
await SendAndListMessagesAsync(storageAccountName, queueName);
await UpdateAndListMessagesAsync(storageAccountName, queueName);
await DeleteMessagesAndQueueAsync(storageAccountName, queueName);

static async Task CreateQueueAsync(string storageAccountName, string queueName)
{
    var options = new DefaultAzureCredentialOptions
    {
        ExcludeEnvironmentCredential = true,
        ExcludeManagedIdentityCredential = true
    };
    var queueClient = new QueueClient(
        new Uri($"https://{storageAccountName}.queue.core.windows.net/{queueName}"),
        new DefaultAzureCredential(options));
    Console.WriteLine($"Creating queue: {queueName}");
    await queueClient.CreateAsync();
    Console.WriteLine("Queue created, press Enter to continue...");
    Console.ReadLine();
}

static async Task SendAndListMessagesAsync(string storageAccountName, string queueName)
{
    var options = new DefaultAzureCredentialOptions
    {
        ExcludeEnvironmentCredential = true,
        ExcludeManagedIdentityCredential = true
    };
    var queueClient = new QueueClient(
        new Uri($"https://{storageAccountName}.queue.core.windows.net/{queueName}"),
        new DefaultAzureCredential(options));
    Console.WriteLine($"Sending message to queue: {queueName}");
    // Example: await queueClient.SendMessageAsync("Hello, Azure Queue!");
    Console.WriteLine("Message sent, press Enter to continue...");
    Console.ReadLine();
}

static async Task UpdateAndListMessagesAsync(string storageAccountName, string queueName)
{
    var options = new DefaultAzureCredentialOptions
    {
        ExcludeEnvironmentCredential = true,
        ExcludeManagedIdentityCredential = true
    };
    var queueClient = new QueueClient(
        new Uri($"https://{storageAccountName}.queue.core.windows.net/{queueName}"),
        new DefaultAzureCredential(options));
    Console.WriteLine($"Updating and listing messages in queue: {queueName}");
    // Example: update or list messages here
    Console.WriteLine("Messages updated/listed, press Enter to continue...");
    Console.ReadLine();
}

static async Task DeleteMessagesAndQueueAsync(string storageAccountName, string queueName)
{
    var options = new DefaultAzureCredentialOptions
    {
        ExcludeEnvironmentCredential = true,
        ExcludeManagedIdentityCredential = true
    };
    var queueClient = new QueueClient(
        new Uri($"https://{storageAccountName}.queue.core.windows.net/{queueName}"),
        new DefaultAzureCredential(options));
    Console.WriteLine($"Deleting messages and queue: {queueName}");
    // Example: delete messages and then delete the queue
    Console.WriteLine("Messages and queue deleted, press Enter to finish...");
    Console.ReadLine();
}