using Azure;
using Azure.Identity;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Threading.Tasks;

// Create a unique name for the queue
// TODO: Replace the <YOUR-STORAGE-ACCT-NAME> placeholder 
string queueName = "myqueue-" + Guid.NewGuid().ToString();
string storageAccountName = "storactname59533458";

// ADD CODE TO CREATE A QUEUE CLIENT AND CREATE A QUEUE

// Create a DefaultAzureCredentialOptions object to exclude certain credentials
DefaultAzureCredentialOptions options = new()
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = true
};

// Instantiate a QueueClient to create and interact with the queue
QueueClient queueClient = new QueueClient(
    new Uri($"https://{storageAccountName}.queue.core.windows.net/{queueName}"),
    new DefaultAzureCredential(options));

Console.WriteLine($"Creating queue: {queueName}");

// Create the queue
await queueClient.CreateAsync();

Console.WriteLine("Queue created, press Enter to add messages to the queue...");
Console.ReadLine();

// ADD CODE TO SEND AND LIST MESSAGES

// ADD CODE TO UPDATE A MESSAGE AND LIST MESSAGES

// ADD CODE TO DELETE MESSAGES AND THE QUEUE