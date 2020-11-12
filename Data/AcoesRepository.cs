using System;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using FunctionAppAcoes.Models;

namespace FunctionAppAcoes.Data
{
    public static class AcoesRepository
    {
        private const string DB_ACOES = "DBAcoes";
        private const string COLLECTION_ACOES = "Acoes";

        static AcoesRepository()
        {
            using var client = GetDocumentClient();

            client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = DB_ACOES }).Wait();

            DocumentCollection collectionInfo = new DocumentCollection();
            collectionInfo.Id = COLLECTION_ACOES;

            collectionInfo.IndexingPolicy =
                new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

            client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DB_ACOES),
                collectionInfo,
                new RequestOptions { OfferThroughput = 400 }).Wait();
        }

        private static DocumentClient GetDocumentClient()
        {
            return new DocumentClient(
                new Uri(Environment.GetEnvironmentVariable("DBAcoesEndpointUri")),
                Environment.GetEnvironmentVariable("DBAcoesEndpointPrimaryKey"));
        }

        public static void Save(Acao acao)
        {
            var horario = DateTime.Now;
            var document = new AcaoDocument();
            document.id = acao.Codigo + horario.ToString("yyyyMMdd-HHmmss");
            document.Codigo = acao.Codigo;
            document.Valor = acao.Valor.Value;
            document.Data = horario.ToString("yyyy-MM-dd HH:mm:ss");

            using var client = GetDocumentClient();
            client.CreateDocumentAsync(
               UriFactory.CreateDocumentCollectionUri(
                   DB_ACOES, COLLECTION_ACOES), document).Wait();
        }

        public static object GetAll()
        {
            using var client = GetDocumentClient();
            FeedOptions queryOptions =
                new FeedOptions { MaxItemCount = -1 };
            return client.CreateDocumentQuery(
                UriFactory.CreateDocumentCollectionUri(
                    DB_ACOES, COLLECTION_ACOES),
                    "SELECT A.id, A.Codigo, A.Valor, A.Data FROM Acoes A " +
                    "ORDER BY A.id", queryOptions)
                .ToList();
        }
    }
}