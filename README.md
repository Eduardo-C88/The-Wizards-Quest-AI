# The Wizards Quest AI
THE WIZARD'S QUEST: THE STOLEN SPELLBOOK é um jogo 3D roguelike baseado em waves onde o jogador controla Zyphoros, um feiticeiro em uma missão para recuperar seu livro de feitiços roubado e derrotar o Rei Esqueleto em batalhas épicas.
## Descrição Geral Do Jogo
O jogador controla o feiticeiro Zyphoros que viu o seu livro de feitiços ser roubado pelo Rei Esqueleto (Skeleton King) que pretende usá-lo para destruir o planeta. Zyphoros terá de usar as suas habilidades e ataques para derrotar, ao longo de 5 níveis, o Rei Esqueleto e o seu exército para resgatar o seu livro.

O jogo é um jogo 3D roguelike wave based low poly.

O objetivo em cada nível é derrotar vários números de waves e um mini boss que aparece no fim de cada nível. Após derrotar todas as waves, um portal irá se abrir para que o jogador avance para o próximo nível até chegar ao nível final, onde Zyphoros terá de enfrentar o Rei Esqueleto.

## Atividade do jogador
O jogador pode deslocar-se livremente pelo mapa, andando ou através de um gancho que poderá ser usado apenas em certos objetos do nível. 

Os inimigos surgirão em sítios predefinidos do mapa irão se deslocar em direção ao jogador para o atacar.

Existem 2 tipos de inimigos: os que atacam a curta distância, apenas por contacto com o player, e os que atacam a longa distância.

## Controles

| Tecla                      | Ação                          |
|----------------------------|-------------------------------|
| WASD                       | Movimento base do jogador     |
| E                          | Usar o gancho                 |
| Espaço                     | Saltar                        |
| Botão Esquerdo do Rato     | Ataque básico                 |
| Botão Direito do Rato      | Usar gancho (Swing)           |
| 1;2;3;4                    | Usar habilidades              |

## Interface
A interface apresentará a seguinte informação:
+ A vida do player;
+ Hotbar com as habilidades;
+ Mira;
+ Número de inimigos vivos por wave.

## Habilidades: 
+ Fireball: Atira uma bola de fogo que mata instantaneamente um inimigo;
+ Lightning: Atira um raio que atinge em cadeia 3 inimigos;
+ Ice shockwave: Gera uma shockwave que ataca e abranda os inimigos em todas as direções;
+ Water Beam: Gera um feixe horizontal que causa dano em quem estiver dentro da zona.

## Inteligência Artificial
### Máquina de Estados Finitos (FSM) do Boss
Uma Máquina de Estados Finitos (FSM) é uma estrutura computacional usada para modelar comportamentos com base em estados discretos e suas transições. No contexto de jogos, FSMs são amplamente utilizadas para gerir a lógica de personagens, como inimigos ou NPCs, garantindo que eles ajam de maneira previsível e controlada.

No jogo, o comportamento do boss é gerido por uma Máquina de Estados Finitos (FSM). Este sistema alterna entre três estados principais: Patrulha (Patrolling), Perseguição (Chasing) e Ataque (Attacking), com transições dinâmicas baseadas nas ações do jogador.

#### Estados e Transições
#### Estado: Patrulha (*Patrolling*)
Comportamento: O boss percorre os pontos de patrulha predefinidos (waypoints) no mapa, movendo-se para o próximo ponto assim que chega ao atual.

Transição para Chasing: Se o jogador estiver dentro do alcance de detecção (detectionRange).

Lógica no codigo:
```csharp
void HandlePatrolling(float playerDistance)
{
    if (!nv.pathPending && nv.remainingDistance < 0.5f)
    {
        GoToNextPatrolPoint();
    }

    if (playerDistance < detectionRange)
    {
        ChangeState(BossState.Chasing);
    }
}
```

#### Estado: Perseguição (*Chasing*)
Comportamento: O boss abandona a patrulha e segue em direção ao jogador. Ele ajusta constantemente seu destino com base na posição do jogador.

Transição para Patrolling: Se o jogador sair do alcance de perseguição (loseSightRange).

Transição para Attacking: Se o jogador estiver dentro da distância de ataque (stoppingDistance).

Lógica no Código:
```csharp
void HandleChasing(float playerDistance)
{
    if (playerDistance > loseSightRange)
    {
        ChangeState(BossState.Patrolling);
    }
    else if (playerDistance <= nv.stoppingDistance)
    {
        ChangeState(BossState.Attacking);
    }
    else
    {
        nv.SetDestination(player.position);
    }
}
```

#### Estado: Ataque (*Attacking*)
Comportamento: O boss ataca o jogador enquanto está dentro do alcance de ataque.

Transição para Chasing: Se o jogador se afastar do alcance de ataque.

Lógica no Código:
```csharp
void HandleAttacking()
{
    Debug.Log("Boss está atacando o jogador!");

    // Transição de volta para Chasing caso o jogador fuja
    float playerDistance = Vector3.Distance(player.position, transform.position);
    if (playerDistance > nv.stoppingDistance)
    {
        ChangeState(BossState.Chasing);
    }
}
```

#### Transições de Estado
As transições entre os estados são gerenciadas pela função ChangeState, que regista a mudança de estado e executa as ações correspondentes ao novo estado:
```csharp
void ChangeState(BossState newState)
{
    currentState = newState;
    Debug.Log($"Boss mudou para o estado: {newState}");

    switch (newState)
    {
        case BossState.Patrolling:
            GoToNextPatrolPoint();
            break;
        case BossState.Chasing:
            break;
        case BossState.Attacking:
            break;
    }
}
```
#### Lógica Central: Função Update
A função Update é onde a lógica da máquina de estados é avaliada a cada quadro. Um switch é usado para determinar o comportamento do boss com base no estado atual.

Descrição:
+ A cada ciclo de Update, a FSM verifica o estado atual do boss e executa a função correspondente:
    + HandlePatrolling() para Patrolling.
    + HandleChasing() para Chasing.
    + HandleAttacking() para Attacking.
+ Além disso, é aqui que a distância entre o boss e o jogador é calculada, determinando possíveis mudanças de estado.

Código:
```csharp
void Update()
{
    if (isDead) return;

    float playerDistance = Vector3.Distance(player.position, transform.position);

    // Verifica se o boss deve entrar em "rage"
    if (!isInRage && health <= maxHealth * 0.3f)
    {
        EnterRageMode();
    }

    // Atualiza o comportamento com base no estado atual
    switch (currentState)
    {
        case BossState.Patrolling:
            HandlePatrolling(playerDistance);
            break;
        case BossState.Chasing:
            HandleChasing(playerDistance);
            break;
        case BossState.Attacking:
            HandleAttacking();
            break;
    }
}
```
### Árvore de Comportamentos (*Behavior Tree*)
Este projeto implementa uma Behavior Tree (Árvore de Comportamento) para controlar a lógica de decisões de um inimigo do tipo boss em um jogo. A Behavior Tree organiza e prioriza as ações do boss, como atacar com habilidades específicas, avaliar a posição do jogador, verificar linhas de visão e respeitar os tempos de recarga das habilidades.

A Behavior Tree é uma estrutura hierárquica muito utilizada em jogos para gerenciar comportamentos complexos de NPCs (Non-Playable Characters). Diferentemente de máquinas de estado (FSMs), ela é mais modular e escalável, permitindo a adição de novos comportamentos com menos risco de causar conflitos, mas ainda sendo possivel utilizar as duas em simultâneo.

#### Estrutura da Árvore de Comportamentos
A Behavior Tree do boss foi implementada usando uma combinação de Selector Nodes (nós que priorizam a primeira condição verdadeira) e Sequence Nodes (nós que executam uma lista de condições em sequência).

##### Árvore Resumida
Root (Selector)
├── Skill 1 (Sequence)
│   ├── CheckPlayerInRange (dentro de um range específico)
│   ├── CheckCooldown (tempo de recarga disponível)
│   ├── CheckLoS (linha de visão válida)
│   └── UseBossSkill1 (executa a Skill 1)
└── Skill 2 (Sequence)
    ├── CheckPlayerInRange (dentro do range de Skill 2)
    ├── CheckCooldown (tempo de recarga disponível)
    ├── CheckLoS (linha de visão válida)
    └── UseBossSkill2 (executa a Skill 2)

##### Explicação dos Componentes
**1. Selector (Nodo Raiz)**
O Selector avalia os comportamentos de forma hierárquica. Ele tenta os nós filhos em sequência e escolhe o primeiro que tiver sucesso. Isso garante que o boss sempre execute a habilidade mais adequada às condições atuais.

**2. Sequence (Sequências para Skill 1 e Skill 2)**
Cada habilidade do boss é representada por uma sequência. A sequência avalia:
+ Se o jogador está dentro do alcance.
+ Se a habilidade está fora do tempo de recarga.
+ Se o jogador está visível (linha de visão).
+ Se todas as condições acima forem cumpridas, a habilidade é usada.

**3. Nós Condicionais**
Estes nós avaliam condições específicas:
+ CheckPlayerInRange: Verifica se o jogador está dentro da faixa de distância (máxima e mínima, se aplicável).
+ CheckCooldown: Avalia se a habilidade está fora do tempo de recarga.
+ CheckLoS: Verifica se há uma linha de visão entre o boss e o jogador.

**4. Nós de Ação**
+ UseBossSkill1: Executa a habilidade 1 (um ataque direcionado).
+ UseBossSkill3: Executa a habilidade 2 (um ataque em área).