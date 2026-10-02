# Capturas — referência operacional 0.2.0-preview.2

Esta entrega não é a versão 1.0. A publicação 1.0 depende da homologação completa solicitada pelo responsável pelo produto.

## Abrir

Abra pelo menu Iniciar após instalar, ou execute `NetToolsAV.exe` na pasta portátil mantendo os arquivos juntos. Runtime .NET incluído; SDK desnecessário. Há instalador por usuário; atualização automática e assinatura digital continuam ausentes. A prévia atual está em `artifacts/releases/0.2.0-preview.2`. Consulte o [manual](manual-do-usuario.md).

Sessões existentes são preservadas em `%LOCALAPPDATA%/NetToolsAV`. Para testes isolados, execute `NetToolsAV.exe --data-dir "CAMINHO"`. O banco de sessões continua no esquema 1. Capturas ficam em `captures/<sessão>/<captura>` com PCAPNG, `summary.json` e `sha256.json`.

## Captura passiva Ethernet

1. Crie ou selecione uma sessão. Escolha explicitamente a interface Ethernet; não há troca automática para Wi-Fi.
2. Clique em **Capturar**. Confira interface, filtro BPF opcional, duração e limite de armazenamento. O padrão é 60 segundos, 256 MiB e modo não promíscuo.
3. Inicie. O host Npcap separado grava segmentos PCAPNG de até 32 MiB e publica resumos aproximadamente a cada segundo. **Parar** encerra a coleta. O fechamento da janela pede o encerramento do host.
4. Consulte **Capturas** para histórico e últimas 80 evidências. Cada evidência tem número do pacote, protocolo e atributos técnicos de origem/destino.
5. Exporte **Relatório** em HTML, TXT ou JSON. Em **Capturas**, use **Exportar evidências ZIP** para copiar PCAPNG, resumo e hashes sem alterar os originais. O destino deve ser um arquivo novo fora da pasta de captura. Os PCAPNG permanecem na pasta indicada pelo histórico, sem conversão ou remoção automática.

Npcap deve estar instalado e permitir acesso ao usuário. O pacote não inclui o driver, não muda sua configuração e não eleva privilégios automaticamente. Falha de acesso aparece como falha da captura; não é diagnóstico de protocolo. O projeto usa a API documentada do Npcap diretamente, sem SharpPcap/PacketDotNet neste incremento.

O aplicativo não altera Wi-Fi/configurações de rede. Há evidências locais de duas capturas diretas verificadas, sem homologação integral; veja [evidências](../qualidade/evidencias.md).

## Importar sem conectar cabo

Clique em **Importar**, selecione um PCAPNG e aguarde. A aplicação copia o original para a pasta da captura e analisa a cópia. O arquivo original é preservado. A tabela incorpora os protocolos observados, mesmo quando não estavam previamente selecionados. O nome de uma sessão não comprova a origem dos dados: confira Source e arquivos de cada captura.

Perfil de importação: SHB/IDB/EPB, endian por seção, múltiplas seções/interfaces, `if_tsresol` e `if_tsoffset`. Limite de 512 MiB por arquivo, 4 MiB por bloco e 65.535 bytes capturados por pacote. PB/SPB são recusados explicitamente. Link types diferentes de Ethernet são contados sem interpretação. Blocos auxiliares não interpretados são contabilizados. O resumo não substitui todos os metadados do original. Arquivo inválido/copiação incompleta não recebe resultado de análise concluída.

## Como ler os resultados

- **Não executado:** nenhuma captura/importação analisada.
- **Observado:** cabeçalho reconhecido no ponto/janela analisados. Não significa saúde ou conformidade.
- **Não observado:** nenhuma mensagem reconhecida nessa janela; considere filtro, ponto de captura e capacidade do parser.
- **Sem analisador:** protocolo do catálogo ainda sem interpretação implementada.
- **Inconclusivo:** falha/interrupção impede uma conclusão completa.

Contagens de UDP/DNS/mDNS e outras camadas podem se sobrepor: não somar contagens por protocolo como total de pacotes. Estrutura inválida/incompleta pode decorrer de truncamento. Checksums, remontagem TCP/IP, RTT, perdas RTP, jitter, precisão PTP e conformidade Dante/AES67 não são medidos nesta prévia. Contadores Npcap ausentes ficam indisponíveis, nunca zero presumido.

## Reproduzir verificações

```powershell
./scripts/build.ps1 -Target Check
./scripts/build.ps1 -Target Publish -OutputDirectory artifacts/releases/0.2.0-preview.2
./scripts/verify-worker.ps1 -Executable artifacts/releases/0.2.0-preview.2/NetToolsAV.exe
./scripts/package.ps1
```

`verify-worker.ps1` testa somente falhas antes da abertura de interface. Duas capturas diretas foram verificadas por arquivos salvos; parada sob carga e encerramento abrupto do host continuam pendentes. Não executar testes de carga em produção.


