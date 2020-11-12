using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;
using FunctionAppAcoes.Models;
using FunctionAppAcoes.Validators;
using FunctionAppAcoes.Data;

namespace FunctionAppAcoes
{
    public static class KafkaAcoesTopicTrigger
    {
        [FunctionName("KafkaAcoesTopicTrigger")]
        public static void Run([KafkaTrigger(
            "KafkaConnection", "topic-acoes",
            ConsumerGroup = "topic-acoes-docdb")]KafkaEventData<string> kafkaEvent,
            ILogger log)
        {
            string dados = kafkaEvent.Value.ToString();
            log.LogInformation($"KafkaAcoesTopicTrigger 1 - Dados: {dados}");
            
            var acao = JsonSerializer.Deserialize<Acao>(dados,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            var validationResult = new AcaoValidator().Validate(acao);
            
            if (validationResult.IsValid)
            {
                log.LogInformation($"KafkaAcoesTopicTrigger - Dados pós formatação: {JsonSerializer.Serialize(acao)}");

                var eventTest = new KafkaEventData<string>(JsonSerializer.Serialize(acao));

                try
                {
                    AcoesRepository.Save(acao);
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                log.LogInformation("KafkaAcoesTopicTrigger - Ação registrada com sucesso!");

                log.LogInformation("KafkaAcoesTopicTrigger - Executando próxima ação...");
                KafkaAcoesTopicTrigger2.Run(eventTest, log);


            }
            else
            {
                log.LogInformation("KafkaAcoesTopicTrigger - Dados inválidos para a Ação");
            }
        }
    }
}