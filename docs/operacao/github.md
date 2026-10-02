# Repositório e distribuição no GitHub

Repositório público criado em 02/10/2026: [ViniRibasS/nettools-av](https://github.com/ViniRibasS/nettools-av). Branch principal `main`, remoto `origin` por HTTPS. Primeiro commit: `5cbe872`. Autor local usa endereço noreply do GitHub; a configuração global do Git foi preservada.

Distribuição: [pré-release 0.2.0-preview.2](https://github.com/ViniRibasS/nettools-av/releases/tag/v0.2.0-preview.2). A tag aponta para a importação inicial, não para um commit original de compilação do binário.

## Estrutura

| Local | Conteúdo |
|---|---|
| src/ | Aplicação, domínio e infraestrutura |
| tests/ | Verificações e geradores de corpus sintético |
| scripts/ | Build, empacotamento, instalador e verificações |
| docs/ | Engenharia, manuais, diagramas, qualidade e instruções de IA |
| assets/ | Marca e avisos de dependências |
| prototype/ | Protótipo histórico com dados fictícios |
| Raiz | README, AGENTS, changelog, contribuição, segurança e configuração .NET |
| GitHub Releases | Instalador, ZIP portátil, respectivos hashes e notas de entrega |

`.tools`, `artifacts`, bin/obj, capturas reais, banco, logs, chaves e arquivos de ambiente não entram no histórico Git. Instalador/ZIP são publicados como assets da release, sem versionar binários gerados na árvore de código. Não anexar toda a pasta artifacts.

## Autenticação e rotina

Git identifica commits com nome/e-mail; isso não autentica no GitHub. GitHub CLI usa autenticação separada. Prefira login pelo navegador, nunca envie senha/token pelo chat ou escreva-os em scripts.

```powershell
gh auth login --hostname github.com --git-protocol https --web
gh auth status
git status
git pull --ff-only
# Edite, valide e revise os arquivos.
git add CAMINHOS-REVISADOS
git diff --cached
git commit -m "Descreva a alteração"
git push
```

Se gh não estiver no PATH, a ferramenta preparada neste workspace está em `.tools/github-cli/bin/gh.exe`. Git instalado no Windows está em `C:\Program Files\Git\cmd\git.exe`; um terminal já aberto pode precisar ser reaberto para atualizar PATH.

## Releases

A primeira distribuição permanece `0.2.0-preview.2`, marcada pré-release, não 1.0. A tag de importação inicial registra o estado do repositório, incluindo documentação posterior ao build do instalador. O binário já existente foi produzido antes do primeiro commit, conforme SOURCE-MANIFEST e BUILD-MANIFEST. Não afirmar que a tag é o commit original de compilação nem modificar o artefato para ocultar isso.

Para próximas entregas, seguir [processo de release](../engenharia/release.md), gerar versão inédita, registrar revisão verdadeira e testar. Repositório público não define automaticamente uma licença de reutilização; nenhuma licença do produto foi escolhida nesta publicação. Licenças das dependências permanecem em assets/licenses.

Referências: [autenticação GitHub CLI](https://cli.github.com/manual/gh_auth_login) e [distribuição por Releases](https://docs.github.com/en/repositories/releasing-projects-on-github/about-releases).
