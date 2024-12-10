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
### Máquina de Estados Finitos (FSM) do Boss - Pedro
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
```csharp
void GoToNextPatrolPoint()
{
    if (patrolPoints.Length == 0) return;

    nv.SetDestination(patrolPoints[currentPatrolIndex].position);
    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
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

```csharp
private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.GetComponent<Player>() != null)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        player.TakeDamage(damage);

        Vector3 knockbackDirection = collision.transform.position - transform.position;
        player.ApplyKnockback(knockbackDirection, knockbackForce);
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
### Árvore de Comportamentos (*Behavior Tree*) - Eduardo
Este projeto implementa uma Árvore de Comportamentos (Behavior Tree) para controlar as decisões de um inimigo do tipo boss em um jogo. A Behavior Tree organiza e prioriza as ações do boss, como atacar com habilidades específicas, avaliar a posição do jogador, verificar linha de visão e respeitar os tempos de recarga das habilidades.
Contexto no Jogo

No gameplay, esta árvore de comportamento torna o boss mais desafiador ao reagir dinamicamente ao posicionamento e ações do jogador. Por exemplo:
+ Se o jogador está dentro de um certo alcance e visível, o boss pode usar uma habilidade direcionada.
+ Se a primeira habilidade estiver em recarga, ele tentará usar uma habilidade alternativa, como um ataque em área.

#### Estrutura da Árvore de Comportamentos

##### Árvore Resumida
```plaintext
Root (Selector)
├── Skill 1 (Sequence)
│   ├── CheckPlayerInRange (dentro de um range específico)
│   ├── CheckCooldown (tempo de recarga disponível)
│   ├── CheckLoS (linha de visão válida)
│   └── UseBossSkill1 (executa a Skill 1)
└── Skill 3 (Sequence)
    ├── CheckPlayerInRange (dentro do range de Skill 3)
    ├── CheckCooldown (tempo de recarga disponível)
    ├── CheckLoS (linha de visão válida)
    └── UseBossSkill3 (executa a Skill 3)
```

| Componente         | Descrição                                                                  |
|--------------------|----------------------------------------------------------------------------|
| Root (Selector)    | Avalia os comportamentos e escolhe o primeiro filho que tiver sucesso.     |
| Skill 1 (Sequence) | Sequência de condições para executar a Habilidade 1 (ataque direcionado).  |
| Skill 3 (Sequence) | Sequência de condições para executar a Habilidade 3 (ataque em área).      |

##### Explicação dos Nós
###### **1. Selector (Nodo Raiz)**

O Selector avalia os comportamentos de forma hierárquica. Ele tenta os nós filhos em sequência e escolhe o primeiro que tiver sucesso. Isso garante que o boss sempre execute a habilidade mais adequada às condições atuais.

Exemplo de código(Selector Node)
``` cs
namespace BehaviorTree
{
	public class Selector : Node
	{
		public Selector(List<Node> children) : base(children) { }
		public Selector() : base() { }
		
		public override NodeState Evaluate()
		{
			foreach(Node node in children)
			{
				switch(node.Evaluate())
				{
					case NodeState.Failure:
						continue;
					case NodeState.Success:
						state = NodeState.Success;
						return state;
					case NodeState.Running:
						state = NodeState.Running;
						return state;
					default:
						continue;	
				}
			}
			
			state = NodeState.Failure;
			return state;
		}
	}
}
```

###### **2. Sequence (Sequências para Skill 1 e Skill 3)**

Cada habilidade do boss é representada por uma sequência. A sequência avalia:
1. Se o jogador está dentro do alcance.
2. Se a habilidade está fora do tempo de recarga.
3. Se o jogador está visível (linha de visão).
4. Se todas as condições acima forem cumpridas, a habilidade é usada.

Exemplo de código(Sequence Node)
``` cs
namespace BehaviorTree
{
	public class Sequence : Node
	{
		public Sequence(List<Node> children) : base(children) { }
		public Sequence() : base() { }
		
		public override NodeState Evaluate()
		{
			bool anyChildRunning = false;
			foreach(Node node in children)
			{
				switch(node.Evaluate())
				{
					case NodeState.Failure:
						state = NodeState.Failure;
						return state;
					case NodeState.Success:
						continue;
					case NodeState.Running:
						anyChildRunning = true;
						continue;
					default:
						state = NodeState.Success;
						return state;	
				}
			}
			
			state = anyChildRunning ? NodeState.Running : NodeState.Success;
			return state;
		}
	}
}
```

###### **3. Nós Condicionais**

| Nome               | Função                                                                                    |
|--------------------|-------------------------------------------------------------------------------------------|
| CheckPlayerInRange | Verifica se o jogador está dentro do alcance mínimo e máximo.                             |
| CheckCooldown      | Avalia se a habilidade está fora do tempo de recarga.                                     |
| CheckLoS           | Verifica se há linha de visão clara entre o boss e o jogador (sem paredes ou obstáculos). |

Exemplo de Código (CheckPlayerInRange)
```cs
public class CheckPlayerInRange : Node
{
	private UnityEngine.Transform bossTransform;
	private UnityEngine.Transform playerTransform;
	private float maxRange;
	private float minRange;

	public CheckPlayerInRange(UnityEngine.Transform bossTransform, UnityEngine.Transform playerTransform, float maxRange, float minRange)
	{
		this.bossTransform = bossTransform;
		this.playerTransform = playerTransform;
		this.maxRange = maxRange;
		this.minRange = minRange;
	}

	public override NodeState Evaluate()
	{
		float distance = Vector3.Distance(bossTransform.position, playerTransform.position);
		Debug.Log("Distance: " + distance);

		// Check if the player is within the min and max range
		if (distance < minRange || distance > maxRange)
		{
			state = NodeState.Failure; // Fail the node if player is too close or too far
			return state;
		}

		state = NodeState.Success; // Success if the player is within the valid range
		return state;
	}
}
```

###### **4. Nós de Ação**

| Nome          | Função                          |
|---------------|---------------------------------|
| UseBossSkill1 | Executa a Habilidade 1 do boss. |
| UseBossSkill3 | Executa a Habilidade 3 do boss. |

```cs
public class UseBossSkill1 : Node
{
	public override NodeState Evaluate()
	{
		BossBT.bossSpell1.TriggerSpell();
		
		state = NodeState.Success;
		return state;
	}
}
```

## ML-Agents para Enemies LVL1 - Filipe
ML-Agents (Machine Learning Agents) é uma ferramenta desenvolvida pela Unity que integra aprendizado por reforço (RL) em jogos ou simulações. Ela permite que agentes controlados por inteligência artificial aprendam comportamentos por meio de interações com o ambiente virtual, simulando situações e recebendo recompensas ou penalidades com base em suas ações. O ML-Agents utiliza frameworks como PyTorch para o treinamento do modelo, enquanto a interface de interação é feita diretamente no Unity.

#### Como os ML-Agents aprendem?
O aprendizado ocorre por meio de Reforço:

- Observações: O agente coleta informações sobre o ambiente.
- Ações: Com base nas observações, ele decide o que fazer.
- Recompensas: A ação tomada gera uma recompensa ou penalidade.
- Treinamento: O modelo ajusta seus parâmetros para maximizar a recompensa acumulada ao longo do tempo.
- No código fornecido, vemos um exemplo de agente treinado para perseguir um jogador enquanto evita penalizações

#### Observações

O agente coleta informações sobre o ambiente, crucial para a tomada de decisões. Neste exemplo:

- Posição relativa do jogador: O agente sabe a direção e a distância para o jogador.
- Normalização: Os valores são escalados para ficarem entre 0 e 1, facilitando o aprendizado do modelo.
```cs
public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 relativePosition = player.position - transform.position;
        if (relativePosition.magnitude == 0) relativePosition = Vector3.forward;
        sensor.AddObservation(relativePosition.normalized);
        sensor.AddObservation(Mathf.Clamp01(relativePosition.magnitude / 50f));
    }
```

#### Ações

O agente recebe ações em formato contínuo (valores entre -1 e 1). As ações controlam:
- Movimento (moveX, moveZ).
- Rotação.
O cálculo da movimentação é suavizado para garantir transições realistas:
```cs
 public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

	Vector3 targetDirection = new Vector3(moveX, 0, moveZ);
        if (targetDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        Vector3 targetVelocity = targetDirection.normalized * moveSpeed;
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * 5f);
```

#### Recompensas

As recompensas incentivam o agente a se comportar de maneira desejada e com tal devem ser ajustados ou alterados dependendo da resposta deste ao longo do treino.

- Recompensas a incentivar proximidade com o Player 
```cs
if (distanceToPlayer <= rewardProximityToPlayer)
    AddReward(0.1f);
if (distanceToPlayer <= proximityToPlayerRange)
    AddReward(0.5f);
```

- Penalização por distanciamento com o Player
```cs
if (distanceToPlayer >= penaltyDistanceFromPlayer)
    AddReward(-0.001f);
```

- Penalização por proximidade com outros Agents (apenas quando o Player não está no range definido)
```cs
Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, proximityPenaltyRange, enemyLayer);
            if (nearbyEnemies.Length > 1 && distanceToPlayer > proximityToPlayerRange)
            {
                AddReward(-0.6f);
            }
```

#### Treinamento 

A duração de cada episódio é limitada para evitar que o treinamento fique muito longo ou irrelevante.
```cs
if (episodeTimer >= episodeDuration)
    EndEpisode();
```
Exemplo de ficheiro yaml usado para aprofundar o treinamento dos ML-Agents depois de completar o treino "default"
```yaml
behaviors:
  BasicEnemyAgent:
    trainer_type: ppo  # PPO is the standard reinforcement learning method
    hyperparameters:
      batch_size: 64  # Number of samples per batch
      buffer_size: 10240  # Total number of experiences in the replay buffer
      learning_rate: 3.0e-4  # Learning rate to optimize training
      num_epoch: 3  # Number of epochs for each optimization pass
    max_steps: 5000000  # Total timesteps to train for
    keep_checkpoints: 5  # Number of checkpoints to keep during training
    checkpoint_interval: 100000  # Save agent progress at every 100,000 steps
    summary_freq: 1000  # Frequency at which summaries/logging are performed
```
## Trabalho de:
+ 25969 Eduardo Costa
+ 25981 Filipe Araújo
+ 25982 Pedro Rodrigues
