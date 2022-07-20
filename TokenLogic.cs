using Common.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Linq;
using UmbracoMongoDbClient;

namespace TokenManager
{
	public class TokenCollectionItem
	{
		[BsonId]
		public string Id { get; set; }
		public TokenValue Token { get; set; }
	}
	public static class TokenLogic
	{
		private static readonly string _databaseName = "TokensDB";
		private static readonly string _collectionName = "Tokens";

		public static void Consume(TokenIdentifier id)
		{
			var database = MongoDBClientConnection.GetDatabase(_databaseName);
			var collection = database.GetCollection<TokenCollectionItem>(_collectionName);
			collection.DeleteOne(a => a.Id == id.ToString());
		}

		public static bool ConsumeAndValidate(TokenIdentifier id, string token)
		{
			var isValid = Validate(id, token);
			Consume(id);
			return isValid;
		}

		public static string Generate(TokenIdentifier id, int validityInSeconds, int NumberOfDigits = 0)
		{
			var database = MongoDBClientConnection.GetDatabase(_databaseName);
			var collection = database.GetCollection<TokenCollectionItem>(_collectionName);
			string oneTimeToken;
			var token = collection.Find(avm => avm.Id == id.ToString())?.FirstOrDefault();
			if (token != null)
			{
				oneTimeToken = token.Token.OneTimeToken;
				collection.DeleteOne(a => a.Id == id.ToString());
			}
			else
			{
				oneTimeToken = (NumberOfDigits > 0) ? CryptoUtils.GetRandomNumber(NumberOfDigits) : Guid.NewGuid().ToString().ToLowerInvariant();
			}
			collection.InsertOne(new TokenCollectionItem() { Id = id.ToString(), Token = new TokenValue(oneTimeToken, validityInSeconds) });
			return oneTimeToken;
		}

		public static bool Validate(TokenIdentifier id, string token)
		{
			if (string.IsNullOrWhiteSpace(token))
			{
				return false;
			}
			var database = MongoDBClientConnection.GetDatabase(_databaseName);
			var collection = database.GetCollection<TokenCollectionItem>(_collectionName);

			var tokenInDb = collection.Find(avm => avm.Id == id.ToString())?.FirstOrDefault();

			if (tokenInDb != null)
			{
				return tokenInDb.Token.Valid(token);
			}
			return false;
		}
	}
}
