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
- **Segurança Robusta:** Autenticação via ASP.NET Core Identity com proteção CSRF integrada.
- **Gestão de Conteúdo:** Edição dinâmica da seção "Sobre" com suporte a cores, imagens, vídeos do YouTube e alinhamento via Quill.js.
- **Gestão de Blog:** CRUD completo de postagens com suporte a conteúdo rico e multimídia.
- **Visualização Responsiva:** Filtros de CSS customizados para garantir que imagens e vídeos inseridos sejam 100% responsivos no site público.
- **Layout Profissional:** Dashboard clean e intuitivo para facilitar a manutenção diária.

## 🛠️ Tecnologias Utilizadas

- **Core:** ASP.NET Core 8.0 MVC
- **Banco de Dados:** SQLite (com Entity Framework Core)
- **Identidade:** Microsoft.AspNetCore.Identity
- **E-mail:** MailKit (SMTP Integration)
- **Frontend:** Vanilla JS, CSS3, FontAwesome 6, Quill.js
- **Otimização:** BuildBundlerMinifier para empacotamento e minificação automática de ativos.

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
