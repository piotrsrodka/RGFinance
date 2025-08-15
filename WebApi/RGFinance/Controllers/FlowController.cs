using Database.Entities;
using Microsoft.AspNetCore.Mvc;
using RGFinance.FlowFeature;

namespace RGFinance.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FlowController : ControllerBase
    {       
        private readonly ILogger<FlowController> _logger;
        private readonly IFlowService flowService;

        public FlowController(ILogger<FlowController> logger, IFlowService flowService)
        {
            _logger = logger;
            this.flowService = flowService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FlowFeature.Flow>> Get(int id)
        {
            var flow = await this.flowService.GetFlowAsync(id);
            return flow;
        }

        //https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml

        [HttpGet("forex")]
        public async Task<Forex> GetForex()
        {
            return await this.flowService.GetForex();
        }


        [HttpPost("asset")]
        public async Task<ActionResult> AddOrUpdate(Asset asset)
        {
            await this.flowService.AddOrUpdateAsset(asset);
            return Ok();
        }

        [HttpPost("profit")]
        public async Task<ActionResult> AddOrUpdateProfit(Profit profit)
        {
            await this.flowService.AddOrUpdateProfit(profit);
            return Ok();
        }

        [HttpDelete("asset/{id}")]
        public async Task<ActionResult> DeleteAsset(int id)
        {
            await this.flowService.DeleteAsset(id);
            return Ok();
        }

        [HttpDelete("profit/{id}")]
        public async Task<ActionResult> DeleteProfit(int id)
        {
            await this.flowService.DeleteProfit(id);
            return Ok();
        }

        [HttpDelete("expense/{id}")]
        public async Task<ActionResult> DeleteExpense(int id)
        {
            await this.flowService.DeleteExpense(id);
            return Ok();
        }

        [HttpPost("expense")]
        public async Task<ActionResult> AddOrUpdateExpense(Expense expense)
        {
            await this.flowService.AddOrUpdateExpense(expense);
            return Ok();
        }
    }
}
