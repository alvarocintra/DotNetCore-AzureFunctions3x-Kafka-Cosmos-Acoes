using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FunctionAppAcoes.Data;

namespace FunctionAppAcoes
{
    public static class Acoes
    {
        [FunctionName("Acoes")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Function Acoes - HTTP GET");
            return new OkObjectResult(AcoesRepository.GetAll());
        }
    }
}