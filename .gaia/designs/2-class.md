<!-- reference @.gaia/designs/design.md -->
<!-- reference @.gaia/designs/1-use-cases.md -->

[<< Back](./design.md)

# Class Diagrams & Data Models

Isometric Tower Defense Game - Domain models, service layers, and data architecture following iDesign principles.

## Template Guidance

**Purpose**: Define classes, relationships, and data flow
**Focus**: Domain entities, interfaces, data structure
**Avoid**: Implementation details, database optimization

**Guidelines**: Domain-driven design, interface-focused, clear relationships

## System Architecture

```mermaid
classDiagram
    class Managers {
        <<layer>>
        +GameSessionManager
        +MultiplayerManager
        +MapManager
        +TribeManager
    }
    class Engines {
        <<layer>>
        +PathfindingEngine
        +TowerEngine
        +EnemyAIEngine
        +PhysicsEngine
    }
    class DataAccess {
        <<layer>>
        +UserRepository
        +MapRepository
        +TribeRepository
        +GameSessionRepository
    }
    class Models {
        <<layer>>
        +User, Map, Tribe
        +Tower, Enemy, Tile
        +GameSession
    }

    Managers --> Engines: uses
    Managers --> DataAccess: uses
    Engines --> DataAccess: uses
    Engines --> Models: operates on
    DataAccess --> Models: persists
```

## Domain Model

```mermaid
classDiagram
    class User {
        +Id: UUID
        +Email: string
        +Username: string
        +PasswordHash: string
        +CreatedAt: DateTime
        +LastLogin: DateTime
        +Level: number
        +TotalGamesPlayed: number
    }

    class Tribe {
        +Id: UUID
        +UserId: UUID
        +Name: string
        +Archetype: TribeArchetype
        +DamageBonus: number
        +AttackSpeedBonus: number
        +RangeBonus: number
        +ColorTheme: string
        +CreatedAt: DateTime
    }

    class TribeArchetype {
        <<enumeration>>
        Aggressive
        Defensive
        Balanced
        Support
    }

    class Map {
        +Id: UUID
        +UserId: UUID
        +Name: string
        +Description: string
        +Width: number
        +Height: number
        +TerrainData: JSON
        +SpawnPoint: Point
        +GoalPoint: Point
        +WaveConfig: JSON
        +IsPublic: boolean
        +PlayCount: number
        +Rating: number
        +CreatedAt: DateTime
    }

    class Tile {
        +X: number
        +Y: number
        +TerrainType: TerrainType
        +Height: number
        +IsBlocked: boolean
        +TowerId?: UUID
    }

    class TerrainType {
        <<enumeration>>
        Grass
        Dirt
        Stone
        Water
        Lava
    }

    class GameSession {
        +Id: UUID
        +MapId: UUID
        +Players: Player[]
        +Status: SessionStatus
        +CurrentWave: number
        +SharedLives: number
        +Towers: Tower[]
        +Enemies: Enemy[]
        +CreatedAt: DateTime
        +CompletedAt?: DateTime
    }

    class SessionStatus {
        <<enumeration>>
        Lobby
        InProgress
        Paused
        Completed
        Abandoned
    }

    class Player {
        +Id: UUID
        +UserId: UUID
        +TribeId: UUID
        +Gold: number
        +IsHost: boolean
        +IsReady: boolean
        +IsConnected: boolean
    }

    class Tower {
        +Id: UUID
        +Type: TowerType
        +Position: Point
        +OwnerId: UUID
        +Level: number
        +Damage: number
        +AttackSpeed: number
        +Range: number
        +TargetMode: TargetMode
        +CreatedAt: DateTime
    }

    class TowerType {
        <<enumeration>>
        Arrow
        Cannon
        Magic
        Wall
    }

    class TargetMode {
        <<enumeration>>
        Nearest
        Strongest
        Weakest
        First
    }

    class Enemy {
        +Id: UUID
        +Type: EnemyType
        +Position: Point
        +TargetPosition: Point
        +Path: Point[]
        +Health: number
        +MaxHealth: number
        +Speed: number
        +GoldReward: number
        +IsTargetingTower: boolean
        +TargetTowerId?: UUID
    }

    class EnemyType {
        <<enumeration>>
        Basic
        Fast
        Tank
        Flying
        Boss
    }

    class Point {
        +X: number
        +Y: number
    }

    User "1" --> "0..*" Tribe: owns
    User "1" --> "0..*" Map: creates
    User --> TribeArchetype: has
    Tribe --> TribeArchetype: is
    Map "1" --> "1..*" Tile: contains
    Tile --> TerrainType: has
    GameSession "1" --> "1" Map: uses
    GameSession "1" --> "2..4" Player: contains
    GameSession "1" --> "0..*" Tower: has
    GameSession "1" --> "0..*" Enemy: tracks
    GameSession --> SessionStatus: has
    Player "1" --> "1" User: references
    Player "1" --> "1" Tribe: uses
    Tower --> TowerType: is
    Tower --> TargetMode: has
    Tower --> Point: at
    Enemy --> EnemyType: is
    Enemy --> Point: at
    Enemy "0..*" --> "0..*" Point: follows
```

## Service Layer (Managers)

```mermaid
classDiagram
    class IGameSessionManager {
        <<interface>>
        +CreateSession(mapId, hostId): Task~GameSession~
        +JoinSession(sessionId, userId, tribeId): Task~Player~
        +StartSession(sessionId): Task~Result~
        +EndSession(sessionId, reason): Task~Result~
        +GetActiveSessionByUserId(userId): Task~GameSession?~
    }

    class GameSessionManager {
        -_pathfindingEngine: IPathfindingEngine
        -_multiplayerManager: IMultiplayerManager
        -_sessionRepository: IGameSessionRepository
        +CreateSession(mapId, hostId): Task~GameSession~
        +JoinSession(sessionId, userId, tribeId): Task~Player~
        +StartSession(sessionId): Task~Result~
        +EndSession(sessionId, reason): Task~Result~
        -ValidateSession(session): bool
    }

    class IMultiplayerManager {
        <<interface>>
        +BroadcastToSession(sessionId, event): Task
        +SyncGameState(sessionId, state): Task
        +HandleDisconnect(sessionId, userId): Task
        +ReconnectPlayer(sessionId, userId): Task~Result~
    }

    class MultiplayerManager {
        -_socketManager: SocketManager
        -_redisCache: Redis
        +BroadcastToSession(sessionId, event): Task
        +SyncGameState(sessionId, state): Task
        +HandleDisconnect(sessionId, userId): Task
        -CreateStateDelta(oldState, newState): StateDelta
    }

    class IMapManager {
        <<interface>>
        +CreateMap(userId, mapData): Task~Map~
        +ValidateMap(mapData): ValidationResult
        +GetPublicMaps(page, pageSize): Task~List~Map~~
        +GetMapById(mapId): Task~Map~
        +PublishMap(mapId): Task~Result~
    }

    class MapManager {
        -_pathfindingEngine: IPathfindingEngine
        -_mapRepository: IMapRepository
        +CreateMap(userId, mapData): Task~Map~
        +ValidateMap(mapData): ValidationResult
        -CheckPathValidity(spawn, goal, terrain): bool
    }

    class ITribeManager {
        <<interface>>
        +CreateTribe(userId, tribeData): Task~Tribe~
        +UpdateTribe(tribeId, tribeData): Task~Tribe~
        +GetUserTribes(userId): Task~List~Tribe~~
        +ValidateTribeStats(tribeData): ValidationResult
    }

    class TribeManager {
        -_tribeRepository: ITribeRepository
        +CreateTribe(userId, tribeData): Task~Tribe~
        +UpdateTribe(tribeId, tribeData): Task~Tribe~
        -CalculateTotalBonusPoints(tribe): number
        -ValidateTribeStats(tribeData): ValidationResult
    }

    IGameSessionManager <|.. GameSessionManager: implements
    IMultiplayerManager <|.. MultiplayerManager: implements
    IMapManager <|.. MapManager: implements
    ITribeManager <|.. TribeManager: implements

    GameSessionManager --> IPathfindingEngine: uses
    GameSessionManager --> IMultiplayerManager: uses
    MapManager --> IPathfindingEngine: uses
```

## Engine Layer (Business Logic)

```mermaid
classDiagram
    class IPathfindingEngine {
        <<interface>>
        +CalculatePath(start, goal, grid): Point[]
        +RecalculateAllPaths(enemies, grid): Map~EnemyId, Path~
        +IsPathValid(path, grid): bool
        +GetNearestTower(position, towers): Tower?
    }

    class PathfindingEngine {
        +CalculatePath(start, goal, grid): Point[]
        +RecalculateAllPaths(enemies, grid): Map~EnemyId, Path~
        -AStarAlgorithm(start, goal, grid): Point[]
        -CalculateHeuristic(pos, goal): number
        -GetNeighbors(pos, grid): Point[]
    }

    class ITowerEngine {
        <<interface>>
        +PlaceTower(type, pos, ownerId, tribe): Tower
        +UpgradeTower(towerId): Tower
        +SellTower(towerId): number
        +ProcessAttacks(towers, enemies, deltaTime): AttackResult[]
        +CalculateTowerStats(type, level, tribe): TowerStats
    }

    class TowerEngine {
        +PlaceTower(type, pos, ownerId, tribe): Tower
        +UpgradeTower(towerId): Tower
        +ProcessAttacks(towers, enemies, deltaTime): AttackResult[]
        -FindTargetEnemy(tower, enemies): Enemy?
        -ApplyTribeBonus(baseStats, tribe): TowerStats
        -CalculateDamage(tower, enemy): number
    }

    class IEnemyAIEngine {
        <<interface>>
        +SpawnWave(waveNumber, config): Enemy[]
        +UpdateEnemies(enemies, deltaTime): void
        +ProcessEnemyActions(enemies, towers): ActionResult[]
        +CalculateEnemyStats(type, wave): EnemyStats
    }

    class EnemyAIEngine {
        +SpawnWave(waveNumber, config): Enemy[]
        +UpdateEnemies(enemies, deltaTime): void
        +ProcessEnemyActions(enemies, towers): ActionResult[]
        -MoveEnemyAlongPath(enemy, deltaTime): void
        -AttackTower(enemy, tower): number
        -CheckGoalReached(enemy): bool
    }

    class IPhysicsEngine {
        <<interface>>
        +UpdatePositions(entities, deltaTime): void
        +CheckCollisions(enemies, projectiles): Collision[]
        +IsPositionValid(pos, grid): bool
    }

    class PhysicsEngine {
        +UpdatePositions(entities, deltaTime): void
        +CheckCollisions(enemies, projectiles): Collision[]
        -InterpolatePosition(start, end, t): Point
        -GetGridCoordinates(worldPos): Point
    }

    IPathfindingEngine <|.. PathfindingEngine: implements
    ITowerEngine <|.. TowerEngine: implements
    IEnemyAIEngine <|.. EnemyAIEngine: implements
    IPhysicsEngine <|.. PhysicsEngine: implements
```

## Data Layer (Repositories)

```mermaid
classDiagram
    class IUserRepository {
        <<interface>>
        +GetByIdAsync(id): Task~User~
        +GetByEmailAsync(email): Task~User~
        +CreateAsync(user): Task~User~
        +UpdateAsync(user): Task~User~
        +UpdateLastLoginAsync(id): Task~void~
    }

    class UserRepository {
        -_prisma: PrismaClient
        +GetByIdAsync(id): Task~User~
        +GetByEmailAsync(email): Task~User~
        +CreateAsync(user): Task~User~
        +UpdateAsync(user): Task~User~
    }

    class IMapRepository {
        <<interface>>
        +GetByIdAsync(id): Task~Map~
        +GetPublicMapsAsync(page, size): Task~List~Map~~
        +GetUserMapsAsync(userId): Task~List~Map~~
        +CreateAsync(map): Task~Map~
        +UpdateAsync(map): Task~Map~
        +DeleteAsync(id): Task~bool~
    }

    class MapRepository {
        -_prisma: PrismaClient
        +GetByIdAsync(id): Task~Map~
        +GetPublicMapsAsync(page, size): Task~List~Map~~
        +CreateAsync(map): Task~Map~
        -ParseTerrainData(json): Tile[]
    }

    class ITribeRepository {
        <<interface>>
        +GetByIdAsync(id): Task~Tribe~
        +GetUserTribesAsync(userId): Task~List~Tribe~~
        +CreateAsync(tribe): Task~Tribe~
        +UpdateAsync(tribe): Task~Tribe~
        +DeleteAsync(id): Task~bool~
    }

    class TribeRepository {
        -_prisma: PrismaClient
        +GetByIdAsync(id): Task~Tribe~
        +GetUserTribesAsync(userId): Task~List~Tribe~~
        +CreateAsync(tribe): Task~Tribe~
    }

    class IGameSessionRepository {
        <<interface>>
        +GetByIdAsync(id): Task~GameSession~
        +GetActiveByUserIdAsync(userId): Task~GameSession?~
        +CreateAsync(session): Task~GameSession~
        +UpdateAsync(session): Task~GameSession~
        +DeleteAsync(id): Task~bool~
    }

    class GameSessionRepository {
        -_redis: Redis
        -_prisma: PrismaClient
        +GetByIdAsync(id): Task~GameSession~
        +CreateAsync(session): Task~GameSession~
        -CacheSession(session): Task~void~
        -InvalidateCache(sessionId): Task~void~
    }

    IUserRepository <|.. UserRepository: implements
    IMapRepository <|.. MapRepository: implements
    ITribeRepository <|.. TribeRepository: implements
    IGameSessionRepository <|.. GameSessionRepository: implements
```

## DTOs & API Contracts

```mermaid
classDiagram
    class CreateUserRequest {
        +Email: string
        +Username: string
        +Password: string
        +Validate(): ValidationResult
    }

    class UserResponse {
        +Id: UUID
        +Email: string
        +Username: string
        +Level: number
        +CreatedAt: DateTime
    }

    class CreateTribeRequest {
        +Name: string
        +Archetype: TribeArchetype
        +DamageBonus: number
        +AttackSpeedBonus: number
        +RangeBonus: number
        +ColorTheme: string
        +Validate(): ValidationResult
    }

    class TribeResponse {
        +Id: UUID
        +Name: string
        +Archetype: TribeArchetype
        +Bonuses: BonusStats
        +ColorTheme: string
    }

    class CreateMapRequest {
        +Name: string
        +Description: string
        +Width: number
        +Height: number
        +TerrainData: Tile[]
        +SpawnPoint: Point
        +GoalPoint: Point
        +WaveConfig: WaveConfiguration
        +Validate(): ValidationResult
    }

    class MapResponse {
        +Id: UUID
        +Name: string
        +Description: string
        +Dimensions: Dimensions
        +Author: string
        +PlayCount: number
        +Rating: number
        +CreatedAt: DateTime
    }

    class JoinSessionRequest {
        +SessionId: UUID
        +TribeId: UUID
    }

    class GameSessionResponse {
        +Id: UUID
        +MapId: UUID
        +MapName: string
        +Players: PlayerInfo[]
        +Status: SessionStatus
        +CurrentWave: number
        +SharedLives: number
    }

    class PlaceTowerRequest {
        +TowerType: TowerType
        +Position: Point
    }

    class TowerResponse {
        +Id: UUID
        +Type: TowerType
        +Position: Point
        +Level: number
        +Stats: TowerStats
        +OwnerId: UUID
    }
```

## Database ERD

```mermaid
erDiagram
    USERS ||--o{ TRIBES : owns
    USERS {
        uuid id PK
        varchar email UK
        varchar username UK
        varchar password_hash
        int level
        int total_games_played
        timestamp created_at
        timestamp last_login
    }

    TRIBES {
        uuid id PK
        uuid user_id FK
        varchar name
        varchar archetype
        decimal damage_bonus
        decimal attack_speed_bonus
        decimal range_bonus
        varchar color_theme
        timestamp created_at
    }

    USERS ||--o{ MAPS : creates
    MAPS {
        uuid id PK
        uuid user_id FK
        varchar name
        text description
        int width
        int height
        jsonb terrain_data
        jsonb spawn_point
        jsonb goal_point
        jsonb wave_config
        boolean is_public
        int play_count
        decimal rating
        timestamp created_at
    }

    MAPS ||--o{ GAME_SESSIONS : uses
    GAME_SESSIONS {
        uuid id PK
        uuid map_id FK
        varchar status
        int current_wave
        int shared_lives
        jsonb game_state
        timestamp created_at
        timestamp completed_at
    }

    GAME_SESSIONS ||--o{ SESSION_PLAYERS : contains
    USERS ||--o{ SESSION_PLAYERS : participates
    TRIBES ||--o{ SESSION_PLAYERS : uses
    SESSION_PLAYERS {
        uuid id PK
        uuid session_id FK
        uuid user_id FK
        uuid tribe_id FK
        int gold
        boolean is_host
        boolean is_ready
        boolean is_connected
        timestamp joined_at
    }

    GAME_SESSIONS ||--o{ TOWERS : has
    SESSION_PLAYERS ||--o{ TOWERS : places
    TOWERS {
        uuid id PK
        uuid session_id FK
        uuid owner_id FK
        varchar type
        jsonb position
        int level
        decimal damage
        decimal attack_speed
        decimal range
        varchar target_mode
        timestamp created_at
    }

    GAME_SESSIONS ||--o{ ENEMIES : tracks
    ENEMIES {
        uuid id PK
        uuid session_id FK
        varchar type
        jsonb position
        jsonb path
        decimal health
        decimal max_health
        decimal speed
        int gold_reward
        boolean is_targeting_tower
        uuid target_tower_id FK
        timestamp spawned_at
    }
```

## iDesign Layer Mapping

**Managers (🟢 Orchestration Layer)**:
- `GameSessionManager`: Coordinates game session lifecycle, validates player joins
- `MultiplayerManager`: Handles WebSocket synchronization, state broadcasting
- `MapManager`: Orchestrates map creation, validation, publishing
- `TribeManager`: Manages tribe CRUD operations, validates stats

**Engines (🟠 Business Logic Layer)**:
- `PathfindingEngine`: A* pathfinding, path validation, recalculation logic
- `TowerEngine`: Tower placement validation, attack processing, stat calculations
- `EnemyAIEngine`: Enemy spawning, movement, tower targeting behavior
- `PhysicsEngine`: Position updates, collision detection, grid transformations

**Data Access (⚫ Persistence Layer)**:
- `UserRepository`: User CRUD with Prisma ORM
- `MapRepository`: Map storage, terrain data serialization
- `TribeRepository`: Tribe persistence
- `GameSessionRepository`: Session state (Redis hot cache + PostgreSQL persistence)

**Models (🟣 Data Contracts)**:
- Domain Entities: `User`, `Tribe`, `Map`, `Tower`, `Enemy`, `GameSession`
- DTOs: `CreateUserRequest`, `TribeResponse`, `GameSessionResponse`
- Enums: `TowerType`, `EnemyType`, `TerrainType`, `SessionStatus`

## Validation Checklist

**Domain Model**:
- [x] Entities map to business concepts from use cases
- [x] Relationships reflect business rules (User owns Tribes, Map contains Tiles)
- [x] Enums define valid states (TowerType, SessionStatus, TerrainType)

**Service Design**:
- [x] Managers coordinate use cases (GameSessionManager orchestrates UC-001)
- [x] Interfaces define clear contracts (IPathfindingEngine, ITowerEngine)
- [x] Dependencies flow inward (Managers → Engines → Data Access)

**Data Design**:
- [x] Repositories abstract persistence (Prisma + Redis)
- [x] DTOs provide clean API contracts (CreateTribeRequest, TowerResponse)
- [x] ERD supports all use cases with proper foreign keys

**Instructions**: All classes support identified use cases. Pathfinding, tower placement, multiplayer sync, map building, and tribe customization fully modeled. Ready for implementation.

[<< Back](./design.md)
