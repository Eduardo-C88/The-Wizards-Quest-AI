# The Wizards Quest AI
## Descrição Geral Do Jogo
O jogador controla o feiticeiro Zyphoros que viu o seu livro de feitiços ser roubado pelo Rei Esqueleto (Skeleton King) que pretende usá-lo para destruir o planeta. Zyphoros terá de usar as suas habilidades e ataques para derrotar, ao longo de 5 níveis, o Rei Esqueleto e o seu exército para resgatar o seu livro.

O jogo é um jogo 3D roguelike wave based low poly.

O objetivo em cada nível é derrotar vários números de waves e um mini boss que aparece no fim de cada nível. Após derrotar todas as waves, um portal irá se abrir para que o jogador avance para o próximo nível até chegar ao nível final, onde Zyphoros terá de enfrentar o Rei Esqueleto.

## Atividade do jogador
O jogador pode deslocar-se livremente pelo mapa, andando ou através de um gancho que poderá ser usado apenas em certos objetos do nível. 

Os inimigos nascerão em sítios predefinidos do mapa irão se deslocar em direção ao jogador para o atacar.

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

No nosso jogo, o BossController utiliza uma FSM para alternar entre três estados principais: Patrulha (Patrolling), Perseguição (Chasing) e Ataque (Attacking). Cada estado define um comportamento específico do boss, dependendo das ações do jogador e do ambiente.

### Estados e Transições
#### Estado: Patrulha (Patrolling)
Comportamento: O boss percorre os pontos de patrulha predefinidos (waypoints) no mapa, movendo-se para o próximo ponto assim que chega ao atual.

Transição para Chasing: Se o jogador estiver dentro do alcance de detecção (detectionRange).

Lógica no codigo:
```
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

#### Estado: Perseguição (Chasing)
Comportamento: O boss abandona a patrulha e segue em direção ao jogador. Ele ajusta constantemente seu destino com base na posição do jogador.

Transição para Patrolling: Se o jogador sair do alcance de perseguição (loseSightRange).

Transição para Attacking: Se o jogador estiver dentro da distância de ataque (stoppingDistance).

Lógica no Código:
```
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

#### Estado: Ataque (Attacking)
Comportamento: O boss ataca o jogador enquanto está dentro do alcance de ataque.

Transição para Chasing: Se o jogador se afastar do alcance de ataque.

Lógica no Código:
```
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
```
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
+Descrição:
    +A cada ciclo de Update, a FSM verifica o estado atual do boss e executa a função correspondente:
        +HandlePatrolling() para Patrolling.
        +HandleChasing() para Chasing.
        +HandleAttacking() para Attacking.
    +Além disso, é aqui que a distância entre o boss e o jogador é calculada, determinando possíveis mudanças de estado.
+Código:
```
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
