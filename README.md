# UmbracoMongoDbTokenManager

Sample code
```csharp
string generatedToken = TokenLogic.Generate(id : "someId", validityInSeconds: 60, NumberOfDigits: 6); // if you don't pass the NumberOfDigits, you would get a random Guid as token
bool isValid = TokenLogic.ConsumeAndValidate(id: "someId", token: generatedToken); 
// You could also validate without consuming using
TokenLogic.Validate(id: "someId", token: generatedToken);
// or just consume it to get rid of it from the database
TokenLogic.Consume(id: "someId");
```