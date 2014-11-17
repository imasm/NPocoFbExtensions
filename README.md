# NPoco Firebird extensions

### Update Or Insert

```csharp
var poco = new UserDecorated();
poco.UserId = 1;
poco.Name = "John Doe";;
poco.Age = 56;
poco.Savings = decimal)345.23;
poco.DateOfBirth = DateTime.Now;

IDatabase db = new Database("connStringName");
db.UpdateOrInsert(poco);
```
