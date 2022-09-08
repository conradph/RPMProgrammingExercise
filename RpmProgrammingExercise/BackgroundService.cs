using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RpmProgrammingExercise.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using nmpsc = System.Configuration;

namespace RpmProgrammingExercise
{
    internal class BackgroundService : IHostedService
    {
        int delay = Int32.Parse(nmpsc.ConfigurationManager.AppSettings["taskExecutionDelay"]);
        int dayCount = Int32.Parse(nmpsc.ConfigurationManager.AppSettings["dayCount"]);
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                GasPriceDAL data = new GasPriceDAL();
                List<GasPrice> prices = data.GetGasPricesFromAPI(dayCount);
                for (int i = 0; i < prices.Count; i++)
                {
                    data.InsertGasPrice(prices[i]);
                }
                await Task.Delay(delay);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}