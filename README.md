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
A Behavior Tree do boss foi implementada usando uma combinação de:
+ Selector Nodes (nós que priorizam a primeira condição verdadeira) 
+ Sequence Nodes (nós que executam uma lista de condições em sequência)

##### Árvore Resumida
```plaintext
Root (Selector)
├── Skill 1 (Sequence)
│   ├── CheckPlayerInRange (dentro de um range específico)
│   ├── CheckCooldown (tempo de recarga disponível)
│   ├── CheckLoS (linha de visão válida)
│   └── UseBossSkill1 (executa a Skill 1)
└── Skill 2 (Sequence)
    ├── CheckPlayerInRange (dentro do range de Skill 3)
    ├── CheckCooldown (tempo de recarga disponível)
    ├── CheckLoS (linha de visão válida)
    └── UseBossSkill3 (executa a Skill 3)
```

##### Explicação dos Componentes

**Node.cs**
``` csharp
namespace BehaviorTree
{
	public enum NodeState
	{
		Success,
		Failure,
		Running
	}
	
	public class Node
	{
		protected NodeState state;
		
		public Node parent;
		protected List<Node> children = new List<Node>();
		
		private Dictionary<string, object> _dataContext = new Dictionary<string, object>();
		
		public Node()
		{
			parent = null;
		}
		
		public Node(List<Node> children)
		{
			foreach (Node child in children)
			{
				_Attach(child);
			}
		}
		
		private void _Attach(Node node)
		{
			node.parent = this;
			children.Add(node);
		}
		
		public virtual NodeState Evaluate()
		{
			return NodeState.Failure;
		}
		
		public void SetData(string key, object value)
		{
			_dataContext[key] = value;
		}
		
		public object GetData(string key)
		{
			object value = null;
			if (_dataContext.TryGetValue(key, out value))
			{
				return value;
			}

			Node node = parent;
			while(node != null)
			{
				value = node.GetData(key);
				if (value != null)
				{
					return value;
				}
				node = node.parent;
			}
			return null;
		}
		
		public bool ClearData(string key)
		{
			if(_dataContext.ContainsKey(key))
			{
				_dataContext.Remove(key);
				return true;
			}

			Node node = parent;
			while(node != null)
			{
				bool cleared = node.ClearData(key);
				if (cleared)
				{
					return true;
				}
				node = node.parent;
			}
			return false;
		}
	}
	
}
```

**1. Selector (Nodo Raiz)**

O Selector avalia os comportamentos de forma hierárquica. Ele tenta os nós filhos em sequência e escolhe o primeiro que tiver sucesso. Isso garante que o boss sempre execute a habilidade mais adequada às condições atuais.

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

**2. Sequence (Sequências para Skill 1 e Skill 2)**

Cada habilidade do boss é representada por uma sequência. A sequência avalia:
+ Se o jogador está dentro do alcance.
+ Se a habilidade está fora do tempo de recarga.
+ Se o jogador está visível (linha de visão).
+ Se todas as condições acima forem cumpridas, a habilidade é usada.

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

**3. Nós Condicionais**

Estes nós avaliam condições específicas:
+ CheckPlayerInRange: Verifica se o jogador está dentro da faixa de distância (máxima e mínima, se aplicável).
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
+ CheckCooldown: Avalia se a habilidade está fora do tempo de recarga.
```cs
public class CheckCooldown : Node{
	private BossSpell1 bossSpell1;
	private BossSpell3 bossSpell3;
	
	public CheckCooldown(BossSpell1 bossSpell1){
		this.bossSpell1 = bossSpell1;
	}
	public CheckCooldown(BossSpell3 bossSpell3){
		this.bossSpell3 = bossSpell3;
	}
	
	public override NodeState Evaluate(){
		// Check cooldown for BossSpell1 if it's not null
        if (bossSpell1 != null)
        {
            if (!bossSpell1.isReady) // If spell1 is not ready
            {
                state = NodeState.Failure; // Fail the node
                return state;
            }
        }

        // Check cooldown for BossSpell3 if it's not null
        if (bossSpell3 != null)
        {
            if (!bossSpell3.isReady) // If spell3 is not ready
            {
                state = NodeState.Failure; // Fail the node
                return state;
            }
        }

        // If neither spell is on cooldown, succeed the node
        state = NodeState.Success;
        return state;
	}
}
```
+ CheckLoS: Verifica se há uma linha de visão entre o boss e o jogador.
```cs
public class CheckLoS : Node
{
    private Transform enemyTransform;  // Transform of the entity (the boss)
    private Transform playerTransform; // Reference to the player's transform
    private float maxRange;            // Maximum range for checking (already handled by the tree)

    public CheckLoS(Transform enemyTransform, Transform playerTransform, float maxRange)
    {
        this.enemyTransform = enemyTransform;
        this.playerTransform = playerTransform;
        this.maxRange = maxRange;
    }

    public override NodeState Evaluate()
    {
        // If player reference is missing, return failure (can't check LoS)
        if (playerTransform == null) 
        {
            state = NodeState.Failure;
            return state;
        }

        // Cast a ray from the enemy to the player
        Ray ray = new Ray(enemyTransform.position, playerTransform.position - enemyTransform.position);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, maxRange))
        {
            // Check if the ray hits the player directly
            if (hit.collider.CompareTag("Player"))
            {
                state = NodeState.Success;  // Line of sight is clear
            }
            else
            {
                state = NodeState.Failure;  // Hit something else (wall, obstacle)
            }
        }
        else
        {
            state = NodeState.Failure;  // Ray did not hit anything (something blocking LoS)
        }

        return state;
    }
}
```

**4. Nós de Ação**

+ UseBossSkill1: Executa a habilidade 1 (um ataque direcionado).
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
+ UseBossSkill2: Executa a habilidade 2 (um ataque em área).
```cs
public class UseBossSkill3 : Node
{
	public override NodeState Evaluate()
	{
		BossBT.bossSpell3.TriggerSpell();
		
		state = NodeState.Success;
		return state;
	}
}
```