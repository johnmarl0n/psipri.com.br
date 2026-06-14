using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using psipri.com.br.Data;
using psipri.com.br.Models.PDM;
using psipri.com.br.Models.PDM.Enums;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace psipri.com.br.Controllers
{
    [Authorize]
    [Route("Admin/PDM")]
    public class PDMController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PDMController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- UTILITY: Get Margem Operacional ---
        private async Task<decimal> GetMargemOperacionalAsync()
        {
            var config = await _context.PDMConfiguracoes.FirstOrDefaultAsync(c => c.Chave == "MargemOperacional");
            if (config != null && decimal.TryParse(config.Valor, out decimal margem))
            {
                return margem;
            }
            return 15.00m; // Default fallback
        }

        // ==========================================================================
        // 1. DASHBOARD & INDEX
        // ==========================================================================
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Pingo de Mel";
            ViewData["ActivePage"] = "Dashboard";

            // Counts
            ViewBag.TotalProdutos = await _context.PDMProdutos.CountAsync(p => p.Ativo);
            ViewBag.TotalReceitas = await _context.PDMReceitas.CountAsync(r => r.Ativa);
            ViewBag.TotalClientes = await _context.PDMClientes.CountAsync();
            ViewBag.TotalVendas = await _context.PDMVendas.CountAsync();

            // Total billing & costs
            ViewBag.FaturamentoTotal = await _context.PDMVendas.SumAsync(v => (decimal?)v.ValorFinal) ?? 0;
            ViewBag.TotalGasto = await _context.PDMProducoes.SumAsync(p => (decimal?)p.CustoTotalProducao) ?? 0;

            // Top recipes sold
            var topVelas = await _context.PDMVendaItems
                .Include(vi => vi.Receita)
                .GroupBy(vi => vi.ReceitaId)
                .Select(g => new {
                    Nome = g.First().Receita != null ? g.First().Receita.Nome : "Excluída",
                    Quantidade = g.Sum(vi => vi.Quantidade)
                })
                .OrderByDescending(x => x.Quantidade)
                .Take(5)
                .ToListAsync();
            
            ViewBag.TopVelasJson = System.Text.Json.JsonSerializer.Serialize(topVelas);

            // Monthly breakdown (last 6 months)
            var hoje = DateTime.Now;
            var meses = Enumerable.Range(0, 6)
                .Select(i => hoje.AddMonths(-i))
                .OrderBy(d => d)
                .ToList();

            var vendasMensais = new List<decimal>();
            var producaoMensal = new List<decimal>();
            var rotulosMeses = new List<string>();

            foreach (var mes in meses)
            {
                var inicioMes = new DateTime(mes.Year, mes.Month, 1);
                var fimMes = inicioMes.AddMonths(1).AddTicks(-1);

                decimal faturamento = await _context.PDMVendas
                    .Where(v => v.DataVenda >= inicioMes && v.DataVenda <= fimMes)
                    .SumAsync(v => (decimal?)v.ValorFinal) ?? 0;

                decimal gasto = await _context.PDMProducoes
                    .Where(p => p.DataProducao >= inicioMes && p.DataProducao <= fimMes)
                    .SumAsync(p => (decimal?)p.CustoTotalProducao) ?? 0;

                vendasMensais.Add(faturamento);
                producaoMensal.Add(gasto);
                rotulosMeses.Add(mes.ToString("MMMM/yyyy", new System.Globalization.CultureInfo("pt-BR")));
            }

            ViewBag.VendasMensaisJson = System.Text.Json.JsonSerializer.Serialize(vendasMensais);
            ViewBag.ProducaoMensalJson = System.Text.Json.JsonSerializer.Serialize(producaoMensal);
            ViewBag.RotulosMesesJson = System.Text.Json.JsonSerializer.Serialize(rotulosMeses);

            // Alertas de estoque mínimo
            var alertasEstoque = await _context.PDMProdutos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo && p.QuantidadeEstoque <= p.EstoqueMinimo)
                .OrderBy(p => p.QuantidadeEstoque)
                .ToListAsync();
            
            return View(alertasEstoque);
        }

        // ==========================================================================
        // 2. PRODUTOS (ESTOQUE INSUMOS)
        // ==========================================================================
        [HttpGet("Produtos")]
        public async Task<IActionResult> Produtos(int? categoriaId)
        {
            ViewData["Title"] = "Estoque de Insumos";
            ViewData["ActivePage"] = "Produtos";

            var query = _context.PDMProdutos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo);

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                query = query.Where(p => p.CategoriaId == categoriaId.Value);
            }

            var produtos = await query.OrderBy(p => p.Nome).ToListAsync();
            ViewBag.Categorias = await _context.PDMCategorias.OrderBy(c => c.Nome).ToListAsync();
            ViewBag.SelectedCategoriaId = categoriaId;

            return View(produtos);
        }

        [HttpGet("Produto/Criar")]
        public async Task<IActionResult> ProdutoCriar()
        {
            ViewData["Title"] = "Cadastrar Insumo";
            ViewData["ActivePage"] = "Produtos";

            ViewBag.Categorias = await _context.PDMCategorias.OrderBy(c => c.Nome).ToListAsync();
            return View(new PDMProduto());
        }

        [HttpPost("Produto/Criar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProdutoCriar(PDMProduto produto, IFormFile? imagemFile)
        {
            if (ModelState.IsValid)
            {
                if (imagemFile != null && imagemFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await imagemFile.CopyToAsync(ms);
                        produto.ImagemThumb = ms.ToArray();
                        produto.ImagemMimeType = imagemFile.ContentType;
                    }
                }

                produto.CriadoEm = DateTime.Now;
                produto.AtualizadoEm = DateTime.Now;
                produto.Ativo = true;

                _context.Add(produto);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Produto cadastrado com sucesso!";
                return RedirectToAction(nameof(Produtos));
            }

            ViewBag.Categorias = await _context.PDMCategorias.OrderBy(c => c.Nome).ToListAsync();
            return View(produto);
        }

        [HttpGet("Produto/Editar/{id}")]
        public async Task<IActionResult> ProdutoEditar(int id)
        {
            ViewData["Title"] = "Editar Insumo";
            ViewData["ActivePage"] = "Produtos";

            var produto = await _context.PDMProdutos.FindAsync(id);
            if (produto == null || !produto.Ativo)
            {
                return NotFound();
            }

            ViewBag.Categorias = await _context.PDMCategorias.OrderBy(c => c.Nome).ToListAsync();
            return View(produto);
        }

        [HttpPost("Produto/Editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProdutoEditar(int id, PDMProduto model, IFormFile? imagemFile, bool removerImagem)
        {
            var produto = await _context.PDMProdutos.FindAsync(id);
            if (produto == null || !produto.Ativo)
            {
                return NotFound();
            }

            // Check if price changed to record in history
            decimal precoAnterior = produto.PrecoCusto;

            if (ModelState.IsValid)
            {
                produto.Nome = model.Nome;
                produto.Descricao = model.Descricao;
                produto.CategoriaId = model.CategoriaId;
                produto.UnidadeMedida = model.UnidadeMedida;
                produto.QuantidadeEstoque = model.QuantidadeEstoque;
                produto.EstoqueMinimo = model.EstoqueMinimo;
                produto.PrecoCusto = model.PrecoCusto;
                produto.Margem = model.Margem;
                produto.AtualizadoEm = DateTime.Now;

                if (removerImagem)
                {
                    produto.ImagemThumb = null;
                    produto.ImagemMimeType = null;
                }
                else if (imagemFile != null && imagemFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await imagemFile.CopyToAsync(ms);
                        produto.ImagemThumb = ms.ToArray();
                        produto.ImagemMimeType = imagemFile.ContentType;
                    }
                }

                // If cost price changed, record history entry
                if (precoAnterior != model.PrecoCusto)
                {
                    var historico = new PDMHistoricoPreco
                    {
                        ProdutoId = produto.Id,
                        PrecoAnterior = precoAnterior,
                        PrecoNovo = model.PrecoCusto,
                        DataAlteracao = DateTime.Now
                    };
                    _context.Add(historico);
                }

                _context.Update(produto);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Produto atualizado com sucesso!";
                return RedirectToAction(nameof(Produtos));
            }

            ViewBag.Categorias = await _context.PDMCategorias.OrderBy(c => c.Nome).ToListAsync();
            return View(model);
        }

        [HttpPost("Produto/Excluir/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProdutoExcluir(int id)
        {
            var produto = await _context.PDMProdutos.FindAsync(id);
            if (produto != null)
            {
                // Soft delete
                produto.Ativo = false;
                produto.AtualizadoEm = DateTime.Now;
                _context.Update(produto);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Produto excluído com sucesso!";
            }
            else
            {
                TempData["Error"] = "Produto não encontrado.";
            }

            return RedirectToAction(nameof(Produtos));
        }

        // Utility to serve image bytes
        [HttpGet("Produto/Imagem/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterImagemProduto(int id)
        {
            var produto = await _context.PDMProdutos.FindAsync(id);
            if (produto != null && produto.ImagemThumb != null && produto.ImagemMimeType != null)
            {
                return File(produto.ImagemThumb, produto.ImagemMimeType);
            }
            return NotFound();
        }

        // ==========================================================================
        // 3. RECEITAS (FICHAS TÉCNICAS E COMPOSIÇÃO)
        // ==========================================================================
        [HttpGet("Receitas")]
        public async Task<IActionResult> Receitas()
        {
            ViewData["Title"] = "Receitas de Velas";
            ViewData["ActivePage"] = "Receitas";

            var receitas = await _context.PDMReceitas
                .Include(r => r.ReceitaItems)
                    .ThenInclude(ri => ri.Produto)
                .Where(r => r.Ativa)
                .OrderBy(r => r.Nome)
                .ToListAsync();

            ViewBag.MargemOperacional = await GetMargemOperacionalAsync();
            return View(receitas);
        }

        [HttpGet("Receita/Detalhe/{id}")]
        public async Task<IActionResult> ReceitaDetalhe(int id)
        {
            ViewData["Title"] = "Ficha Técnica";
            ViewData["ActivePage"] = "Receitas";

            var receita = await _context.PDMReceitas
                .Include(r => r.ReceitaItems)
                    .ThenInclude(ri => ri.Produto)
                        .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(r => r.Id == id && r.Ativa);

            if (receita == null)
            {
                return NotFound();
            }

            ViewBag.MargemOperacional = await GetMargemOperacionalAsync();
            return View(receita);
        }

        [HttpGet("Receita/Etiquetas/{id}")]
        public async Task<IActionResult> ReceitaEtiquetas(int id)
        {
            ViewData["Title"] = "Etiquetas de Velas";
            ViewData["ActivePage"] = "Receitas";

            var receita = await _context.PDMReceitas
                .Include(r => r.ReceitaItems)
                    .ThenInclude(ri => ri.Produto)
                .FirstOrDefaultAsync(r => r.Id == id && r.Ativa);

            if (receita == null)
            {
                return NotFound();
            }

            return View(receita);
        }

        [HttpGet("Receita/Criar")]
        public async Task<IActionResult> ReceitaCriar()
        {
            ViewData["Title"] = "Criar Receita";
            ViewData["ActivePage"] = "Receitas";

            ViewBag.Produtos = await _context.PDMProdutos.Where(p => p.Ativo).OrderBy(p => p.Nome).ToListAsync();
            return View(new PDMReceita());
        }

        [HttpPost("Receita/Criar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceitaCriar(PDMReceita receita, string[] produtoIds, string[] quantidades, IFormFile? imagemFile)
        {
            if (ModelState.IsValid)
            {
                if (imagemFile != null && imagemFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await imagemFile.CopyToAsync(ms);
                        receita.ImagemThumb = ms.ToArray();
                        receita.ImagemMimeType = imagemFile.ContentType;
                    }
                }

                receita.CriadoEm = DateTime.Now;
                receita.Ativa = true;

                _context.Add(receita);
                await _context.SaveChangesAsync(); // Save recipe to get Id

                // Save items
                if (produtoIds != null && quantidades != null)
                {
                    for (int i = 0; i < produtoIds.Length; i++)
                    {
                        if (int.TryParse(produtoIds[i], out int prodId) && decimal.TryParse(quantidades[i].Replace(",", "."), out decimal qty))
                        {
                            var item = new PDMReceitaItem
                            {
                                ReceitaId = receita.Id,
                                ProdutoId = prodId,
                                Quantidade = qty
                            };
                            _context.Add(item);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Receita criada com sucesso!";
                return RedirectToAction(nameof(Receitas));
            }

            ViewBag.Produtos = await _context.PDMProdutos.Where(p => p.Ativo).OrderBy(p => p.Nome).ToListAsync();
            return View(receita);
        }

        [HttpGet("Receita/Editar/{id}")]
        public async Task<IActionResult> ReceitaEditar(int id)
        {
            ViewData["Title"] = "Editar Receita";
            ViewData["ActivePage"] = "Receitas";

            var receita = await _context.PDMReceitas
                .Include(r => r.ReceitaItems)
                .FirstOrDefaultAsync(r => r.Id == id && r.Ativa);

            if (receita == null)
            {
                return NotFound();
            }

            ViewBag.Produtos = await _context.PDMProdutos.Where(p => p.Ativo).OrderBy(p => p.Nome).ToListAsync();
            return View(receita);
        }

        [HttpPost("Receita/Editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceitaEditar(int id, PDMReceita model, string[] produtoIds, string[] quantidades, IFormFile? imagemFile, bool removerImagem)
        {
            var receita = await _context.PDMReceitas
                .Include(r => r.ReceitaItems)
                .FirstOrDefaultAsync(r => r.Id == id && r.Ativa);

            if (receita == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                receita.Nome = model.Nome;
                receita.Descricao = model.Descricao;
                receita.Instrucoes = model.Instrucoes;
                receita.MargemLucro = model.MargemLucro;
                receita.RendimentoUnidades = model.RendimentoUnidades;

                if (removerImagem)
                {
                    receita.ImagemThumb = null;
                    receita.ImagemMimeType = null;
                }
                else if (imagemFile != null && imagemFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await imagemFile.CopyToAsync(ms);
                        receita.ImagemThumb = ms.ToArray();
                        receita.ImagemMimeType = imagemFile.ContentType;
                    }
                }

                // Update items: clear current ones and add new ones
                _context.PDMReceitaItems.RemoveRange(receita.ReceitaItems);

                if (produtoIds != null && quantidades != null)
                {
                    for (int i = 0; i < produtoIds.Length; i++)
                    {
                        if (int.TryParse(produtoIds[i], out int prodId) && decimal.TryParse(quantidades[i].Replace(",", "."), out decimal qty))
                        {
                            var item = new PDMReceitaItem
                            {
                                ReceitaId = receita.Id,
                                ProdutoId = prodId,
                                Quantidade = qty
                            };
                            _context.Add(item);
                        }
                    }
                }

                _context.Update(receita);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Receita atualizada com sucesso!";
                return RedirectToAction(nameof(Receitas));
            }

            ViewBag.Produtos = await _context.PDMProdutos.Where(p => p.Ativo).OrderBy(p => p.Nome).ToListAsync();
            return View(model);
        }

        [HttpPost("Receita/Excluir/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceitaExcluir(int id)
        {
            var receita = await _context.PDMReceitas.FindAsync(id);
            if (receita != null)
            {
                // Soft delete
                receita.Ativa = false;
                _context.Update(receita);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Receita excluída com sucesso!";
            }
            else
            {
                TempData["Error"] = "Receita não encontrada.";
            }

            return RedirectToAction(nameof(Receitas));
        }

        [HttpGet("Receita/Imagem/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterImagemReceita(int id)
        {
            var receita = await _context.PDMReceitas.FindAsync(id);
            if (receita != null && receita.ImagemThumb != null && receita.ImagemMimeType != null)
            {
                return File(receita.ImagemThumb, receita.ImagemMimeType);
            }
            return NotFound();
        }

        // ==========================================================================
        // 4. PRODUÇÕES (HISTÓRICO E BAIXA DE ESTOQUE)
        // ==========================================================================
        [HttpGet("Producoes")]
        public async Task<IActionResult> Producoes()
        {
            ViewData["Title"] = "Histórico de Produção";
            ViewData["ActivePage"] = "Producoes";

            var producoes = await _context.PDMProducoes
                .Include(p => p.Receita)
                .OrderByDescending(p => p.DataProducao)
                .ToListAsync();

            return View(producoes);
        }

        [HttpGet("Producao/Registrar")]
        public async Task<IActionResult> ProducaoRegistrar(int? receitaId)
        {
            ViewData["Title"] = "Registrar Produção";
            ViewData["ActivePage"] = "Producoes";

            ViewBag.Receitas = await _context.PDMReceitas.Where(r => r.Ativa).OrderBy(r => r.Nome).ToListAsync();
            ViewBag.SelectedReceitaId = receitaId;

            return View();
        }

        [HttpPost("Producao/Registrar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProducaoRegistrar(int receitaId, int quantidadeLotes)
        {
            var receita = await _context.PDMReceitas
                .Include(r => r.ReceitaItems)
                    .ThenInclude(ri => ri.Produto)
                .FirstOrDefaultAsync(r => r.Id == receitaId && r.Ativa);

            if (receita == null)
            {
                TempData["Error"] = "Receita não encontrada.";
                return RedirectToAction(nameof(Producoes));
            }

            if (quantidadeLotes <= 0)
            {
                TempData["Error"] = "A quantidade de lotes deve ser maior que zero.";
                return RedirectToAction(nameof(ProducaoRegistrar), new { receitaId });
            }

            // Check stock availability
            foreach (var item in receita.ReceitaItems)
            {
                var produto = item.Produto;
                if (produto == null) continue;

                decimal qtyNeeded = item.Quantidade * quantidadeLotes;
                if (produto.QuantidadeEstoque < qtyNeeded)
                {
                    TempData["Error"] = $"Estoque insuficiente para o insumo: {produto.Nome}. Necessário: {qtyNeeded.ToString("N3")}, em estoque: {produto.QuantidadeEstoque.ToString("N3")}.";
                    return RedirectToAction(nameof(ProducaoRegistrar), new { receitaId });
                }
            }

            // Deduct stock and calculate total cost
            decimal totalCusto = 0;
            foreach (var item in receita.ReceitaItems)
            {
                var produto = item.Produto;
                if (produto == null) continue;

                decimal qtyNeeded = item.Quantidade * quantidadeLotes;
                produto.QuantidadeEstoque -= qtyNeeded;
                produto.AtualizadoEm = DateTime.Now;
                _context.Update(produto);

                totalCusto += qtyNeeded * produto.PrecoCusto;
            }

            // Create production entry
            var producao = new PDMProducao
            {
                ReceitaId = receitaId,
                QuantidadeProduzida = quantidadeLotes,
                CustoTotalProducao = totalCusto,
                DataProducao = DateTime.Now
            };

            _context.Add(producao);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Produção de {quantidadeLotes} lote(s) da receita '{receita.Nome}' registrada com sucesso!";
            return RedirectToAction(nameof(Producoes));
        }

        // ==========================================================================
        // 5. CLIENTES (CRM E FIDELIZAÇÃO)
        // ==========================================================================
        [HttpGet("Clientes")]
        public async Task<IActionResult> Clientes()
        {
            ViewData["Title"] = "Clientes";
            ViewData["ActivePage"] = "Clientes";

            var clientes = await _context.PDMClientes
                .OrderBy(c => c.Nome)
                .ToListAsync();

            return View(clientes);
        }

        [HttpGet("Cliente/Criar")]
        public IActionResult ClienteCriar()
        {
            ViewData["Title"] = "Cadastrar Cliente";
            ViewData["ActivePage"] = "Clientes";
            return View(new PDMCliente());
        }

        [HttpPost("Cliente/Criar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClienteCriar(PDMCliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.CriadoEm = DateTime.Now;
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente cadastrado com sucesso!";
                return RedirectToAction(nameof(Clientes));
            }
            return View(cliente);
        }

        [HttpGet("Cliente/Editar/{id}")]
        public async Task<IActionResult> ClienteEditar(int id)
        {
            ViewData["Title"] = "Editar Cliente";
            ViewData["ActivePage"] = "Clientes";

            var cliente = await _context.PDMClientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost("Cliente/Editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClienteEditar(int id, PDMCliente model)
        {
            var cliente = await _context.PDMClientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                cliente.Nome = model.Nome;
                cliente.Email = model.Email;
                cliente.Telefone = model.Telefone;
                cliente.Observacoes = model.Observacoes;

                _context.Update(cliente);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente atualizado com sucesso!";
                return RedirectToAction(nameof(Clientes));
            }
            return View(model);
        }

        [HttpPost("Cliente/Excluir/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClienteExcluir(int id)
        {
            var cliente = await _context.PDMClientes.FindAsync(id);
            if (cliente != null)
            {
                _context.PDMClientes.Remove(cliente);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente removido com sucesso!";
            }
            return RedirectToAction(nameof(Clientes));
        }

        // ==========================================================================
        // 6. VENDAS E FATURAMENTO
        // ==========================================================================
        [HttpGet("Vendas")]
        public async Task<IActionResult> Vendas()
        {
            ViewData["Title"] = "Registro de Vendas";
            ViewData["ActivePage"] = "Vendas";

            var vendas = await _context.PDMVendas
                .Include(v => v.Cliente)
                .Include(v => v.VendaItems)
                    .ThenInclude(vi => vi.Receita)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();

            return View(vendas);
        }

        [HttpGet("Venda/Registrar")]
        public async Task<IActionResult> VendaRegistrar()
        {
            ViewData["Title"] = "Registrar Nova Venda";
            ViewData["ActivePage"] = "Vendas";

            ViewBag.Clientes = await _context.PDMClientes.OrderBy(c => c.Nome).ToListAsync();
            ViewBag.Receitas = await _context.PDMReceitas.Where(r => r.Ativa).OrderBy(r => r.Nome).ToListAsync();
            return View();
        }

        [HttpPost("Venda/Registrar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VendaRegistrar(int? clienteId, decimal desconto, string[] receitaIds, string[] quantidades, string[] precosUnitarios)
        {
            if (receitaIds == null || receitaIds.Length == 0)
            {
                TempData["Error"] = "A venda deve conter pelo menos um item.";
                ViewBag.Clientes = await _context.PDMClientes.OrderBy(c => c.Nome).ToListAsync();
                ViewBag.Receitas = await _context.PDMReceitas.Where(r => r.Ativa).OrderBy(r => r.Nome).ToListAsync();
                return View();
            }

            decimal totalBruto = 0;
            var itemsList = new System.Collections.Generic.List<PDMVendaItem>();

            for (int i = 0; i < receitaIds.Length; i++)
            {
                if (int.TryParse(receitaIds[i], out int recId) &&
                    int.TryParse(quantidades[i], out int qty) &&
                    decimal.TryParse(precosUnitarios[i].Replace(",", "."), out decimal unitPrice))
                {
                    decimal subtotal = qty * unitPrice;
                    totalBruto += subtotal;

                    itemsList.Add(new PDMVendaItem
                    {
                        ReceitaId = recId,
                        Quantidade = qty,
                        PrecoUnitario = unitPrice,
                        Subtotal = subtotal
                    });
                }
            }

            var venda = new PDMVenda
            {
                ClienteId = clienteId > 0 ? clienteId : null,
                DataVenda = DateTime.Now,
                ValorTotal = totalBruto,
                Desconto = desconto,
                ValorFinal = totalBruto - desconto,
                VendaItems = itemsList
            };

            _context.Add(venda);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Venda registrada com sucesso!";
            return RedirectToAction(nameof(Vendas));
        }

        [HttpPost("Venda/Excluir/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VendaExcluir(int id)
        {
            var venda = await _context.PDMVendas
                .Include(v => v.VendaItems)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda != null)
            {
                _context.PDMVendaItems.RemoveRange(venda.VendaItems);
                _context.PDMVendas.Remove(venda);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Venda excluída com sucesso!";
            }
            return RedirectToAction(nameof(Vendas));
        }

        // ==========================================================================
        // 7. FECHAMENTO DE ESTOQUE (HISTÓRICO DE POSIÇÃO)
        // ==========================================================================
        [HttpGet("Fechamentos")]
        public async Task<IActionResult> Fechamentos()
        {
            ViewData["Title"] = "Fechamento de Estoque";
            ViewData["ActivePage"] = "Fechamentos";

            var fechamentos = await _context.PDMFechamentos
                .OrderByDescending(f => f.DataFechamento)
                .ToListAsync();

            return View(fechamentos);
        }

        [HttpGet("Fechamento/Detalhe/{id}")]
        public async Task<IActionResult> FechamentoDetalhe(int id)
        {
            ViewData["Title"] = "Detalhes do Fechamento";
            ViewData["ActivePage"] = "Fechamentos";

            var fechamento = await _context.PDMFechamentos
                .Include(f => f.FechamentoItems)
                    .ThenInclude(fi => fi.Produto)
                        .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fechamento == null)
            {
                return NotFound();
            }

            return View(fechamento);
        }

        [HttpPost("Fechamento/Criar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FazerFechamento()
        {
            var produtos = await _context.PDMProdutos
                .Where(p => p.Ativo)
                .ToListAsync();

            if (!produtos.Any())
            {
                TempData["Error"] = "Não há insumos cadastrados para realizar o fechamento.";
                return RedirectToAction(nameof(Fechamentos));
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

            _context.Add(fechamento);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Fechamento de estoque realizado com sucesso! Posição atual de {produtos.Count} insumos gravada.";
            return RedirectToAction(nameof(Fechamentos));
        }

        // ==========================================================================
        // 8. CONFIGURAÇÕES
        // ==========================================================================
        [HttpGet("Configuracoes")]
        public async Task<IActionResult> Configuracoes()
        {
            ViewData["Title"] = "Configurações";
            ViewData["ActivePage"] = "Configuracoes";

            var margemConfig = await _context.PDMConfiguracoes.FirstOrDefaultAsync(c => c.Chave == "MargemOperacional");
            ViewBag.MargemOperacional = margemConfig?.Valor ?? "15.00";

            return View();
        }

        [HttpPost("Configuracoes/Salvar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarConfiguracoes(string margemOperacional)
        {
            if (decimal.TryParse(margemOperacional.Replace(",", "."), out decimal val) && val >= 0)
            {
                var config = await _context.PDMConfiguracoes.FirstOrDefaultAsync(c => c.Chave == "MargemOperacional");
                if (config == null)
                {
                    config = new PDMConfiguracao
                    {
                        Chave = "MargemOperacional",
                        Valor = val.ToString("F2")
                    };
                    _context.Add(config);
                }
                else
                {
                    config.Valor = val.ToString("F2");
                    _context.Update(config);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Configurações salvas com sucesso!";
            }
            else
            {
                TempData["Error"] = "Margem Operacional inválida.";
            }

            return RedirectToAction(nameof(Configuracoes));
        }
    }
}
