# Registro consolidado de evidências

Consolidação documental em 02/10/2026. Não foram repetidas capturas ou ensaios de rede para escrever estes documentos.

| ID documental | Evidência | Resultado e alcance |
|---|---|---|
| EV-01 | Check Release, raiz artifacts/checks/d8b3158a65294a9ca790a692e3899057 | 39 verificações; zero erros/avisos de build; domínio, banco, parser e arquivo |
| EV-02 | verify-worker em binário publicado/instalado | Dois casos de falha tratados; sem captura iniciada |
| EV-03 | artifacts/qa/installer-result.json | Install/Reinstall/Uninstall/RunningAppBlocked true; 417 arquivos; 13 arquivos de usuário preservados |
| EV-04 | artifacts/qa/field-evidence.json | Quatro hashes corretos e totais reanalisados |
| EV-05 | Janela instalada observada por acessibilidade | Sessões carregadas e importação de 9.501 pacotes visível; não é QA visual integral |

Os caminhos de artifacts são locais e ignorados pelo Git. Sem esses arquivos, este documento é registro do ensaio, não substituto da evidência bruta. Não há CI remoto ou revisor independente registrado.

## Campo

| Execução arquivada | Estado | Pacotes | Descartes do driver | SHA-256 do PCAPNG |
|---|---|---:|---|---|
| 20261001-062146-dc75993ee82c474b9469c9696e464e29 | Limite atingido | 3.751 | 0 registrado | 353420C1D81C0C3D54D180AF8AAF7E9D6F17F585F576EA9F11DAEC33EE84802B |
| 20261001-062357-f09c3ceda10b4d4d911f1e1c19ff026a | Concluída | 3.773 | 0 registrado | C868E177BD38EC190355F005AD42839004CBE9114E1D83B5E39453733EA4A672 |
| 20261001-060610-02690f04e9d3488d8fde00c077c2a795 | Importado | 35.483 | Indisponível | 8EC28B4FB11047CFE37AF17358C773D55557618B320A2BA14077DE43E8E66C26 |
| 20261001-060725-84ec09178066473e97cd7c3bd95795c5 | Importado | 9.501 | Indisponível | A90B8C076FAE530561255E2F1724CCB9C9E4982E68B7891A0DFB298CCE078A39 |

Arquivos permanecem no perfil do usuário, não no repositório. Capinfos reconheceu PCAPNG Ethernet; seu total de 35.483 apareceu arredondado como 35 k na saída padrão. O número exato veio do resumo e da reanálise. Zero descartes não mede perda ponta a ponta nem comprova carga máxima suportada.

## Pacote

Installer: NetToolsAV-0.2.0-preview.2-win-x64-setup.exe, 47.408.084 bytes. SHA-256: `50B2EE1C646B17C32458DCD81B5C8689E3C76283B79B2B8499C9F891514F4F84`. Runtime incluído; sem driver/capturas/dados de usuário. Não assinado digitalmente. Reinstalação testada não equivale a upgrade entre versões.

## Limitações de QA

Ferramenta de UI teve falhas/atrasos e imagem inconsistente com janela selecionada. Imagem de outra aplicação não foi usada como evidência. Importação/exportação por código e árvore de acessibilidade não substituem conferência visual de todos os fluxos. Máquina limpa, recuperação integral, carga prolongada e módulos ausentes continuam pendentes.
