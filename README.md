# NetTools AV

Aplicativo Windows para observação e diagnóstico de protocolos em redes audiovisuais. **Protocolos são a raiz do produto**; IP/MAC são atributos de evidência, sem cadastro ou diagnóstico por equipamento.

Baseline: **02/10/2026 · 0.2.0-preview.2 · homologação 1.0 pendente**.

## Começar

**[Baixar instalador e versão portátil](https://github.com/ViniRibasS/nettools-av/releases/tag/v0.2.0-preview.2)** · [Documentação completa](docs/README.md) · [Guia de uso do GitHub](docs/operacao/github.md)

Menu Iniciar → NetTools AV. Instalador local: `artifacts/installers/NetToolsAV-0.2.0-preview.2-win-x64-setup.exe`. Versão portátil: `artifacts/releases/0.2.0-preview.2`; mantenha os arquivos juntos. Runtime .NET incluído; Npcap separado para captura ao vivo. Importação PCAPNG funciona sem driver. Sem atualização automática ou assinatura digital de produção.

Leia o [manual de uso](docs/operacao/manual-do-usuario.md) e o [índice completo](docs/README.md).

## Entrega atual

- Sessões SQLite e seleção de protocolos.
- Captura passiva Ethernet em processo separado, filtro BPF, limites e segmentos PCAPNG.
- Importação PCAPNG, inspeção básica de dez identificadores de protocolo, histórico e evidências.
- Relatórios HTML/TXT/JSON, exportação ZIP, temas e logo RJ45/arroba.
- Instalador por usuário com preservação de dados na desinstalação.

Presença não comprova conformidade. RTP/RTCP, QDP e outros módulos permanecem sem analisador: [matriz vigente](docs/engenharia/14-matriz-0.2-e-homologacao.md).

## Engenharia

| Assunto | Documento |
|---|---|
| Visão técnica | [Resumo](docs/engenharia/resumo-tecnico.md) |
| Componentes e processos | [Arquitetura](docs/engenharia/arquitetura-atual.md) |
| Diagramas funcionais e técnicos | [Diagramas Mermaid](docs/diagramas/README.md) |
| Requisitos e backlog | [Rastreabilidade](docs/engenharia/requisitos-rastreabilidade.md) |
| Ambiente e build | [Desenvolvimento](docs/engenharia/desenvolvimento.md) |
| Testes e 1.0 | [Qualidade](docs/qualidade/plano-de-validacao.md) |
| Release | [Empacotamento e entrega](docs/engenharia/release.md) |
| Agentes de IA | [AGENTS.md](AGENTS.md) e [guia](docs/ia/guia-de-trabalho.md) |

## Verificar

Na raiz, em PowerShell, com SDK compatível com `global.json`:

```powershell
./scripts/build.ps1 -Target Check
./scripts/package.ps1
./scripts/verify-worker.ps1 -Executable 'artifacts/releases/0.2.0-preview.2/NetToolsAV.exe'
./scripts/installer.ps1 -SkipPackage
```

O último comando exige `.tools/inno/ISCC.exe`. Não execute `verify-installer.ps1` no perfil que já usa a instalação. [Evidências](docs/qualidade/evidencias.md): 39 verificações aprovadas, ciclo do instalador testado e quatro arquivos de campo conferidos por hash/reanálise; não é homologação integral.

`prototype/` é histórico; a aplicação entregue está em `src/NetToolsAV.Desktop`. O [escopo original](Escopo_Implementacao_Monitor_AV_v0.1.md) é subordinado à [revisão por protocolos](docs/engenharia/11-escopo-por-protocolos.md). Esta revisão documental não modifica os documentos incorporados ao instalador já gerado.
