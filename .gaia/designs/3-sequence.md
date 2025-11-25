<!-- reference @.gaia/designs/design.md -->
<!-- reference @.gaia/designs/1-use-cases.md -->
<!-- reference @.gaia/designs/2-class.md -->

[<< Back](./design.md)

# Sequence Diagrams

Use case execution flows showing class collaboration over time.

## Template Guidance

**Purpose**: Show how system executes use cases through object interactions
**Focus**: Use case flows, object collaboration, interaction patterns
**Avoid**: Implementation details, infrastructure concerns

**Guidelines**: Use case driven, appropriate abstraction, include error scenarios

## Primary Use Case Flows

### UC-001: Join Multiplayer Game

```mermaid
sequenceDiagram
    autonumber
    participant Player
    participant Frontend
    participant API
    participant GameSessionManager
    participant MultiplayerManager
    participant PathfindingEngine
    participant Redis
    participant Database

    Player->>Frontend: Click "Join Game" (lobby)
    Frontend->>API: POST /api/sessions/{sessionId}/join {tribeId}
    API->>GameSessionManager: JoinSession(sessionId, userId, tribeId)

    GameSessionManager->>Database: Get session by ID
    Database-->>GameSessionManager: Session data

    alt Session Full or Invalid
        GameSessionManager-->>API: Error: Session full/invalid
        API-->>Frontend: 400 Bad Request
        Frontend-->>Player: Show error message
    else Session Valid
        GameSessionManager->>Database: Add player to session
        GameSessionManager->>GameSessionManager: Validate all players ready (2-4 players)

        alt Not All Ready
            GameSessionManager-->>API: Player added, waiting for others
            API-->>Frontend: 200 OK {status: "waiting"}
        else All Ready
            GameSessionManager->>Database: Get map data
            GameSessionManager->>PathfindingEngine: Initialize pathfinding grid
            GameSessionManager->>Redis: Cache game state
            GameSessionManager->>MultiplayerManager: BroadcastToSession("game:start")
            MultiplayerManager-->>Frontend: WebSocket: game:start event
            Frontend->>Frontend: Load game canvas, initialize PixiJS
            Frontend-->>Player: Game countdown (3, 2, 1, Start!)
            GameSessionManager-->>API: Session started
            API-->>Frontend: 200 OK {status: "in_progress"}
        end
    end
```

### UC-002: Place Tower (with Pathfinding Recalculation)

```mermaid
sequenceDiagram
    autonumber
    participant Player
    participant Frontend
    participant WebSocket
    participant GameSessionManager
    participant TowerEngine
    participant PathfindingEngine
    participant MultiplayerManager
    participant EnemyAIEngine

    Player->>Frontend: Click tower type, click tile (x, y)
    Frontend->>Frontend: Validate sufficient gold (100)
    Frontend->>WebSocket: Emit "tower:place" {type, x, y}

    WebSocket->>GameSessionManager: HandleTowerPlacement(sessionId, playerId, towerData)
    GameSessionManager->>TowerEngine: PlaceTower(type, position, ownerId, tribe)

    TowerEngine->>TowerEngine: Validate tile availability
    alt Tile Occupied or Invalid
        TowerEngine-->>GameSessionManager: Error: Invalid placement
        GameSessionManager->>WebSocket: Emit "tower:place:error"
        WebSocket-->>Frontend: Show red overlay, prevent placement
    else Tile Valid
        TowerEngine->>TowerEngine: Deduct tower cost (100 gold)
        TowerEngine->>TowerEngine: Create tower instance
        TowerEngine->>TowerEngine: Apply tribe bonuses to tower stats
        TowerEngine-->>GameSessionManager: Tower created

        GameSessionManager->>PathfindingEngine: RecalculateAllPaths(enemies, grid)
        PathfindingEngine->>PathfindingEngine: Mark tile as blocked
        PathfindingEngine->>PathfindingEngine: Run A* for all active enemies

        alt All Enemies Boxed In (No Path)
            PathfindingEngine->>PathfindingEngine: Find nearest tower for each enemy
            PathfindingEngine-->>GameSessionManager: Paths + target towers
            GameSessionManager->>EnemyAIEngine: ProcessEnemyActions(enemies, towers)
            EnemyAIEngine->>EnemyAIEngine: Set enemies to attack mode
            EnemyAIEngine->>TowerEngine: Enemy targets tower for destruction
        else Paths Exist
            PathfindingEngine-->>GameSessionManager: Updated paths
        end

        GameSessionManager->>MultiplayerManager: BroadcastToSession("tower:placed", towerData)
        MultiplayerManager->>WebSocket: Broadcast to all clients
        WebSocket-->>Frontend: Receive "tower:placed" event
        Frontend->>Frontend: Render tower on all players' canvases
        Frontend->>Frontend: Update player gold
        Frontend-->>Player: Tower visible, enemies reroute
    end
```

### UC-006: Start Wave (Enemy Spawning)

```mermaid
sequenceDiagram
    autonumber
    participant Player
    participant Frontend
    participant WebSocket
    participant GameSessionManager
    participant EnemyAIEngine
    participant PathfindingEngine
    participant MultiplayerManager

    Player->>Frontend: Click "Start Wave" button
    Frontend->>WebSocket: Emit "wave:start"

    WebSocket->>GameSessionManager: HandleWaveStart(sessionId)
    GameSessionManager->>GameSessionManager: Validate all players ready (or 10s timeout)

    alt Players Not Ready
        GameSessionManager->>WebSocket: Emit "wave:waiting"
        WebSocket-->>Frontend: Show waiting message
    else Players Ready
        GameSessionManager->>GameSessionManager: Increment wave number
        GameSessionManager->>EnemyAIEngine: SpawnWave(waveNumber, mapConfig)

        EnemyAIEngine->>EnemyAIEngine: Calculate difficulty scaling (wave * 1.15)
        EnemyAIEngine->>EnemyAIEngine: Spawn enemies in sequence (0.5s interval)

        loop For Each Enemy
            EnemyAIEngine->>EnemyAIEngine: Create enemy (type, health, speed, gold reward)
            EnemyAIEngine->>PathfindingEngine: CalculatePath(spawnPoint, goalPoint, grid)
            PathfindingEngine->>PathfindingEngine: Run A* algorithm
            PathfindingEngine-->>EnemyAIEngine: Enemy path
            EnemyAIEngine->>MultiplayerManager: BroadcastToSession("enemy:spawn", enemyData)
        end

        MultiplayerManager->>WebSocket: Broadcast enemy spawn events
        WebSocket-->>Frontend: Receive "enemy:spawn" events
        Frontend->>Frontend: Render enemies on spawn point
        Frontend->>Frontend: Start enemy movement along path
        Frontend-->>Player: Wave started, enemies moving

        GameSessionManager-->>WebSocket: Wave started
        WebSocket-->>Frontend: "wave:started" event
    end
```

### UC-008: Recalculate Pathfinding (A* Algorithm)

```mermaid
sequenceDiagram
    autonumber
    participant TowerPlacement
    participant PathfindingEngine
    participant EnemyAIEngine
    participant TowerEngine

    TowerPlacement->>PathfindingEngine: RecalculateAllPaths(enemies, grid)

    PathfindingEngine->>PathfindingEngine: Mark new tower tile as blocked
    PathfindingEngine->>PathfindingEngine: Get all active enemies

    loop For Each Enemy
        PathfindingEngine->>PathfindingEngine: A* Algorithm(currentPos, goalPoint, grid)
        PathfindingEngine->>PathfindingEngine: Calculate heuristic (Manhattan distance)
        PathfindingEngine->>PathfindingEngine: Get neighbors (adjacent tiles)
        PathfindingEngine->>PathfindingEngine: Check terrain cost modifiers

        alt Path Found (< 100ms)
            PathfindingEngine->>PathfindingEngine: Reconstruct path from A* nodes
            PathfindingEngine->>PathfindingEngine: Update enemy path array
        else No Path (Enemy Boxed In)
            PathfindingEngine->>PathfindingEngine: GetNearestTower(enemyPos, towers)
            PathfindingEngine->>EnemyAIEngine: Set enemy target to tower
            EnemyAIEngine->>EnemyAIEngine: Switch enemy to attack mode
            EnemyAIEngine->>TowerEngine: Enemy begins attacking tower
        end
    end

    PathfindingEngine-->>TowerPlacement: All paths recalculated
```

### UC-009: Synchronize Game State (WebSocket Broadcasting)

```mermaid
sequenceDiagram
    autonumber
    participant GameSessionManager
    participant MultiplayerManager
    participant Redis
    participant SocketManager
    participant Client1
    participant Client2
    participant Client3

    GameSessionManager->>MultiplayerManager: BroadcastToSession(sessionId, event, data)

    MultiplayerManager->>Redis: Get cached state
    Redis-->>MultiplayerManager: Previous game state

    MultiplayerManager->>MultiplayerManager: CreateStateDelta(oldState, newState)
    MultiplayerManager->>MultiplayerManager: Serialize delta to JSON

    MultiplayerManager->>SocketManager: Emit to session room
    SocketManager->>Client1: WebSocket message (delta)
    SocketManager->>Client2: WebSocket message (delta)
    SocketManager->>Client3: WebSocket message (delta)

    Client1->>Client1: Apply delta to local state
    Client2->>Client2: Apply delta to local state
    Client3->>Client3: Apply delta to local state

    Client1->>Client1: Interpolate changes (smooth rendering)
    Client2->>Client2: Interpolate changes (smooth rendering)
    Client3->>Client3: Interpolate changes (smooth rendering)

    MultiplayerManager->>Redis: Update cached state
```

## Error Handling & Integration Patterns

### WebSocket Disconnection Handling

```mermaid
sequenceDiagram
    autonumber
    participant Client
    participant SocketManager
    participant MultiplayerManager
    participant GameSessionManager
    participant Redis

    Client-xSocketManager: WebSocket connection lost

    SocketManager->>MultiplayerManager: HandleDisconnect(sessionId, userId)
    MultiplayerManager->>GameSessionManager: PlayerDisconnected(sessionId, userId)

    GameSessionManager->>GameSessionManager: Start 30-second reconnection timer
    GameSessionManager->>GameSessionManager: Pause player resource generation
    GameSessionManager->>GameSessionManager: Keep player towers active

    alt Player Reconnects (< 30s)
        Client->>SocketManager: WebSocket reconnection
        SocketManager->>MultiplayerManager: ReconnectPlayer(sessionId, userId)
        MultiplayerManager->>Redis: Get latest game state
        Redis-->>MultiplayerManager: Current state
        MultiplayerManager->>SocketManager: Send full state sync
        SocketManager-->>Client: Receive complete state
        Client->>Client: Restore game canvas
        GameSessionManager->>GameSessionManager: Resume resource generation
    else Player Doesn't Reconnect (> 30s)
        GameSessionManager->>GameSessionManager: Remove player from session
        GameSessionManager->>GameSessionManager: Redistribute player resources
        GameSessionManager->>MultiplayerManager: BroadcastToSession("player:left", userId)
        MultiplayerManager-->>SocketManager: Notify remaining players
    end
```

### Pathfinding Timeout Handling

```mermaid
sequenceDiagram
    autonumber
    participant TowerPlacement
    participant PathfindingEngine
    participant MultiplayerManager

    TowerPlacement->>PathfindingEngine: RecalculateAllPaths(enemies, grid)
    PathfindingEngine->>PathfindingEngine: Start recalculation (timeout: 100ms)

    alt Recalculation Complete (< 100ms)
        PathfindingEngine-->>TowerPlacement: Updated paths
        TowerPlacement->>MultiplayerManager: BroadcastToSession("paths:updated")
    else Recalculation Timeout (> 100ms)
        PathfindingEngine->>PathfindingEngine: Use previous enemy paths
        PathfindingEngine->>PathfindingEngine: Queue recalculation for next tick
        PathfindingEngine-->>TowerPlacement: Previous paths (degraded)
        TowerPlacement->>MultiplayerManager: BroadcastToSession("paths:degraded")
    end
```

## Complex Business Process

### Complete Game Loop (60fps Tick)

```mermaid
sequenceDiagram
    autonumber
    participant GameLoop
    participant EnemyAIEngine
    participant TowerEngine
    participant PhysicsEngine
    participant PathfindingEngine
    participant MultiplayerManager

    GameLoop->>GameLoop: Tick (every 16.67ms for 60fps)

    GameLoop->>EnemyAIEngine: UpdateEnemies(enemies, deltaTime)
    EnemyAIEngine->>EnemyAIEngine: Move enemies along paths
    EnemyAIEngine->>EnemyAIEngine: Check if goal reached
    EnemyAIEngine->>EnemyAIEngine: Process enemy tower attacks

    GameLoop->>TowerEngine: ProcessAttacks(towers, enemies, deltaTime)
    TowerEngine->>TowerEngine: Find target enemies in range
    TowerEngine->>TowerEngine: Calculate damage (base + tribe bonuses)
    TowerEngine->>TowerEngine: Apply damage to enemies
    TowerEngine->>TowerEngine: Check enemy health (kill if <= 0)

    GameLoop->>PhysicsEngine: UpdatePositions(entities, deltaTime)
    PhysicsEngine->>PhysicsEngine: Interpolate enemy positions
    PhysicsEngine->>PhysicsEngine: Check projectile collisions

    alt Grid Changed This Tick
        GameLoop->>PathfindingEngine: RecalculateAllPaths(enemies, grid)
    end

    GameLoop->>MultiplayerManager: SyncGameState(sessionId, stateDelta)
    MultiplayerManager->>MultiplayerManager: Broadcast only changed entities
```

## Mapping Guidelines

**Simple Use Cases**: UC-003 (Build Map), UC-004 (Customize Tribe) - Single sequence with validation
**Complex Use Cases**: UC-001 (Join Game), UC-002 (Place Tower), UC-006 (Start Wave) - Multi-step orchestration
**Real-Time Game Logic**: UC-008 (Pathfinding), UC-009 (State Sync) - Performance-critical flows with sub-100ms requirements

**Sequence Diagram Coverage**:
- UC-001: Join Multiplayer Game - Full multiplayer session initialization
- UC-002: Place Tower - Tower placement with pathfinding recalculation
- UC-006: Start Wave - Enemy spawning with A* pathfinding
- UC-008: Recalculate Pathfinding - A* algorithm execution with timeout handling
- UC-009: Synchronize Game State - WebSocket state delta broadcasting
- Error Handling: WebSocket disconnection, pathfinding timeout, reconnection flows
- Game Loop: 60fps tick with physics, enemy AI, tower attacks, state sync

**Performance Constraints**:
- Pathfinding recalculation: < 100ms for 50x50 grid with active enemies
- WebSocket state synchronization: < 100ms latency player-to-player
- Game loop tick: 16.67ms (60fps sustained)
- A* algorithm: Early termination if exceeding 100ms budget

**Instructions**: All critical game flows documented with actors from class diagrams (GameSessionManager, PathfindingEngine, TowerEngine, EnemyAIEngine, MultiplayerManager). Error scenarios include WebSocket disconnects, pathfinding timeouts, and player reconnection. Real-time multiplayer synchronization uses state delta broadcasting for performance.

[<< Back](./design.md)
