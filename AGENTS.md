# Instruções para agentes de IA — NetTools AV

Aplicam-se a este repositório. Instruções do usuário e políticas do ambiente têm precedência. Este arquivo não autoriza publicação, envio de dados ou provas em redes externas.

## Contexto obrigatório

Leia `docs/README.md`, `docs/engenharia/14-matriz-0.2-e-homologacao.md` e o documento do módulo. Consulte `docs/ia/guia-de-trabalho.md`. Confira Git e preserve alterações existentes.

Produto Windows WPF/.NET, baseline 0.2.0-preview.2. Versão em `Directory.Build.props`, SDK em `global.json`, dependências em `packages.lock.json`. `prototype/` é histórico, não é o aplicativo instalado.

## Invariantes

- Protocolos são a raiz. Não criar inventário ou diagnóstico por equipamento. IP/MAC/porta/grupo são atributos de evidência.
- Catálogo não significa analisador; presença não significa saúde. Não observado, não executado, indisponível e falha são distintos.
- Não declarar 1.0 sem homologação completa e aceite registrado. Não inventar testes, hardware, resultados ou conformidade.
- Preserve o layout aprovado e os temas. Documente decisões novas de escopo/arquitetura antes de codificá-las.
- Não iniciar captura, alterar rede, instalar drivers ou gerar tráfego como parte de tarefa documental. Use fixtures offline primeiro; campo segue o escopo autorizado pelo usuário.
- Preserve banco e capturas. Não embutir dados de campo em testes, commits, pacotes ou transmissões externas.
- Não presumir direito de redistribuir Npcap. Não carregar DLLs arbitrárias nem interpretar BPF como shell.

## Engenharia

Core contém domínio/análise sem WPF ou SQLite. Infrastructure implementa banco, arquivos e interop. Desktop orquestra UI/host; usa code-behind, não MVVM completo.

Antes de parser: fonte/versionamento, perfil suportado, limites e casos positivos/negativos/truncados. Não identificar protocolo somente por porta quando sua estrutura é necessária.

Use `scripts/build.ps1 -Target Check` para mudanças C#. Testes do worker não capturam tráfego; descreva seus limites. `scripts/verify-installer.ps1` instala/remove o app e só deve rodar em perfil de teste sem instalação registrada. Não é verificação genérica.

Prefira SDK local quando presente. Não remover locks nem desabilitar avisos/auditoria para esconder falha. Não sobrescrever release existente silenciosamente. Não alegar commits, tags ou CI inexistentes.

## Encerramento

Informe comportamento/arquivos alterados, verificações executadas e pendências. Para documentação, valide links, caminhos, versões e diagramas; não requer prova de rede. Preserve histórico e proveniência de evidências.
