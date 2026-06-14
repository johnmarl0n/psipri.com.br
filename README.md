# Psipri - Landing Page Profissional e Gestão Pingo de Mel

![Versão .NET](https://img.shields.io/badge/.NET-8.0-blue.svg)
![Licença](https://img.shields.io/badge/Licença-No%20License-red.svg)

Uma plataforma web robusta e elegante desenvolvida em **ASP.NET Core 8 MVC**, projetada para profissionais de psicologia (clínica e jurídica) e integrada a um sistema de gestão sob medida para a fabricação artesanal de velas da marca **Pingo de Mel**.

## 🌟 Funcionalidades

### 1. Landing Page Pública (Redesenhada)
- **Design Premium & Acolhedor:** Estética elegante com paleta de cores curada em tons de verde oliva (`#2D3E2F`) e dourado fosco (`#A68B5C`), com sombras suaves e micro-animações interativas.
- **Seletor Interativo ("Como posso te ajudar hoje?"):** Painel dinâmico em JavaScript que permite ao paciente clicar no sintoma/necessidade atual (Ansiedade, Relacionamento, Perícias, etc.) e visualizar uma mensagem empática com um botão de agendamento rápido via WhatsApp contendo texto personalizado.
- **Diferenciais da Profissional:** Seção visual em cards destacando "Escuta Acolhedora", "Embasamento Científico", "Sigilo Absoluto" e "Rigor Jurídico".
- **FAQ Dinâmico:** Acordeão contendo as principais dúvidas sobre consultas online, reembolso de planos de saúde e perícias técnicas.
- **SEO Local & Estruturação:** Títulos `<h1>` otimizados para termos locais ("Psicóloga Clínica e Jurídica em Campinas"), sitemap.xml, robots.txt e dados estruturados em JSON-LD (`ProfessionalService`) para Google Maps.

### 2. Painel de Manutenção Administrativa
- **Gestão de Conteúdo:** Edição dinâmica da biografia "Sobre" com suporte a Quill.js (imagens, vídeos e formatação rica) e alteração direta da foto principal (Hero) do site.
- **Gestão do Blog:** CRUD completo de artigos e publicações técnicas com redimensionamento de layouts responsivos.

### 3. Sistema Pingo de Mel (CRM, Estoque & Vendas)
- **Controle de Insumos (Estoque):** Cadastro de produtos por unidades de medida adequadas (KG para cera, ML para essências/corantes, UN para pavios/potes, MT para fitas).
- **Fórmulas de Receitas:** Composição detalhada de velas. O sistema calcula automaticamente o custo real do produto e sugere um preço de venda com base em uma margem de markup configurável (margem de custo operacional).
- **Lançamento de Produção:** Produção em lote que realiza a baixa automática no estoque dos insumos individuais de acordo com a receita.
- **CRM & Vendas Mensais:** Histórico de clientes, registro de vendas mensais com cálculo automático e controle de faturamento.
- **Personalizador e Impressão de Etiquetas:** Painel integrado para personalizar e imprimir etiquetas das velas diretamente da receita. Suporta layouts de A4 (6 ou 12 etiquetas por página) e rolo de bobina térmica individual (80x80mm) com estilos de impressão limpos (`@media print`).
- **Fechamento Mensal Automatizado:** Um serviço em segundo plano (`BackgroundService` no ASP.NET) configurado para rodar às 23:45 do último dia de cada mês, tirando um "print" da posição de estoque e valor total de ativos para auditoria financeira histórica.

## 🛠️ Tecnologias Utilizadas

- **Core:** ASP.NET Core 8.0 MVC
- **Banco de Dados:** Microsoft SQL Server (MSSQL)
- **Identidade:** Microsoft.AspNetCore.Identity (login administrativo em PT-BR)
- **E-mail:** MailKit (Integração SMTP com disparo seguro)
- **Trabalho em Segundo Plano:** HostedServices nativos do .NET
- **Frontend:** Vanilla JS, CSS3, FontAwesome 6, Quill.js
- **Otimização:** BuildBundlerMinifier para minificação automática de CSS (`site.min.css`).

## ⚙️ Configurações e Instalação

### Pré-requisitos
- .NET 8.0 SDK
- Servidor SQL Server configurado

### Configuração
1. Configure a string de conexão ("DefaultConnection") e credenciais de SMTP no arquivo `appsettings.json`.
2. O banco de dados executará todas as migrações estruturais e seedings de forma automatizada ao iniciar o aplicativo.
3. Para rodar localmente:
   ```bash
   dotnet build
   dotnet run --urls "http://localhost:5058"
   ```

## 📦 Deploy em Produção

Para gerar o pacote otimizado e pronto para publicação no servidor (ex: IIS / SmarterASP.NET):
```bash
dotnet publish -c Release
```

---
Desenvolvido por **John Dias** (johnmarl0n) para Dra. Priscila Batista.
