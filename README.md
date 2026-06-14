# Psipri - Landing Page Profissional e Gestão Pingo de Mel

![Versão .NET](https://img.shields.io/badge/.NET-8.0-blue.svg)
![Licença](https://img.shields.io/badge/Licença-No%20License-red.svg)

Uma plataforma web robusta e elegante desenvolvida em **ASP.NET Core 8 MVC**, projetada para profissionais de psicologia (clínica e jurídica) e integrada a um sistema de gestão sob medida para a fabricação artesanal de velas da marca **Pingo de Mel**.

---

## 🌟 Funcionalidades

### 1. Landing Page Pública

- **Design Premium & Acolhedor:** Estética elegante com paleta de cores curada em tons de verde oliva (`#2D3E2F`) e dourado fosco (`#A68B5C`), com sombras suaves e micro-animações interativas.
- **Navegação Mobile Responsiva:** Menu hambúrguer com efeito *glassmorphism* (backdrop-filter blur), animação de slide-down e fechamento automático ao clicar em links âncora ou fora da área do menu.
- **Seletor Interativo ("Como posso te ajudar hoje?"):** Painel dinâmico em JavaScript que permite ao paciente clicar no sintoma/necessidade atual (Ansiedade, Relacionamento, Perícias, etc.) e visualizar uma mensagem empática com um botão de agendamento rápido via WhatsApp contendo texto personalizado. Layout agora responsivo em coluna única no mobile.
- **Diferenciais da Profissional:** Seção visual em cards destacando "Escuta Acolhedora", "Embasamento Científico", "Sigilo Absoluto" e "Rigor Jurídico".
- **FAQ Dinâmico:** Acordeão com as principais dúvidas sobre consultas, reembolso de planos de saúde e perícias técnicas.

### 2. SEO & Descoberta

- **Títulos Otimizados para SEO Local:** A página inicial exibe `"Psicóloga em Campinas - Dra. Priscila Batista | Psicoterapia & Perícias"`. Demais páginas renderizam `"[Título da Página] - Dra. Priscila Batista"`, dentro do limite de 70 caracteres recomendado pelo Google.
- **Sitemap XML Dinâmico (`/sitemap.xml`):** Gerado automaticamente pelo `HomeController` em tempo de execução, incluindo a página inicial e todos os artigos publicados do blog com suas respectivas datas de publicação. O arquivo estático foi removido em favor desta abordagem.
- **Dados Estruturados JSON-LD:** Dois blocos de Schema.org implementados diretamente na página inicial:
  - `ProfessionalService`: localização, telefone, horário de atendimento e redes sociais.
  - `FAQPage`: respostas às perguntas frequentes para exibição de *rich snippets* no Google.
- **robots.txt:** Configurado para permitir rastreamento de todas as páginas públicas e bloquear as áreas `/Admin/` e `/Account/`.
- **Meta Description & Canonical:** Tags otimizadas em todas as páginas.

### 3. Painel de Manutenção Administrativa

- **Gestão de Conteúdo:** Edição dinâmica da biografia "Sobre" com suporte a Quill.js (imagens, vídeos e formatação rica) e alteração direta da foto principal (Hero) do site.
- **Gestão do Blog:** CRUD completo de artigos e publicações técnicas com redimensionamento de layouts responsivos.

### 4. Sistema Pingo de Mel (CRM, Estoque & Vendas) — Mobile First

- **Interface 100% Responsiva (Mobile First):** Todo o painel PDM foi otimizado para uso prioritário em smartphones e tablets, com 3 breakpoints:
  - `≤ 1024px` (tablet): sidebar em gaveta deslizante, KPI em 2 colunas, padding reduzido.
  - `≤ 768px` (mobile): padding compacto, gráficos empilham em coluna única, formulários em coluna única, botões touch-friendly (`min-height: 44px`), nome da usuária oculto no header.
  - `≤ 480px` (S20 Ultra e similares): KPI em 1 coluna, tabelas ultracompactas.
- **Sidebar com Overlay:** Em mobile/tablet, a sidebar abre com um fundo escuro semi-transparente (overlay), bloqueia o scroll do `<body>` e fecha automaticamente ao clicar em qualquer link de navegação ou no overlay.
- **Controle de Insumos (Estoque):** Cadastro de produtos por unidades de medida adequadas (KG para cera, ML para essências/corantes, UN para pavios/potes, MT para fitas).
- **Fórmulas de Receitas:** Composição detalhada de velas. O sistema calcula automaticamente o custo real do produto e sugere um preço de venda com base em uma margem de markup configurável.
- **Lançamento de Produção:** Produção em lote que realiza a baixa automática no estoque dos insumos de acordo com a receita.
- **CRM & Vendas Mensais:** Histórico de clientes, registro de vendas mensais com cálculo automático e controle de faturamento.
- **Personalizador e Impressão de Etiquetas:** Painel integrado para personalizar e imprimir etiquetas das velas diretamente da receita. Suporta layouts de A4 (6 ou 12 por página) e rolo de bobina térmica individual (80x80mm) com estilos de impressão limpos (`@media print`).
- **Fechamento Mensal Automatizado:** Um `BackgroundService` configurado para rodar às 23:45 do último dia de cada mês, registrando a posição de estoque e valor total de ativos para auditoria histórica.

---

## 🛠️ Tecnologias Utilizadas

| Camada | Tecnologia |
|--------|-----------|
| Core | ASP.NET Core 8.0 MVC |
| Banco de Dados | Microsoft SQL Server (MSSQL) |
| Identidade | Microsoft.AspNetCore.Identity |
| E-mail | MailKit (integração SMTP) |
| Background Jobs | HostedService nativo do .NET |
| Frontend | Vanilla JS, CSS3, FontAwesome 6, Quill.js, Chart.js |
| Build/Otimização | BuildBundlerMinifier (minificação automática de CSS e JS) |

---

## ⚙️ Configuração e Instalação

### Pré-requisitos
- .NET 8.0 SDK
- Microsoft SQL Server

### Passos
1. Configure a string de conexão (`DefaultConnection`) e as credenciais de SMTP no arquivo `appsettings.json`.
2. O banco de dados executa todas as migrações estruturais e seedings automaticamente ao iniciar o aplicativo.
3. Para rodar localmente:
   ```bash
   dotnet build
   dotnet run --launch-profile http
   ```

---

## 📦 Deploy em Produção

Para gerar o pacote otimizado para publicação (IIS / SmarterASP.NET):
```bash
dotnet publish -c Release
```

---

## 📋 Histórico de Alterações Recentes

| Data | Alteração |
|------|-----------|
| Jun/2026 | Responsividade completa do painel PDM para mobile/tablet (3 breakpoints, overlay sidebar, scroll-lock) |
| Jun/2026 | Correção do bug HTTP 405 na exclusão de produtos (atributo `href` → `action` no `<form>`) |
| Jun/2026 | SEO: título otimizado para busca local, `FAQPage` JSON-LD, sitemap.xml dinâmico via controller |
| Jun/2026 | Correção do overflow horizontal na seção "Como posso te ajudar hoje?" no mobile |
| Jun/2026 | Implementação do menu de navegação mobile com glassmorphism e animações |

---

Desenvolvido por **John Dias** (johnmarl0n) para Dra. Priscila Batista.

## 📄 Licença

Este projeto é proprietário e não possui licença aberta de uso ou distribuição ("No License"). Todo o código e recursos associados são de uso exclusivo para a cliente designada, não sendo permitida a cópia, modificação, distribuição ou reutilização do mesmo. Para mais detalhes, consulte o arquivo [LICENSE](LICENSE).
