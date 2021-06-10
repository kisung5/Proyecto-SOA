using System.Linq;
using MongoDB.Driver;
using System.Collections.Generic;
using API_service.Models;

namespace API_service.Services
{
    public class DocumentService
    {
        private readonly IMongoCollection<Document> _documents;

        public DocumentService(IAnalyzerDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documents = database.GetCollection<Document>(settings.CollectionName);
        }

        public List<Document> Get() =>
            _documents.Find(document => true).ToList();

        public Document GetById(string id) =>
            _documents.Find<Document>(document => document.Id == id).FirstOrDefault();

        public Document GetByName(string name) =>
            _documents.Find<Document>(document => document.Name == name).FirstOrDefault();

        public Document Create(Document document)
        {
            _documents.InsertOne(document);
            return document;
        }

        public void Update(string id, Document documentIn) =>
            _documents.ReplaceOne(document => document.Id == id, documentIn);

        public void Remove(Document documentIn) =>
            _documents.DeleteOne(document => document.Id == documentIn.Id);

        public void Remove(string id) =>
            _documents.DeleteOne(document => document.Id == id);
    }
}
