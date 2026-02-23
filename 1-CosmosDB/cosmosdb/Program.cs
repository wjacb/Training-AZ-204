using Microsoft.Azure.Cosmos;
using dotenv.net;

string databaseName = "myDatabaseWjacb"; 
string containerName = "myContainerWjacb"; 

// Load environment
DotEnv.Load();
var envVars = DotEnv.Read();
string cosmosDbAccountUrl = envVars["DOCUMENT_ENDPOINT"];
string accountKey = envVars["ACCOUNT_KEY"];

if (string.IsNullOrEmpty(cosmosDbAccountUrl) || string.IsNullOrEmpty(accountKey))
{
    Console.WriteLine("Please set into .env file the DOCUMENT_ENDPOINT and ACCOUNT_KEY environment variables!!.");
    return;
}

// CREATE THE COSMOS DB 

	CosmosClient client = new(
	    accountEndpoint: cosmosDbAccountUrl,
	    authKeyOrResourceToken: accountKey
	);


try
{
    // CREATE A DB IF IS NOT

	Database database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
	Console.WriteLine($"Created or retrieved database: {database.Id}");


    // CREATE A CONTAINER AND SET NAME WITH ID TO MAKE IT DIFFERENT
	
	Container container = await database.CreateContainerIfNotExistsAsync(
	    id: containerName,
	    partitionKeyPath: "/id"
	);
	Console.WriteLine($"Created or retrieved container: {container.Id}");

    // TYPED ITEM (PRODUCT) TO ADD TO THE CONTAINER

	Product newItem = new Product
	{
	    id = Guid.NewGuid().ToString(), 
	    name = "Sample Item number 1",
	    description = "For deploy this parte of exercise"
	};


    // ADD THE ITEM TO THE CONTAINER
			
	ItemResponse<Product> createResponse = await container.CreateItemAsync(
	    item: newItem,
	    partitionKey: new PartitionKey(newItem.id)
	);
	Console.WriteLine($"Created item with ID: {createResponse.Resource.id}");
	Console.WriteLine($"Request charge: {createResponse.RequestCharge} RUs");

}
catch (CosmosException ex)
{
    // Handle DB-specific exceptions
    
    Console.WriteLine($"Cosmos DB Error: {ex.StatusCode} - {ex.Message}");
}
catch (Exception ex)
{
    // Handle exceptions
    
    Console.WriteLine($"Error: {ex.Message}");
}

// This class represents a product in the Cosmos DB container
public class Product
{
    public string? id { get; set; }
    public string? name { get; set; }
    public string? description { get; set; }
}