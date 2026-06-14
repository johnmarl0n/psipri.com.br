using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace psipri.com.br.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPDMTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PDMCategorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMCategorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PDMClientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMClientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PDMConfiguracoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Chave = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMConfiguracoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PDMFechamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataFechamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Periodo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValorTotalEstoque = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMFechamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PDMReceitas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Instrucoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MargemLucro = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RendimentoUnidades = table.Column<int>(type: "int", nullable: false),
                    ImagemThumb = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImagemMimeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMReceitas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PDMProdutos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UnidadeMedida = table.Column<int>(type: "int", nullable: false),
                    QuantidadeEstoque = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EstoqueMinimo = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PrecoCusto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Margem = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ImagemThumb = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImagemMimeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMProdutos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PDMProdutos_PDMCategorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "PDMCategorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PDMVendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    DataVenda = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Desconto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorFinal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FormaPagamento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMVendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PDMVendas_PDMClientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "PDMClientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PDMProducoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceitaId = table.Column<int>(type: "int", nullable: false),
                    QuantidadeProduzida = table.Column<int>(type: "int", nullable: false),
                    CustoTotalProducao = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataProducao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMProducoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PDMProducoes_PDMReceitas_ReceitaId",
                        column: x => x.ReceitaId,
                        principalTable: "PDMReceitas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PDMFechamentoItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechamentoId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    QuantidadeEstoque = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PrecoCusto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMFechamentoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PDMFechamentoItems_PDMFechamentos_FechamentoId",
                        column: x => x.FechamentoId,
                        principalTable: "PDMFechamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PDMFechamentoItems_PDMProdutos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "PDMProdutos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PDMHistoricoPrecos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    PrecoAnterior = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecoNovo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMHistoricoPrecos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PDMHistoricoPrecos_PDMProdutos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "PDMProdutos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PDMReceitaItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceitaId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMReceitaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PDMReceitaItems_PDMProdutos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "PDMProdutos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PDMReceitaItems_PDMReceitas_ReceitaId",
                        column: x => x.ReceitaId,
                        principalTable: "PDMReceitas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PDMVendaItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendaId = table.Column<int>(type: "int", nullable: false),
                    ReceitaId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDMVendaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PDMVendaItems_PDMReceitas_ReceitaId",
                        column: x => x.ReceitaId,
                        principalTable: "PDMReceitas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PDMVendaItems_PDMVendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "PDMVendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PDMCategorias",
                columns: new[] { "Id", "Icone", "Nome" },
                values: new object[,]
                {
                    { 1, "fa-fire", "Ceras e Parafinas" },
                    { 2, "fa-flask", "Essências e Cheiros" },
                    { 3, "fa-grip-lines-vertical", "Pavios e Suportes" },
                    { 4, "fa-palette", "Corantes e Pigmentos" },
                    { 5, "fa-box-open", "Embalagens e Potes" },
                    { 6, "fa-ribbon", "Fitas e Enfeites" },
                    { 7, "fa-ellipsis-h", "Outros Insumos" }
                });

            migrationBuilder.InsertData(
                table: "PDMConfiguracoes",
                columns: new[] { "Id", "Chave", "Valor" },
                values: new object[] { 1, "MargemOperacional", "15.00" });

            migrationBuilder.CreateIndex(
                name: "IX_PDMFechamentoItems_FechamentoId",
                table: "PDMFechamentoItems",
                column: "FechamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_PDMFechamentoItems_ProdutoId",
                table: "PDMFechamentoItems",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_PDMHistoricoPrecos_ProdutoId",
                table: "PDMHistoricoPrecos",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_PDMProducoes_ReceitaId",
                table: "PDMProducoes",
                column: "ReceitaId");

            migrationBuilder.CreateIndex(
                name: "IX_PDMProdutos_CategoriaId",
                table: "PDMProdutos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_PDMReceitaItems_ProdutoId",
                table: "PDMReceitaItems",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_PDMReceitaItems_ReceitaId",
                table: "PDMReceitaItems",
                column: "ReceitaId");

            migrationBuilder.CreateIndex(
                name: "IX_PDMVendaItems_ReceitaId",
                table: "PDMVendaItems",
                column: "ReceitaId");

            migrationBuilder.CreateIndex(
                name: "IX_PDMVendaItems_VendaId",
                table: "PDMVendaItems",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_PDMVendas_ClienteId",
                table: "PDMVendas",
                column: "ClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PDMConfiguracoes");

            migrationBuilder.DropTable(
                name: "PDMFechamentoItems");

            migrationBuilder.DropTable(
                name: "PDMHistoricoPrecos");

            migrationBuilder.DropTable(
                name: "PDMProducoes");

            migrationBuilder.DropTable(
                name: "PDMReceitaItems");

            migrationBuilder.DropTable(
                name: "PDMVendaItems");

            migrationBuilder.DropTable(
                name: "PDMFechamentos");

            migrationBuilder.DropTable(
                name: "PDMProdutos");

            migrationBuilder.DropTable(
                name: "PDMReceitas");

            migrationBuilder.DropTable(
                name: "PDMVendas");

            migrationBuilder.DropTable(
                name: "PDMCategorias");

            migrationBuilder.DropTable(
                name: "PDMClientes");
        }
    }
}
