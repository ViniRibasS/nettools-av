'use strict';
const ProtocolCatalog = (() => {
  const definitions = [
    ['mdns','mDNS / DNS-SD','Descoberta','RFC 6762 / 6763','UDP · 5353','layers'],
    ['igmp','IGMPv3','Multicast','v3','IP · protocolo 2','layers'],
    ['ptp','PTPv2','Sincronismo','v2','UDP · 319 / 320','clock'],
    ['rtp','RTP / RTCP','Mídia','RTP v2','UDP · portas por sessão','activity'],
    ['qdp','QDP','Descoberta','Perfil pendente','UDP · perfil QDP','search'],
    ['icmp','ICMPv4','Rede básica','IPv4','IP · protocolo 1','activity'],
    ['arp','ARP','Rede básica','Ethernet / IPv4','Ethernet','ethernet'],
    ['tcp','TCP','Transporte','RFC 9293','IP · protocolo 6','activity'],
    ['udp','UDP','Transporte','RFC 768','IP · protocolo 17','activity'],
    ['dns','DNS','Resolução','Perfil DNS','UDP / TCP','search'],
    ['dhcp','DHCPv4','Endereçamento','IPv4','UDP','ethernet'],
    ['ssdp','SSDP','Descoberta','Perfil SSDP','UDP','search'],
    ['lldp','LLDP','Vizinhança','Perfil LLDP','Ethernet','layers'],
    ['ndp','NDP / ICMPv6','Rede básica','IPv6','ICMPv6','ethernet'],
    ['sap','SAP / SDP','Anúncios de sessão','Perfil SAP / SDP','UDP / descrição de sessão','file'],
    ['rtsp','RTSP','Controle de mídia','Versão a selecionar','TCP','activity'],
    ['ndi','NDI','Tecnologia / integração','Mecanismo a definir','Conforme documentação / SDK','layers'],
    ['artnet','Art-Net','Automação','Revisão a selecionar','UDP','activity'],
    ['sacn','sACN','Automação','E1.31 · revisão pendente','UDP','activity'],
    ['modbus','Modbus TCP','Automação','Consultas de leitura','TCP','layers'],
    ['snmp','SNMP','Gerenciamento','Versão a selecionar','UDP','layers'],
    ['dante','Dante','Perfil de correlação','Descoberta / relógio / mídia','Múltiplos protocolos','layers'],
    ['aes67','AES67','Perfil de correlação','Perfil a validar','PTP / RTP / descrição de sessão','layers'],
    ['qlan','Q-LAN','Perfil de correlação','Perfil a validar','Conforme documentação','layers']
  ].map(([id,name,family,version,transport,icon])=>({id,name,family,version,transport,icon}));
  const observations = {
    mdns:{presence:true,packets:124,state:'Validado',status:'ok',criterion:'Registros da amostra interpretáveis',detail:'Estruturas DNS e registros de serviço da amostra foram interpretados. O critério valida apenas os campos observados.',metrics:[['Registros PTR',18],['Registros SRV',12],['Mensagens truncadas',0]],limit:'Anúncios observados não comprovam transporte de mídia ou cobertura de toda a rede.'},
    igmp:{presence:true,packets:18,state:'Validado',status:'ok',criterion:'Queries e reports interpretáveis',detail:'Queries e reports v3 presentes no cenário. Grupos e tipos de registro foram identificados.',metrics:[['Queries',3],['Reports',15],['Grupos observados',4]],limit:'Captura local não comprova as tabelas de snooping nem a entrega multicast fora deste ponto.'},
    ptp:{presence:true,packets:360,state:'Inconclusivo',status:'neutral',criterion:'Precisão de sincronismo',detail:'Mensagens PTPv2 presentes. A amostra permite observar tipo, domínio e sequência; não medir precisão do sincronismo.',metrics:[['Domínio observado',0],['Mensagens Sync',120],['Mensagens Follow_Up',120]],limit:'Presença de PTP não comprova lock. Não há medição de offset ou base de tempo qualificada.'},
    rtp:{presence:true,packets:2997,state:'Atenção',status:'warn',criterion:'Continuidade de sequência RTP',detail:'Foram observadas três lacunas de sequência no fluxo de exemplo. A origem das lacunas é inconclusiva.',metrics:[['Fluxos RTP',1],['Lacunas de sequência',3],['Jitter RTP','Indisponível']],limit:'Descartes da captura não estão disponíveis. Não atribuir as lacunas à rede. Clock rate não confirmado; jitter não calculado.'},
    qdp:{presence:false,packets:0,state:'Não observado',status:'neutral',criterion:'Presença na janela de captura',detail:'Nenhuma mensagem QDP interpretável foi observada nos 60 segundos do cenário.',metrics:[['Janela', '60 s'],['Mensagens identificadas',0],['Perfil','Pendente']],limit:'Ausência na captura não confirma bloqueio. A implementação do parser depende de fonte verificável de estrutura QDP.'},
    icmp:{presence:true,packets:40,state:'Validado',status:'ok',criterion:'Correlação Echo Request / Reply',detail:'Vinte pares de solicitação e resposta correlacionados na amostra ICMPv4.',metrics:[['Echo Request',20],['Echo Reply',20],['RTT médio','0,85 ms']],limit:'O critério se restringe às mensagens ICMP correlacionadas. Não é diagnóstico de disponibilidade ou saúde de equipamento.'}
  };
  function blank(id){const def=definitions.find(p=>p.id===id);if(!def)return null;return {...def,presence:null,packets:null,state:'Não executado',status:'neutral',criterion:'Análise ainda não executada',detail:'Protocolo selecionado para esta sessão. Nenhuma análise ou coleta foi realizada.',metrics:[],limit:'A seleção no catálogo não comprova implementação nem suporte homologado.'}}
  function demo(){return Object.entries(observations).map(([id,data])=>({...blank(id),...data}))}
  return {definitions,blank,demo};
})();
