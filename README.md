# Psipri - Landing Page Profissional para Psicologia

![Versão .NET](https://img.shields.io/badge/.NET-8.0-blue.svg)
![Licença](https://img.shields.io/badge/Licença-No%20License-red.svg)

Uma plataforma web robusta e elegante desenvolvida em **ASP.NET Core 8 MVC**, projetada especificamente para profissionais de psicologia, com foco especial em **Psicologia Jurídica e Clínica**.

## 🌟 Funcionalidades

### Landing Page Pública
- **Design Premium:** Estética inspirada em referências modernas de alta qualidade, com paleta de cores orgânica e tipografia sofisticada.
- **SEO Otimizado:** Meta tags estruturadas para ranqueamento local (ex: Campinas e região).
- **Atuação Clínica:** Seção interativa em estilo acordeão para detalhamento de especialidades.
- **Atendimento Online:** Seção dedicada explicando as vantagens e o funcionamento da psicoterapia remota.
- **Blog Integrado:** Sistema de posts e "playbooks" para compartilhamento de conhecimento técnico.

### Área Administrativa (Painel de Manutenção)
- **Segurança Robusta:** Autenticação via ASP.NET Core Identity com proteção CSRF refinada e suporte a ambientes sem SSL (URLs temporárias).
- **Gestão de Conteúdo:** Edição dinâmica da seção "Sobre" com suporte a cores, imagens, vídeos do YouTube e alinhamento via Quill.js.
- **Gestão de Blog:** CRUD completo de postagens com suporte a conteúdo rico e multimídia.
- **Visualização Responsiva:** Filtros de CSS customizados para garantir que imagens e vídeos inseridos sejam 100% responsivos no site público.
- **Login Customizado:** Interface de autenticação totalmente traduzida (PT-BR), limpa e com alternador de visibilidade de senha (eye-toggle) integrado ao campo.

## 🛠️ Tecnologias Utilizadas

- **Core:** ASP.NET Core 8.0 MVC
- **Banco de Dados:** Microsoft SQL Server (MSSQL) (otimizado para melhor desempenho e baixíssimo consumo de RAM na hospedagem)
- **Identidade:** Microsoft.AspNetCore.Identity (customizado para login único e segurança reforçada)
- **E-mail:** MailKit (SMTP Integration)
- **Frontend:** Vanilla JS, CSS3, FontAwesome 6, Quill.js
- **Otimização:** BuildBundlerMinifier para empacotamento e minificação automática de ativos.

## ⚙️ Melhorias de Estabilização e Produção

Recentemente, o projeto passou por uma rodada de estabilização para garantir o funcionamento perfeito em servidores como **SmarterASP.NET**:
- **Migração de Banco de Dados:** Banco totalmente migrado de SQLite para SQL Server, resolvendo problemas de gargalo de memória RAM e reciclagem de pool de aplicativos no ambiente IIS do SmarterASP.NET.
- **Migrações Automáticas:** O sistema agora aplica migrações de banco de dados automaticamente ao iniciar no servidor, dispensando comandos manuais de terminal.
- **Seeding de Segurança:** Garantia programática de que apenas o usuário administrador ("Priscila") exista, com remoção automática de contas não autorizadas e reset de senha via código.
- **Políticas de Cookies:** Ajustadas para permitir o login em URLs temporárias via HTTP, mantendo a segurança via `SameAsRequest`.

## 🚀 Como Iniciar o Projeto

### Pré-requisitos
- .NET 8.0 SDK
- Visual Studio 2022 ou VS Code

### Configuração Local
1. Clone o repositório.
2. Configure suas credenciais de e-mail no arquivo `appsettings.json`.
3. Aplique as migrations iniciais:
   ```bash
   dotnet ef database update
   ```
4. Execute o projeto:
   ```bash
   dotnet run
   ```

## 📦 Implantação (Deployment)

O projeto está configurado para publicação em ambiente Windows/Linux. Para gerar os arquivos de produção:
```bash
dotnet publish -c Release
```

## 📜 Licença

Este projeto é de uso exclusivo e não possui uma licença aberta (**No License**). Todos os direitos reservados ao autor.

---
Desenvolvido por **John Dias** (johnmarl0n) para Dra. Priscila Batista.
