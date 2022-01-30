
# INTRO

Bona tarda tothom, el meu projecte consisteix en la creació d'un NPC fent ús de Deep Reinforcement Learning. El Deep Reinforcement Learning
es una mescla entre Deep Learning i Reinforcement learning, possant-la corta, sense tant tecnisisme, l'objectiu es que un Agent aprengui una
politica a base d'ensaig i error.

Les politiques que l'agent avia d'aprendre van ser dos, saber moures ciclicament d'esquerra a dreta entre uns limits i saber passar a mode atac
quan veies un enemic en la seva area d'actuació.

Perque he escullit aquest problema? 1. el principal es que aquest projecte em pot servir per poder enriquir el TFG i 2, no em donava molt la gana en tenir que treballar amb projectes
on avia de fer us de base de dades.

# D1

Amb aquesta diapositiva us explicaré les eines i arquitectura utilitzada per fer que un Agent aprengui una o unes politiques.
La capa d'adalt de tot es Mlagent toolkit, es un seguit de tecnologies que convinades et permeten aplicar Reinforcement learning.

Unity, ml-agents, mlagent-learn i Tensorboard, tensorboard diria que no es propia d'aquest Kit però l'he utilitzat per veure les estadistiques d'aprenentatge.

Nem a parlar una mica sobre les diferents tecnologies.

Unity: com suposo que tots sabeu, es un motor gràfic, que te la seva API propia, llançament d'events, creacio d'objectes, tractament de collision entre un munt d'altres coses. Unity treballa amb
els llenguatges de programacio, C++ i C#. El meu projecte corre en C#.

ml-agents: paquet que s'ha d'instal·lar per tenir acces una API que et permet implementar logica als Agents entre d'altres funcionalitats.

mlagents-learn: es un proces Python que te els algoritmes de aprenentatge automatic, Unity mitjançant el paquet de ml-agent es connecta a aquest procés Python mitjançasnt un broker, es el la part blava aquella.
Si us fixeu, en l'esquema aquest, hi ha aixo que diu configuation.yaml (Yet another marker language), es un fitxer que et permet configurar un seguit de parametres d'aprenentatge que utilitza els algoritmes de mlagents-learn (les versions que veureu totes han estat entrenades amb el fitxer de configuració per defecte excepte l'ultima, que vaig modificar uns quans parametres per intentar forçar que l'aprenentage sigui més rápid).

tensorboard: tecnologia de tensorflow, que bueno, et permet obrir un port local per veure estadistiques en temps real o no, del proces d'aprenentatge.

# D2

Nem a parla sobre les diferents fases del projecte, obviament primerament vaig haver d'escollir les poliques que havia de modelitzar que són aquelles dues que he explicat abans (moviments ciclics i mode attack/defensa) més endavanet entendreu perque dic mode attack/defensa.

El projecte té 4 versions diferents i cada versió a tingut que passar per la seva fase de modelització, d'entrenament, de treure estadistiques i finalment fer testings amb les xarxes neuronals resultants en l'entorn grafic. Per cert, la xarxa neuronal no es mes que un fitxer amb extensió .onnx que son les sigles d'Open Neural Network Exchange. Es un estandar que et permet reutilitzar models de machine learning en differents frameworks.

Abans de continuar amb la part d'experiments i conclusió final, nem a parlar d'on m'he inspirat o d'on he tret informació, bueno, básicament 90% del coneixement d'aquesta pràctica ha estat gracies a la bona documentació del projecte ml-agents en Github, algunes pàgines randoms d'internet i videos de Youtube.

Us he donat una mica la xapa amb Agent i que els Agents aprenent, però no he definit que es un Agent, un Agent es un ente, una bola, cuadrat, no te ni perque tenir forma, però ha de ser capaç de fer les seguents 3 coses:

- obtenir informació del medi
- poder fer accions
- ser premiat o castigat segons els seus actes. (La manera com es donen els rewards es absolutament important)

Nem a veura-ho amb la versió 1.

# V1

En el video es pot veure un ninot, aquest serà el nostre Agent, ell ha aprés a moures ciclicament d'esquerra a dreta. Com ho apres? bueno fixeu-vos en les caixes verdes del video. Aquestes caixes verdes
son colliders o triggers (API d'Unity). Quan hi ha collisions la mateixa API de unity et permet holdear la funcionalitat en unes funcions que s'executen quan aquestes interatuen. La idea aqui es que quan es crei l'Agent aquest esculli un limit aleatoriament i que al arribar al limit corresponent sigui premiat, en cas contrari castigat. Per donar-li fácilitat al seu aprenentatatge si l'orientació del vector velocitat i el vector orientació tenen el mateix sentit sel premia.

Si us fixeu en els rajos aquests, aixo es raytracing llençat per dos sensor però en aquesta versió són totalment inutils.

# V2.1

En aquesta versió l'Agent te la habilitat de fer salts, l'idea es que quan l'Agent fa un salt passa a mode atac i durant un segons (configurable) si toca a un enemic, el mata. Si es tocat per un enemic ell es mor i finalment fixeu-vos en els rajos dels sensors, rajos amb més distancia en la part frontal i menys distancia en la part trasera, per donar l'efecte de realisme que es veu més cap endavant que cap enrere.

En aquesta versió l'Agent no va entrenar mai amb enemics i per tant quan en veu un dirigin-se a ell, s'el menja tot i ja tenir la lógica de mode atac incorporat.

# V2.2

En la versió 2.2 l'Agent ja ha entrenat amb enemics, però te unes petites falles com ara saltar innesariament quan ja esta en mode atac. El raig vermell que veieu es raytracing que hem permet saber si l'Agent esta sobre el terra o no.

# V2.3

Arregla problemes de la versió 2.2 i informa li passa al motor d'aprenentatge l'estat de la variable "atac mode" perque la tingui en compte. Aquesta versió te tunejat els parametres d'aprenentatge i el nombre de sessions d'entrenament respecte les altres va pasar de 5*10^5 sessions a 5*10^6.

# Conclusions

Com a conclusions dir que fer aquest projecte m'ha consumit temps però es una de les coses més interesat i maques que he fet en la carrera. Per acabar dir que.

- 1. Us recomano visitar el projecte ml-agents de Unity en Github
- 2. El meu projecte es public a la meva compte de Github i si algu li vol donar una ullada, encantat

Per cert, així es com es veu un Agent entrenant-se amb Unity. Gràcies.
