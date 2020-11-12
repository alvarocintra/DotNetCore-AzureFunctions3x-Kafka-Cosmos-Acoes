using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;
using FunctionAppAcoes.Models;
using FunctionAppAcoes.Validators;
using FunctionAppAcoes.Data;

namespace FunctionAppAcoes
{
    public static class KafkaAcoesTopicTrigger2
    {
        [FunctionName("KafkaAcoesTopicTrigger2")]
        public static void Run([KafkaTrigger(
            "KafkaConnection", "topic-acoes-2",
            ConsumerGroup = "topic-acoes-docdb")]KafkaEventData<string> kafkaEvent,
            ILogger log)
        {
            string dados = kafkaEvent.Value.ToString();
            log.LogInformation($"KafkaAcoesTopicTrigger 2 - Dados: {dados}");

            var acao = JsonSerializer.Deserialize<Acao>(dados,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            var validationResult = new AcaoValidator().Validate(acao);

            if (validationResult.IsValid)
            {
                log.LogInformation($"KafkaAcoesTopicTrigger - Dados p�s formata��o: {JsonSerializer.Serialize(acao)}");
                //AcoesRepository.Save(acao);
                log.LogInformation("KafkaAcoesTopicTrigger - A��o registrada com sucesso!");
                log.LogInformation("KafkaAcoesTopicTrigger - �ltima a��o executada.");
            }
            else
            {
                log.LogInformation("KafkaAcoesTopicTrigger - Dados inv�lidos para a A��o");
            }
        }
    }
}