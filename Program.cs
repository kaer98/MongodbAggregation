using MongoDB.Driver;
using MongoDB.Bson;



string pass = "atlasPass";
string connectionUri = $"mongodb+srv://myAtlasDBUser:{pass}@myatlasclusteredu.luqajyq.mongodb.net/?retryWrites=true&w=majority&appName=myAtlasClusterEDU";


var client = new MongoClient(connectionUri);

var dataBase = client.GetDatabase("Bank");
var accountsCollection = dataBase.GetCollection<Account>("account");





var matchBalanceStage = Builders<Account>.Filter.Lt(user => user.Balance, 5000);
var projectStage = Builders<Account>.Projection.Expression(u =>
    new
    {
        AccountId = u.AccountId,
        AccountType = u.AccountType,
        Balance = u.Balance,
        GBP = u.Balance / 1.30M
    });



var aggregate = accountsCollection.Aggregate()
                        .Match(matchBalanceStage)
                        .Group(u => u.AccountHolder, g => new { AccountHolder = g.Key, Balance = g.Sum(u => u.Balance), AmountOfAccounts = g.Count() })
                        .SortByDescending(u => u.Balance);

var aggregate2 = accountsCollection.Aggregate()
                        .Match(matchBalanceStage)
                        .Sort("{account_type:1,balance:-1}")
                        .Project(projectStage);



var results = aggregate2.ToList();

foreach (var account in results)
{
    Console.WriteLine(account);
}