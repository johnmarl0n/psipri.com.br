using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using psipri.com.br.Data;
using psipri.com.br.Models.PDM;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace psipri.com.br.Services
{
    public class PDMStockClosingJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PDMStockClosingJob> _logger;

        public PDMStockClosingJob(IServiceProvider serviceProvider, ILogger<PDMStockClosingJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Pingo de Mel Stock Closing Job started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    // Calculate if it's the last day of the month and near the end of the day
                    var lastDayOfMonth = DateTime.DaysInMonth(now.Year, now.Month);
                    
                    if (now.Day == lastDayOfMonth && now.Hour == 23 && now.Minute >= 45)
                    {
                        await PerformClosingAsync();
                        // Wait 2 hours to get past midnight and avoid double-runs
                        await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in PDMStockClosingJob background thread.");
                }

                // Check every 10 minutes
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private async Task PerformClosingAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Check if closing already exists for today to avoid duplicates
                var today = DateTime.Today;
                var alreadyClosed = await context.PDMFechamentos
                    .AnyAsync(f => f.DataFechamento.Date == today);

                if (alreadyClosed)
                {
                    return;
                }

                var produtos = await context.PDMProdutos
                    .Where(p => p.Ativo)
                    .ToListAsync();

                if (!produtos.Any())
                {
                    _logger.LogWarning("No active PDM products found for auto stock closing.");
                    return;
                }

                var itemsList = new System.Collections.Generic.List<PDMFechamentoItem>();
                decimal valorTotal = 0;

                foreach (var prod in produtos)
                {
                    decimal subtotal = prod.QuantidadeEstoque * prod.PrecoCusto;
                    valorTotal += subtotal;

                    itemsList.Add(new PDMFechamentoItem
                    {
                        ProdutoId = prod.Id,
                        QuantidadeEstoque = prod.QuantidadeEstoque,
                        PrecoCusto = prod.PrecoCusto,
                        ValorTotal = subtotal
                    });
                }

                var fechamento = new PDMFechamento
                {
                    DataFechamento = DateTime.Now,
                    ValorTotalEstoque = valorTotal,
                    FechamentoItems = itemsList
                };

                context.Add(fechamento);
                await context.SaveChangesAsync();
                _logger.LogInformation("PDM monthly stock closing auto-saved successfully. Items: {Count}, Total Value: {Val}", produtos.Count, valorTotal);
            }
        }
    }
}
