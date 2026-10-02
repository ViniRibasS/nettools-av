# Suporte, backup e recuperação

Procedimentos para 0.2.0-preview.2. Backup/restauração abaixo são procedimentos manuais recomendados; não existe assistente de recuperação nem homologação integral de desastre.

## Diagnóstico por sintoma

| Sintoma | Conferência | Próximo passo |
|---|---|---|
| Npcap não encontrado | Driver separado e acesso permitido | Acionar TI; não copiar wpcap.dll de origem arbitrária |
| Interface não encontrada | GUID selecionado e lista atualizada | Selecionar novamente a interface correta; não usar outra por conveniência |
| Filtro inválido | Sintaxe BPF, não filtro de exibição Wireshark | Corrigir expressão; teste vazio apenas se escopo permitir coletar tudo visível |
| Zero pacotes | Estado, duração, filtro, link e ponto | Registrar contexto; não concluir que o protocolo não funciona |
| Sem analisador | Matriz da versão | Não é erro de driver; funcionalidade ainda pendente |
| Importação recusada | Formato, tamanho, tipo de bloco e mensagem | Preservar original; registrar defeito/perfil não suportado |
| Captura interrompida | Último resumo sem confirmação terminal | Preservar arquivos; analisar offline; não alterar Status manualmente |
| Banco de versão mais nova | Versão instalada e schema | Usar versão compatível; nunca reduzir user_version manualmente |
| Falha ao exportar ZIP | Destino existente ou dentro da captura | Escolher nome novo fora da origem; conferir espaço/permissões |
| Instalação bloqueada | Janela e host de captura abertos | Finalizar normalmente e tentar novamente |
| Erro de gravação/disco cheio | Espaço no volume de dados, permissões | Parar coleta, preservar evidência e liberar espaço fora da sessão ativa |

Falha do driver/captura não é falha de protocolo. Não apague o banco ou reinstale Npcap como primeira tentativa sem diagnóstico.

## Backup consistente manual

1. Pare a captura e feche todas as instâncias do app/worker. Evite copiar o banco enquanto há gravação.
2. Copie a **raiz inteira de dados** `%LOCALAPPDATA%\NetToolsAV` para destino protegido e datado. Se usou `--data-dir`, copie essa raiz.
3. Preserve banco, capturas, resumos e hashes juntos. Inclua arquivos auxiliares SQLite que existirem; não pressupor que banco sozinho basta.
4. Registre versão do app, horário/fuso, quantidade de arquivos e hashes do backup. Confira leitura de uma cópia; não use o backup mestre para ensaios.

Não há criptografia própria do aplicativo. Proteção do destino e acesso são responsabilidade do ambiente. Defina retenção com o responsável pelo produto; não há prazo automático implementado.

## Restauração sem sobrescrever a única cópia

1. Com o aplicativo fechado, preserve o diretório atual, mesmo se parecer corrompido.
2. Restaure o backup em **novo diretório**. Use versão compatível com o esquema.
3. Abra a cópia com `NetToolsAV.exe --data-dir "C:\CAMINHO\COPIA-DE-TESTE"` em PowerShell, usando o caminho real do executável.
4. Confira sessões, associação dos resumos, importação e leitura de PCAPNG; compare hashes. Não execute nova captura na única cópia do backup.
5. Só depois escolha o diretório operacional. Registre o resultado; não declare recuperação homologada sem prova completa.

Não existe rollback automático de banco nem migração genérica. Reinstalar binários não recupera dados corrompidos. Não abrir banco futuro com executável antigo após forçar mudança de versão.

## Coleta para suporte

Use o [modelo de defeito](../modelos/defeito.md). Anexe versão exata, Windows/arquitetura, ação, mensagem completa, horário/fuso, resultado esperado e observado. Para captura: GUID/interface, filtro, limites, estado terminal, versão do driver realmente instalada, descartes e hashes. Não confundir versão da DLL libpcap com versão do driver Npcap.

Envie somente evidências autorizadas, preferindo caso sintético mínimo. Não publicar PCAPNG ou informações da rede por padrão. Não existe canal externo de suporte ou envio automático configurado; escolha um responsável interno autorizado.
