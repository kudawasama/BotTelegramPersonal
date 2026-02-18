# 🎮 HOJA DE RUTA - EXPANSIÓN DEL SISTEMA RPG

## 📋 ÍNDICE
1. [Fase 0: Corrección Inmediata](#fase-0)
2. [Fase 1: Mejoras de Combate](#fase-1)
3. [Fase 2: Sistema de Mapas y Zonas](#fase-2)
4. [Fase 3: Sistema de Mazmorras](#fase-3)
5. [Fase 4: Reestructuración de Clases](#fase-4)
6. [Fase 5: Contenido Adicional](#fase-5)

---

## <a name="fase-0"></a>🔧 FASE 0: CORRECCIÓN INMEDIATA (1-2 horas)

### Problema Actual
Al invocar esqueleto o cualquier minion, NO se muestra información sobre la invocación:
```
✨ Invocar Esqueleto
⚔️ Bandido contraataca
```

**Información faltante:**
- Nombre del minion invocado
- HP del minion
- Turnos restantes
- Estado (controlado/no controlado)
- Descripción y habilidades

### Solución
Agregar campo `SkillDetails` al CombatResult para mostrar info adicional de skills.

**Modificaciones:**
1. `CombatResult.cs`: Agregar `public string? SkillDetails { get; set; }`
2. `RpgCombatService.Actions.cs`: Asignar detalles de invocación a `result.SkillDetails`
3. `RpgCombatService.cs`: Incluir `SkillDetails` en `GetCombatNarrative()`

**Impacto:** ⏱️ 1 hora | 🔨 Bajo | 📈 Alta visibilidad

---

## <a name="fase-1"></a>⚔️ FASE 1: MEJORAS DE COMBATE (3-4 horas)

### 1.1 Sistema de Probabilidades para Invocaciones
**Actualmente:** Invocación siempre exitosa (si hay recursos)

**Propuesta:**
```csharp
// Probabilidad base según tipo de minion
Skeleton:  85% éxito
Zombie:    75% éxito
Ghost:     65% éxito
Elemental: 70% éxito
Lich:      50% éxito
Horror:    40% éxito

// Modificadores:
+ Inteligencia/10% (máx +15%)
+ Sabiduría/15% (máx +10%)
+ Pasiva "Necromancer Mastery": +20%
- Enemy Level vs Player Level
```

**Feedback en combate:**
```
✨ Invocar Esqueleto
🎲 Probabilidad: 92.5% | Roll: 34.8%
✅ ¡Invocación exitosa!

💀 Esqueleto Guerrero invocado
   💚 85/85 HP | ⏱️ 5 turnos
   ✅ CONTROLADO
   ⚔️ "Un guerrero caído resucitado"
   ✨ Ataque físico (+25% daño)
```

### 1.2 Minions en Acción
**Mejorar feedback actual:**
```
━━━━━━━━━━━━━━━
💀 ESBIRROS ACTIVOS

💀 Esqueleto #1: ⚔️ 42 daño
💀 Esqueleto #2: ⚔️ 38 daño ⚡ CRÍTICO
🧟 Zombie: 🎯 Falla (Roll: 78% vs 65%)

📊 Daño total esbirros: 80
```

### 1.3 Detalles de Acciones
**Expandir narrativa de todas las acciones:**

**Meditar:**
```
🧘 Meditar
💙 +45 Mana (48% restaurado)
🎯 Probabilidad enemy: 88.5% | Roll: 62.3%
⚠️ Vulnerable: -15 defensa este turno

⚔️ Bandido aprovecha tu vulnerabilidad
🩸 Daño: 47 (+12 por vulnerabilidad)
```

**Esquivar:**
```
💨 Esquivar
⚡ +15% evasión este turno
⏱️ Iniciativa: +5

⚔️ Bandido intenta atacar
🎯 Probabilidad: 81.5% → 66.5% (evasión)
Roll: 88.2%
❌ ¡El ataque falla!
```

---

## <a name="fase-2"></a>🗺️ FASE 2: SISTEMA DE MAPAS Y ZONAS (8-10 horas)

### 2.1 Arquitectura de Mundo

```
MUNDO ABIERTO
├─ Regiones (5-8)
│  ├─ Bosque de Olden
│  ├─ Montañas Heladas
│  ├─ Desierto Carmesí
│  ├─ Pantanos Malditos
│  └─ Fortaleza Oscura
│
└─ Zonas por Región (3-5)
   ├─ Campamento Base (spawn)
   ├─ Zona de Caza (Lv 1-5)
   ├─ Ruinas Antiguas (Lv 5-10)
   ├─ Entrada Mazmorra (Lv 10+)
   └─ Ciudad/Santuario
```

### 2.2 Sistema de Movimiento

**Modelo de Datos:**
```csharp
public class GameRegion
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int RecommendedLevel { get; set; }
    public List<GameZone> Zones { get; set; }
    public string Emoji { get; set; }
}

public class GameZone
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    public List<string> ConnectedZones { get; set; } // IDs
    public ZoneType Type { get; set; } // Town, Hunting, Dungeon, Boss
    public double EncounterRate { get; set; } // 0.0 - 1.0
    public List<string> PossibleEnemies { get; set; }
    public bool HasShop { get; set; }
    public bool IsSafeZone { get; set; }
}
```

**Comandos:**
```
/map     - Mapa de región actual
/travel  - Viajar a otra zona
/explore - Explorar zona actual
```

**UI de Movimiento:**
```
📍 UBICACIÓN ACTUAL
🌲 Bosque de Olden - Claro del Cazador

"Un claro tranquilo rodeado de árboles antiguos.
Ocasionalmente, criaturas salvajes pasan por aquí."

⚔️ Nivel recomendado: 3-7
🎲 Encuentros: 35% por exploración
🛡️ Zona segura: No

━━━━━━━━━━━━━━━
🗺️ ZONAS CONECTADAS:

→ 🏕️ Campamento Base (Sur) - Lv 1
→ 🌲 Bosque Profundo (Norte) - Lv 5-10
→ 🏚️ Ruinas Olvidadas (Este) - Lv 8-12
→ 🌉 Puente del Troll (Oeste) - Lv 6-9

━━━━━━━━━━━━━━━
[⚔️ Explorar] [🗺️ Viajar] [🏠 Menú RPG]
```

### 2.3 Sistema de Exploración

```csharp
public class ExplorationResult
{
    public enum ResultType
    {
        Combat,         // Encuentro con enemigo
        Treasure,       // Cofre/recurso
        Event,          // Evento random
        Nothing,        // Nada encontrado
        DungeonEntrance // Descubrió mazmorra
    }
    
    public ResultType Type { get; set; }
    public string Message { get; set; }
    public RpgEnemy? Enemy { get; set; }
    public RpgItem? Treasure { get; set; }
    public Dungeon? DiscoveredDungeon { get; set; }
}
```

**Probabilidades de Exploración:**
```
Zona Lv 1-5:
- Combat: 50%
- Treasure: 15%
- Event: 10%
- Nothing: 24%
- Dungeon: 1%

Zona Lv 5-10:
- Combat: 60%
- Treasure: 10%
- Event: 12%
- Nothing: 15%
- Dungeon: 3%

Zona Lv 10+:
- Combat: 70%
- Treasure: 8%
- Event: 8%
- Nothing: 9%
- Dungeon: 5%
```

---

## <a name="fase-3"></a>🏰 FASE 3: SISTEMA DE MAZMORRAS (12-15 horas)

### 3.1 Arquitectura de Mazmorras

```csharp
public class Dungeon
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int MinLevel { get; set; }
    public int TotalFloors { get; set; } // 5-25
    public int CurrentFloor { get; set; }
    public DungeonDifficulty Difficulty { get; set; }
    public bool RequiresKey { get; set; }
    public List<DungeonFloor> Floors { get; set; }
    public DungeonRewards FinalRewards { get; set; }
}

public enum DungeonDifficulty
{
    Common,      // 5 pisos, Lv 5+
    Uncommon,    // 8 pisos, Lv 10+
    Rare,        // 12 pisos, Lv 15+
    Epic,        // 18 pisos, Lv 20+
    Legendary    // 25 pisos, Lv 25+
}

public class DungeonFloor
{
    public int FloorNumber { get; set; }
    public FloorType Type { get; set; } // Combat, Elite, Trap, Rest, Boss
    public RpgEnemy? Enemy { get; set; }
    public FloorReward? Reward { get; set; }
    public bool IsCleared { get; set; }
}
```

### 3.2 Mecánicas de Mazmorra

**Restricciones:**
- ❌ No se puede salir hasta completar o morir
- ❌ No se puede descansar fuera de "Rest Floors"
- ❌ HP/Mana/Stamina NO se restauran automáticamente
- ✅ Se pueden usar items
- ✅ Se puede cambiar equipment
- ✅ Los minions persisten entre pisos

**Tipos de Pisos:**
```
🗡️ Combat Floor (60%):
   - 1-3 enemigos normales
   - Recompensa: Oro, Items comunes

⚔️ Elite Floor (20%):
   - 1 enemigo élite (HP +50%, Stats +30%)
   - Recompensa: Items raros, Oro x2

💀 Boss Floor (cada 5 pisos):
   - 1 jefe poderoso
   - Recompensa: Equipment legendario, Skill point

😴 Rest Floor (10%):
   - Restaura 50% HP/Mana/Stamina
   - Puede guardar progreso

🪤 Trap Floor (10%):
   - Evento de trampa
   - Pierdes HP o recursos
   - Posible recompensa si superas
```

### 3.3 Sistema de Llaves

```csharp
public class DungeonKey
{
    public string Id { get; set; }
    public DungeonDifficulty UnlocksDifficulty { get; set; }
    public bool IsConsumed { get; set; } // Se consume al entrar
}
```

**Obtención de Llaves:**
- 🔑 Common Key: Drop de jefes Lv 5+ (15%)
- 🗝️ Uncommon Key: Drop de jefes Lv 10+ (10%)
- 🔐 Rare Key: Drop de jefes Lv 15+ (7%)
- 🎖️ Epic Key: Drop de jefes Lv 20+ (5%)
- 👑 Legendary Key: Drop de jefes Lv 25+ (3%)

### 3.4 UI de Mazmorra

```
🏰 MAZMORRA: Cripta Olvidada
━━━━━━━━━━━━━━━━━━━━━━
🎖️ Dificultad: EPIC (18 pisos)
📍 Piso actual: 7/18
⚔️ Nivel recomendado: 20+

━━━━━━━━━━━━━━━━━━━━━━
👤 Kudawa
💚 HP: 145/220 (66%)
💙 Mana: 78/150 (52%)
💛 Stamina: 42/80 (53%)
⚡ Energía: 0/100 (recupera al salir)

💀 Esbirros: 2/3
   💀 Esqueleto #1: 65/85 HP
   🧟 Zombie: 102/120 HP

━━━━━━━━━━━━━━━━━━━━━━
📊 PROGRESO:
✅ Pisos 1-6 completados
🔶 Piso 7 (ACTUAL): 🗡️ Combat Floor
⬜ Pisos 8-10: ???
💀 Piso 10: BOSS FLOOR
⬜ Pisos 11-18: ???

━━━━━━━━━━━━━━━━━━━━━━
[⚔️ Avanzar] [🎒 Inventario] [📊 Stats]
               [❌ Rendirse (Pierdes todo)]
```

**Durante Combate:**
```
🏰 CRIPTA OLVIDADA - Piso 7/18
⚔️ COMBATE - Turno 3

✨ Invocar Zombie
🎲 Probabilidad: 78.5% | Roll: 45.2%
✅ ¡Invocación exitosa!

🧟 Zombie Putrefacto invocado
   💚 120/120 HP | ⏱️ 6 turnos
   ⚠️ NO CONTROLADO (50% obediencia)
   🩸 "Un cadáver reanimado con sed..."
   ✨ Veneno: 10% chance por ataque

━━━━━━━━━━━━━━━━━━━━━
💀 ESBIRROS ACTIVOS (3/3):
💀 Esqueleto: ⚔️ 38 daño
🧟 Zombie #1: ⚔️ 52 daño + 🩸 Veneno
🧟 Zombie #2: ⚠️ Desobedece (Roll: 72% > 50%)

━━━━━━━━━━━━━━━━━━━━━
⚔️ Guardia Esquelético contraataca
...
```

### 3.5 Recompensas Finales

```
🏆 MAZMORRA COMPLETADA
🏰 Cripta Olvidada (Epic)
━━━━━━━━━━━━━━━━━━━━━━

📊 ESTADÍSTICAS:
⚔️ Enemigos derrotados: 42
💀 Jefes vencidos: 4
⏱️ Tiempo: 1h 23m
💎 Perfección: 18/18 pisos
⭐ Puntuación: S

━━━━━━━━━━━━━━━━━━━━━━
🎁 RECOMPENSAS:

💰 Oro: 8,500
✨ XP: 2,400 (+20% bonus perfección)
🎖️ Skill Point: +1

🎒 EQUIPMENT:
⚔️ Espada Maldita (Legendary)
   +85 Atk | +15% Crit | -10 HP/turno
   
🛡️ Armadura de las Sombras (Epic)
   +62 Def | +25% Evasión | Sigilo +2

💍 Anillo del Nigromante (Rare)
   +30 INT | Minions +1 slot | -20% mana cost

━━━━━━━━━━━━━━━━━━━━━━
🔑 LLAVE ESPECIAL:
👑 Legendary Dungeon Key x1

[✅ Reclamar] [📊 Ver Ranking Mazmorras]
```

---

## <a name="fase-4"></a>🎭 FASE 4: REESTRUCTURACIÓN DE CLASES (6-8 horas)

### 4.1 Concepto
**Actualmente:** 4 clases base (Warrior, Mage, Rogue, Cleric) - Sin requisitos

**Nuevo Sistema:** TODAS las clases son desbloqueables mediante acciones

### 4.2 Clases de Inicio

```
Cuando creas personaje:
━━━━━━━━━━━━━━━━━━━━━━
👤 CREAR PERSONAJE

Elige tu nombre:
[Kudawa]

🎭 ESPECIALIZACIÓN INICIAL:
Todas las clases están bloqueadas.
Tu personaje comenzará como "Aventurero"
y desbloqueará clases según tus acciones.

----

👤 AVENTURERO (Clase Base)
━━━━━━━━━━━━━━━━━━━━━━
"Un viajero sin especialización.
Stats balanceados, sin bonificaciones."

📊 Stats Iniciales:
STR: 10 | INT: 10 | DEX: 10
CON: 10 | WIS: 10 | CHA: 10

❤️ HP: 100 | 💙 Mana: 50
💛 Stamina: 50 | ⚡ Energía: 100

✨ Skills: Ninguna
💎 Pasivas: Ninguna

━━━━━━━━━━━━━━━━━━━━━━
💡 DESBLOQUEA CLASES:
- Realiza 100 ataques físicos → ⚔️ Warrior
- Lanza 100 hechizos → 🔮 Mage
- Esquiva 80 ataques → 🗡️ Rogue
- Cura 1000 HP → ✨ Cleric

[✅ Comenzar Aventura]
```

### 4.3 Sistema de Clases Desbloqueables

```csharp
public class CharacterClass
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ClassTier Tier { get; set; } // Basic, Advanced, Master, Hidden
    public Dictionary<string, int> RequiredActions { get; set; }
    public List<string> RequiredClasses { get; set; } // Clases previas necesarias
    public StatsBonus Bonuses { get; set; }
    public List<string> UnlockedSkills { get; set; }
    public List<string> GrantedPassives { get; set; }
}

public enum ClassTier
{
    Basic,    // Clases base (Warrior, Mage, etc)
    Advanced, // Requieren clase básica + acciones
    Master,   // Requieren clase avanzada + muchas acciones
    Hidden    // Sistema actual de hidden classes
}
```

### 4.4 Árbol de Clases

```
AVENTURERO (Inicio)
    ↓
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
TIER 1: CLASES BÁSICAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

⚔️ WARRIOR (100 ataques físicos)
├→ 🛡️ PALADIN (500 ataques + 300 defensas + curar 2000 HP)
├→ 🪓 BERSERKER (800 ataques + 200 críticos + recibir 5000 daño)
└→ ⚔️ DUAL BLADE (600 ataques + 400 esquivas + 150 contras)

🔮 MAGE (100 hechizos mágicos)
├→ 🔥 PYROMANCER (500 hechizos + 300 fuego + 5000 mana gastado)
├→ ❄️ CRYOMANCER (500 hechizos + 300 hielo + 50 enemigos congelados)
└→ ⚡ STORMMAGE (500 hechizos + 300 rayo + 40 aturdimientos)

🗡️ ROGUE (80 esquivas)
├→ 🔪 ASSASSIN (400 esquivas + 300 críticos + 150 kills stealth)
├→ 🃏 TRICKSTER (500 esquivas + 200 engaños + 100 robos)
└→ 🏹 RANGER (400 esquivas + 500 ataques precisos + 200 kills a distancia)

✨ CLERIC (curar 1000 HP)
├→ 💫 HIGH PRIEST (curar 10000 HP + 200 buffs + 50 resurrecciones)
├→ 🛡️ GUARDIAN (curar 5000 HP + 500 defensas + absorber 10000 daño)
└→ 🌟 ORACLE (curar 8000 HP + 300 observaciones + 100 predicciones)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
TIER 2: CLASES AVANZADAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

💀 NECROMANCER (Mage  + 1000 dark magic + 800 summon_undead)
🌊 ELEMENTAL SAGE (Pyro + Cryo + Storm + 1000 combo spells)
⚔️ BLADE DANCER (Dual Blade + Assassin + 500 combo_10x)
🐺 BEAST MASTER (Ranger + 200 tames + 100 pet commands)

... (sistema actual hidden classes)
```

### 4.5 Cambio de Clase

```
📋 GESTIÓN DE CLASES
━━━━━━━━━━━━━━━━━━━━━━

👤 CLASE ACTIVA:
⚔️ Warrior (Nivel 12)
Bonos: +10 STR, +5 CON
Skills: 4/8 desbloqueadas

━━━━━━━━━━━━━━━━━━━━━━
✅ CLASES DESBLOQUEADAS:

⚔️ Warrior (ACTIVA)
🔮 Mage
🗡️ Rogue

⏳ EN PROGRESO:
🛡️ Paladin (78/300 defensas)
🔥 Pyromancer (412/500 hechizos)
💀 Necromancer (650/1000 dark magic)

🔒 BLOQUEADAS:
✨ Cleric (0/1000 HP curado)
🪓 Berserker (345/800 ataques)
... (ver lista completa)

━━━━━━━━━━━━━━━━━━━━━━
[🔄 Cambiar Clase] [📊 Ver Progreso]
```

---

## <a name="fase-5"></a>🌟 FASE 5: CONTENIDO ADICIONAL (Variable)

### 5.1 Sistema de Facciones
- Reputación con diferentes grupos
- Misiones exclusivas por facción
- Conflictos entre facciones

### 5.2 Crafteo y Mejora de Equipment
- Craftear items con materiales
- Mejorar equipment existente (+1, +2, +3...)
- Encantar con propiedades especiales

### 5.3 PvP Arena
- Duelos 1v1
- Rankings competitivos
- Recompensas semanales

### 5.4 Eventos Temporales
- Invasiones de jefes mundiales
- Eventos estacionales
- Mazmorras limitadas

### 5.5 Sistema de Logros
- 100+ logros desbloqueables
- Títulos y recompensas
- Progreso visible en perfil

---

## 📊 PRIORIZACIÓN Y TIEMPOS

### Ruta Crítica (Orden Recomendado)
1. ✅ **Fase 0**: Corrección invocaciones (1-2h) - **URGENTE**
2. ⚔️ **Fase 1**: Mejoras combate (3-4h) - **ALTA PRIORIDAD**
3. 🗺️ **Fase 2**: Mapas y zonas (8-10h) - **MEDIA-ALTA**
4. 🏰 **Fase 3**: Mazmorras (12-15h) - **MEDIA**
5. 🎭 **Fase 4**: Clases (6-8h) - **MEDIA-BAJA**
6. 🌟 **Fase 5**: Contenido extra (Variable) - **BAJA**

### Tiempo Total Estimado
- **Núcleo esencial** (Fases 0-2): ~12-16 horas
- **Experiencia completa** (Fases 0-4): ~30-39 horas
- **Todo el contenido** (Fases 0-5): 50+ horas

---

## 🎯 PRÓXIMOS PASOS INMEDIATOS

1. **Aprobar esta hoja de ruta** o solicitar cambios
2. **Comenzar Fase 0**: Fix de invocaciones (1-2h)
3. **Decidir si continuar** con Fase 1 o saltar a Fase 2/3
4. **Iteración progresiva**: Implementar, probar, mejorar

¿Quieres que comience con la **Fase 0** ahora?
