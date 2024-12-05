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

###Estados e Transições
1. Estado: Patrulha (Patrolling)
Comportamento: O boss percorre os pontos de patrulha predefinidos (waypoints) no mapa, movendo-se para o próximo ponto assim que chega ao atual.
Transição para Chasing: Se o jogador estiver dentro do alcance de detecção (detectionRange).

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
