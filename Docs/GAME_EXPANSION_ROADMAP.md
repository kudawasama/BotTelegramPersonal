# 🎮 HOJA DE RUTA - EXPANSIÓN DEL SISTEMA RPG

**Última actualización:** 18 de febrero de 2026  
**Versión:** 2.0 - Refactorización Mayor

## 📊 PROGRESO GENERAL
```
✅ Fase 0: Corrección Invocaciones         [██████████] 100%
✅ Fase 1: Mejoras de Combate             [██████████] 100%
✅ Fase 2: Sistema de Mapas y Zonas       [██████████] 100%
⏸️ Fase 3: Sistema de Mazmorras          [░░░░░░░░░░]   0%
⏸️ Fase 3.5: Leveling Mascotas/Minions   [░░░░░░░░░░]   0%
⏸️ Fase 4: Reestructuración de Clases    [░░░░░░░░░░]   0%
🔄 Fase 5: Refactorización UI/UX          [██████░░░░]  60% ← EN PROGRESO (Menús ✅)
⏸️ Fase 6: Máquina de Estados FSM        [░░░░░░░░░░]   0%
⏸️ Fase 7: Generación de Imágenes        [░░░░░░░░░░]   0%
⏸️ Fase 8: Telegram Mini App              [░░░░░░░░░░]   0%
⏸️ Fase 9: IA Narrativa (Dungeon Master) [░░░░░░░░░░]   0%
```

## 📋 ÍNDICE
1. [✅ Fase 0: Corrección Inmediata](#fase-0) - COMPLETADA
2. [✅ Fase 1: Mejoras de Combate](#fase-1) - COMPLETADA
3. [✅ Fase 2: Sistema de Mapas y Zonas](#fase-2) - COMPLETADA
4. [Fase 3: Sistema de Mazmorras](#fase-3)
5. [Fase 3.5: Leveling de Mascotas/Minions](#fase-3-5)
6. [Fase 4: Reestructuración de Clases](#fase-4)
7. [🔥 Fase 5: Refactorización UI/UX (CRÍTICA)](#fase-5)
8. [Fase 6: Máquina de Estados Finita](#fase-6)
9. [Fase 7: Generación de Imágenes Dinámicas](#fase-7)
10. [Fase 8: Telegram Mini App](#fase-8)
11. [Fase 9: IA Narrativa (Dungeon Master)](#fase-9)

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

## <a name="fase-3-5"></a>🐾 FASE 3.5: LEVELING DE MASCOTAS/MINIONS (4-6 horas)

### 3.5.1 Sistema de XP para Mascotas

**Modelo Actualizado:**
```csharp
public class RpgPet
{
    // Existing properties...
    public int Experience { get; set; }
    public int ExperienceToNextLevel => Level * 100;
    public int CombatsParticipated { get; set; }
    public int DamageDealt { get; set; }
    public int KillsEarned { get; set; }
    public int BossesDefeated { get; set; }
}
```

**Formas de ganar XP:**
- 🎯 **Combate Activo** (50 XP): Por participar en el combate
- 💀 **Kill Enemigo** (100 XP): Si la mascota da el golpe final
- 👑 **Boss Kill** (500 XP): Participar en matar un jefe
- 🗺️ **Exploración** (15 XP): Por estar equipada durante exploración
- ⚒️ **Entrenamiento** (`/train <pet>`, 100 oro → 200 XP)

**Bonificaciones por Nivel:**
```
Lv 5:  +5% stats
Lv 10: +10% stats + nueva habilidad
Lv 15: +15% stats
Lv 20: +20% stats + evolución de habilidad
Lv 25: +30% stats + habilidad única
Lv 30: +50% stats + transformación especial
```

### 3.5.2 Sistema de XP para Minions

**Modelo Actualizado:**
```csharp
public class Minion
{
    // Existing properties...
    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public int CombatsServived { get; set; }
    public int TotalDamageDealt { get; set; }
    public int Kills { get; set; }
    public bool IsPermanent { get; set; } // Guardado entre combates
}
```

**Formas de ganar XP:**
- 🛡️ **Supervivencia** (30 XP): Por no morir en un combate
- ⚔️ **Daño Infligido** (1 XP por cada 10 de daño)
- 💀 **Kill Obtenido** (150 XP): Si el minion mata al enemigo
- 👑 **Boss Participación** (300 XP): Si participa contra jefe

**Persistencia de Minions:**
- Los minions ahora se guardan entre combates (como compañeros permanentes)
- Máximo 3 minions activos permanentes
- Al invocar uno nuevo con slots llenos, debe "retirar" uno existente
- Comando `/minions` para ver stats y gestionar el equipo

**Bonificaciones por Nivel:**
```
Cada nivel: +10% HP, +5% ataque
Lv 5:  Habilidad mejorada
Lv 10: Segunda habilidad
Lv 15: +50% duración (más turnos)
Lv 20: Evolución (cambia de tipo/apariencia)
```

### 3.5.3 UI de Compañeros

**Comando `/companions`:**
```
🐾 **TUS COMPAÑEROS**

━━━━━━━━━━━━━━━━━━━━━
🦊 MASCOTAS ACTIVAS
━━━━━━━━━━━━━━━━━━━━━

🦊 Zorro Rojo ⭐ Lv.12
   💚 HP: 450/450
   💪 Atk: 85 | 🛡️ Def: 40
   ✨ XP: 850/1200
   🎯 Combates: 45 | 💀 Kills: 23
   👑 Jefes: 3 | 🏆 Boss: 3

━━━━━━━━━━━━━━━━━━━━━
💀 ESBIRROS PERMANENTES
━━━━━━━━━━━━━━━━━━━━━

💀 Esqueleto Guerrero ⭐ Lv.8
   💚 HP: 320/320
   ⚔️ Atk: 65 | 🛡️ Def: 25
   ✨ XP: 450/800
   ⏱️ Supervivencias: 15 | 💀 Kills: 8

👻 Espectro Guardián ⭐ Lv.5
   💚 HP: 200/200
   🔮 Mag: 90 | ⚡ Spd: 70
   ✨ XP: 200/500
   💀 Kills: 8 | 👑 Boss: 1

━━━━━━━━━━━━━━━━━━━━━
[⚒️ Entrenar] [👁️ Ver Detalles] [🔄 Gestionar]
```

**Archivos a Modificar:**
- `src/BotTelegram/RPG/Models/RpgPet.cs`
- `src/BotTelegram/RPG/Models/Minion.cs`
- `src/BotTelegram/RPG/Services/RpgCombatService.cs`
- `src/BotTelegram/RPG/Commands/CompanionsCommand.cs` (nuevo)

---

## <a name="fase-5"></a>🔥 FASE 5: REFACTORIZACIÓN UI/UX (CRÍTICA) (10-12 horas)

**⚠️ PRIORIDAD CRÍTICA** - Basado en auditoría de UX

### 5.1 Problema Identificado

**Issues Actuales:**
1. ❌ **21 botones simultáneos** → Sobrecarga cognitiva
2. ❌ **Teclado ocupa 60% de pantalla** → Scroll constante
3. ❌ **Nuevo mensaje por acción** → Spam en chat
4. ❌ **ReplyKeyboardMarkup** → Sin edición en tiempo real

### 5.2 Arquitectura Jerárquica de Menús

**Diseño Nuevo (4 Categorías Madre):**
```
🏠 MENÚ PRINCIPAL
┌─────────────────────┐
│ ⚔️ Aventura         │
│ 👤 Personaje        │
│ 🏘️ Ciudad           │
│ ⚙️ Ayuda            │
└─────────────────────┘

⚔️ AVENTURA
┌─────────────────────┐
│ 🎯 Combate          │
│ 🗺️ Explorar         │
│ 🏰 Mazmorra         │
│ 🔙 Volver           │
└─────────────────────┘

👤 PERSONAJE
┌─────────────────────┐
│ 📊 Stats            │
│ 🎒 Inventario       │
│ ✨ Skills           │
│ 🐾 Compañeros       │
│ 🎭 Clases           │
│ 🔙 Volver           │
└─────────────────────┘

🏘️ CIUDAD
┌─────────────────────┐
│ 🛒 Tienda           │
│ ⚒️ Herrería         │
│ 🏛️ Gremio           │
│ 🏆 Rankings         │
│ 🔙 Volver           │
└─────────────────────┘
```

**Beneficio:** Máximo 6 botones por pantalla, navegación intuitiva

### 5.3 Single Message Interaction (SMI)

**Concepto:**
En lugar de enviar múltiples mensajes, **editar un solo mensaje** en tiempo real.

**Ejemplo - ANTES:**
```
[MSG 1] ⚔️ Atacas al Goblin (45 daño)
[MSG 2] 🩸 Goblin contraataca (32 daño)
[MSG 3] ⚔️ Atacas al Goblin (51 daño)
[MSG 4] ⚔️ Goblin muere. +120 XP
[MSG 5] 💰 Loot: 85 oro
```

**Ejemplo - DESPUÉS:**
```
[EDICIÓN EN TIEMPO REAL DEL MISMO MENSAJE]

⚔️ **COMBATE EN CURSO**
━━━━━━━━━━━━━━━━━━━━━━
👤 Kudawa Lv.23
   ❤️ ████████░░ 180/220 HP
   💙 ██████████ 95/95 Mana
   
🐗 Goblin Salvaje Lv.21
   ❤️ ██░░░░░░░░ 35/180 HP

━━━━━━━━━━━━━━━━━━━━━━
📜 COMBATE LOG:
   ⚔️ Atacaste (45 daño)
   🩸 Goblin contraataca (32 daño)
   ⚔️ Atacaste (51 daño)
   
[⚔️ Atacar] [🛡️ Defender] [✨ Skills] [🎒 Items]
```

**Implementación:**
```csharp
// Guardar MessageId del combate
var combatMessage = await bot.SendMessage(chatId, "Iniciando combate...");
player.ActiveCombatMessageId = combatMessage.MessageId;

// En cada turno, EDITAR en lugar de ENVIAR NUEVO
while (combat.IsActive)
{
    await bot.EditMessageText(
        chatId, 
        player.ActiveCombatMessageId,
        GenerateCombatView(combat),
        replyMarkup: GetCombatKeyboard()
    );
}
```

### 5.4 Transición a InlineKeyboardMarkup

**Cambiar de ReplyKeyboardMarkup → InlineKeyboardMarkup**

**Ventajas:**
- ✅ No ocupa espacio del teclado del usuario
- ✅ Desaparece al completar la acción
- ✅ Se puede editar dinámicamente
- ✅ Usa `CallbackData` para procesamiento limpio

**Ejemplo:**
```csharp
// ANTES (ReplyKeyboardMarkup)
var keyboard = new ReplyKeyboardMarkup(new[]
{
    new KeyboardButton[] { "⚔️ Atacar", "🛡️ Defender" },
    new KeyboardButton[] { "✨ Skills", "🎒 Items" }
})
{
    ResizeKeyboard = true
};

// DESPUÉS (InlineKeyboardMarkup)
var keyboard = new InlineKeyboardMarkup(new[]
{
    new[]
    {
        InlineKeyboardButton.WithCallbackData("⚔️ Atacar", "combat_attack"),
        InlineKeyboardButton.WithCallbackData("🛡️ Defender", "combat_defend")
    },
    new[]
    {
        InlineKeyboardButton.WithCallbackData("✨ Skills", "combat_skills"),
        InlineKeyboardButton.WithCallbackData("🎒 Items", "combat_items")
    }
});
```

### 5.5 Barras de Progreso Animadas

**Implementación:**
```csharp
public static string GenerateProgressBar(int current, int max, int length = 10)
{
    var percentage = (double)current / max;
    var filled = (int)(percentage * length);
    var empty = length - filled;
    
    var color = percentage > 0.7 ? "💚" : percentage > 0.3 ? "💛" : "❤️";
    
    return color + new string('█', filled) + new string('░', empty);
}

// Uso:
var hpBar = GenerateProgressBar(player.HP, player.MaxHP);
// Resultado: 💚████████░░
```

### 5.6 Plan de Refactorización

**Archivos a Modificar:**
1. `RpgCommand.cs` - Menú principal jerárquico
2. `CallbackQueryHandler.cs` - Procesar nuevos callbacks
3. `RpgCombatService.cs` - Single message combat
4. `MapCommand.cs` - InlineKeyboard
5. `TravelCommand.cs` - InlineKeyboard
6. Todos los comandos que usen ReplyKeyboardMarkup

**Tiempo estimado:** 10-12 horas  
**Impacto UX:** ⭐⭐⭐⭐⭐ CRÍTICO

---

## <a name="fase-6"></a>🧩 FASE 6: MÁQUINA DE ESTADOS FINITA (FSM) (8-10 horas)

### 6.1 Problema Actual

El código tiene múltiples `if/else` y `switch` gigantes que hacen difícil:
- Mantener el flujo del juego
- Validar acciones disponibles según contexto
- Agregar nuevas features sin romper lógica existente

### 6.2 Solución: State Machine

**Definición de Estados:**
```csharp
public enum GameState
{
    Idle,           // En menú principal
    Exploring,      // Explorando zona
    InCombat,       // En combate activo
    InDungeon,      // Dentro de mazmorra
    Shopping,       // En tienda
    Resting,        // Descansando en posada
    Crafting,       // Creando items
    TravelMenu,     // Viendo mapa/viajando
    PetManagement,  // Gestionando mascotas
    SkillsMenu      // Viendo/usando skills
}

public class PlayerState
{
    public GameState CurrentState { get; set; } = GameState.Idle;
    public Dictionary<GameState, List<string>> AllowedActions { get; set; }
    public Dictionary<GameState, List<GameState>> ValidTransitions { get; set; }
}
```

**Configuración:**
```csharp
public class StateManager
{
    private static readonly Dictionary<GameState, List<string>> AllowedCommands = new()
    {
        [GameState.Idle] = new() { "rpg_adventure", "rpg_character", "rpg_city", "rpg_map" },
        [GameState.InCombat] = new() { "combat_attack", "combat_defend", "combat_skills", "combat_items" },
        [GameState.Shopping] = new() { "shop_buy", "shop_sell", "shop_exit" },
        [GameState.InDungeon] = new() { "dungeon_advance", "dungeon_rest", "dungeon_use_item" }
    };
    
    public bool CanExecuteAction(RpgPlayer player, string action)
    {
        return AllowedCommands[player.State.CurrentState].Contains(action);
    }
    
    public bool TransitionTo(RpgPlayer player, GameState newState)
    {
        if (!ValidTransitions[player.State.CurrentState].Contains(newState))
            return false;
            
        player.State.CurrentState = newState;
        return true;
    }
}
```

**Beneficio:**
- ✅ Solo se muestran botones válidos para el estado actual
- ✅ No más "Este comando no está disponible en combate"
- ✅ Código más mantenible y escalable

**Archivos a Crear:**
- `src/BotTelegram/RPG/Models/GameState.cs`
- `src/BotTelegram/RPG/Services/StateManager.cs`

**Tiempo estimado:** 8-10 horas  
**Impacto Técnico:** ⭐⭐⭐⭐⭐ MUY ALTO

---

## <a name="fase-7"></a>🎨 FASE 7: GENERACIÓN DE IMÁGENES DINÁMICAS (12-15 horas)

### 7.1 Concepto

En lugar de solo texto, generar **tarjetas visuales** para:
- Stats del personaje
- Inventario (mostrar items con iconos)
- Combate (barras de vida animadas)
- Mapas (vista gráfica de zonas)

### 7.2 Tecnología: SkiaSharp

```csharp
using SkiaSharp;

public class CardGenerator
{
    public byte[] GenerateStatsCard(RpgPlayer player)
    {
        using var surface = SKSurface.Create(new SKImageInfo(800, 600));
        var canvas = surface.Canvas;
        
        // Fondo
        canvas.Clear(SKColors.DarkSlateGray);
        
        // Avatar (emoji grande)
        var avatarPaint = new SKPaint
        {
            TextSize = 120,
            IsAntialias = true,
            Color = SKColors.White
        };
        canvas.DrawText(player.Emoji, 50, 150, avatarPaint);
        
        // Nombre y Nivel
        var namePaint = new SKPaint
        {
            TextSize = 48,
            IsAntialias = true,
            Color = SKColors.Gold,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        canvas.DrawText($"{player.Name} - Lv.{player.Level}", 200, 100, namePaint);
        
        // Barras de HP/Mana/XP
        DrawProgressBar(canvas, 200, 150, 500, 30, player.HP, player.MaxHP, SKColors.Green);
        DrawProgressBar(canvas, 200, 200, 500, 30, player.Mana, player.MaxMana, SKColors.Blue);
        DrawProgressBar(canvas, 200, 250, 500, 30, player.XP, player.ExperienceToNextLevel, SKColors.Gold);
        
        // Stats en columnas
        DrawStats(canvas, 200, 320, player);
        
        return surface.Snapshot().Encode(SKEncodedImageFormat.Png, 90).ToArray();
    }
}
```

### 7.3 Ejemplos de Uso

**Stats:**
```
/stats → Envía imagen en lugar de texto
```

**Inventario:**
```
/inventory → Grid visual con iconos de items
```

**Combate:**
```
Durante combate → Imagen con barras animadas que bajan en tiempo real
```

**Beneficio:**
- ✅ Visual mucho más atractivo
- ✅ Más fácil de leer stats
- ✅ Se ve profesional (AAA quality)
- ✅ Compatible con Telegram (enviar como foto)

**Archivos a Crear:**
- `src/BotTelegram/RPG/Services/ImageGenerator.cs`
- `src/BotTelegram/RPG/Services/CombatVisualizer.cs`
- `src/BotTelegram/RPG/Services/InventoryRenderer.cs`

**Tiempo estimado:** 12-15 horas  
**Impacto UX:** ⭐⭐⭐⭐⭐ MUY ALTO  
**Prioridad:** BAJA (feature premium)

---

## <a name="fase-8"></a>📱 FASE 8: TELEGRAM MINI APP (TMA) (20-30 horas)

### 8.1 Concepto

Crear un **panel web interactivo** que se abre dentro de Telegram para:
- Gestión de inventario complejo (drag & drop)
- Mapa interactivo (clickeable)
- Árbol de skills visual
- Crafting con preview
- Leaderboards con filtros
- Dashboard de estadísticas

### 8.2 Stack Tecnológico

```
Frontend:  Blazor WebAssembly / React
Backend:   ASP.NET Core Web API (ya existente)
Database:  Actual sistema de JSON
Integration: Telegram.Bot.WebApp
```

### 8.3 Arquitectura

```
src/
├── BotTelegram/              (Existing)
│   ├── RPG/
│   └── API/                  (NEW - Web API endpoints)
│       ├── StatsController.cs
│       ├── InventoryController.cs
│       └── CombatController.cs
│
└── BotTelegram.WebApp/       (NEW - Blazor/React project)
    ├── Pages/
    │   ├── Dashboard.razor
    │   ├── Inventory.razor
    │   ├── Map.razor
    │   └── Skills.razor
    ├── Components/
    │   ├── StatCard.razor
    │   ├── ItemGrid.razor
    │   └── SkillTree.razor
    └── wwwroot/
        ├── css/
        └── js/
```

### 8.4 Funcionalidades

**Dashboard:**
- Vista general de personaje
- Gráficos de progresión
- Últimas actividades
- Quick actions

**Inventario Avanzado:**
- Drag & drop para equipar
- Filtros por tipo/rareza
- Comparación de items
- Vender múltiples items

**Mapa Interactivo:**
- Vista 2D del mundo
- Click para viajar
- Zonas descubiertas/bloqueadas
- Información de zonas al hover

**Árbol de Skills:**
- Visualización de dependencias
- Preview de skills
- Asignación de puntos
- Respec con costo

**Beneficio:**
- ✅ Experiencia de usuario AAA
- ✅ No limitado por UI de Telegram
- ✅ Funcionalidades avanzadas (drag & drop, animaciones)
- ✅ Abre dentro de Telegram sin salir

**Tiempo estimado:** 20-30 horas  
**Impacto UX:** ⭐⭐⭐⭐⭐ EXTREMO  
**Prioridad:** BAJA (proyecto avanzado)

---

## <a name="fase-9"></a>🤖 FASE 9: IA NARRATIVA (DUNGEON MASTER) (15-20 horas)

### 9.1 Concepto

La IA no solo chatea, sino que **narra dinámicamente** las consecuencias de las acciones del jugador.

### 9.2 Ejemplo

**ANTES:**
```
⚔️ Atacaste al goblin
🩸 45 de daño
❤️ Goblin: 75/120 HP
```

**DESPUÉS (con IA narrativa):**
```
⚔️ Tu espada corta el aire con un silbido. El goblin intenta 
esquivar pero es demasiado lento. La hoja se clava en su hombro, 
arrancándole un grito de dolor. Verde icor brota de la herida, 
manchando el suelo del bosque.

🩸 45 de daño ❤️ 75/120 HP

El goblin retrocede, furioso, blandiendo su daga oxidada con 
renovada ferocidad. Sus ojos amarillos brillan con odio.
```

### 9.3 Implementación

```csharp
using Microsoft.SemanticKernel;

public class NarrativeAI
{
    private readonly IKernel _kernel;
    
    public async Task<string> NarrateCombatAction(
        CombatAction action, 
        RpgPlayer player, 
        RpgEnemy enemy, 
        int damage)
    {
        var prompt = $@"
Eres un Dungeon Master épico al estilo de D&D. Narra en 2-3 líneas dramáticas:

CONTEXTO:
- Acción: {action.Name}
- Jugador: {player.Name} (Lv.{player.Level} {player.Class})
- Enemigo: {enemy.Name} (Lv.{enemy.Level})
- Daño causado: {damage}
- HP enemigo restante: {enemy.HP}/{enemy.MaxHP}

ESTILO:
- Descriptivo y cinematográfico
- Lenguaje medieval/fantasy
- Enfocado en la acción física
- Sin diálogo
- Máximo 3 líneas

NARRATIVA:";
        
        var result = await _kernel.InvokePromptAsync(prompt);
        return result.ToString();
    }
    
    public async Task<string> NarrateExploration(
        GameZone zone, 
        ExplorationResult result)
    {
        var prompt = $@"
Narra en 2-3 líneas el resultado de explorar {zone.Name}:

RESULTADO: {result.Type}
{(result.Enemy != null ? $"Enemigo encontrado: {result.Enemy.Name}" : "")}
{(result.Treasure != null ? $"Tesoro: {result.Treasure.Name}" : "")}

Descripción de zona: {zone.Description}

NARRATIVA:";
        
        var result = await _kernel.InvokePromptAsync(prompt);
        return result.ToString();
    }
}
```

### 9.4 Integración

**Combate:**
```csharp
// En RpgCombatService.cs
var narrative = await _narrativeAI.NarrateCombatAction(action, player, enemy, damage);
result.Message = narrative + $"\n\n🩸 {damage} daño ❤️ {enemy.HP}/{enemy.MaxHP} HP";
```

**Exploración:**
```csharp
// En ExplorationService.cs
var narrative = await _narrativeAI.NarrateExploration(zone, result);
result.Message = narrative + result.Message;
```

**Beneficio:**
- ✅ Cada combate es único e impredecible
- ✅ Inmersión narrativa total
- ✅ El jugador se siente en un D&D real
- ✅ Diferenciador competitivo total

**Archivos a Crear:**
- `src/BotTelegram/AI/NarrativeEngine.cs`
- `src/BotTelegram/AI/CombatNarrator.cs`
- `src/BotTelegram/AI/ExplorationNarrator.cs`

**Tiempo estimado:** 15-20 horas  
**Impacto UX:** ⭐⭐⭐⭐⭐ EXTREMO  
**Prioridad:** MEDIA (diferenciador competitivo)

---

## 📊 PRIORIZACIÓN Y TIEMPOS ACTUALIZADOS

### Ruta Crítica (Orden Recomendado)
1. ✅ **Fase 0**: Corrección invocaciones (1-2h) - **COMPLETADA**
2. ✅ **Fase 1**: Mejoras combate (3-4h) - **COMPLETADA**
3. ✅ **Fase 2**: Mapas y zonas (8-10h) - **COMPLETADA**
4. 🔥 **Fase 5**: Refactorización UI/UX (10-12h) - **CRÍTICA** ← **SIGUIENTE**
5. 🐾 **Fase 3.5**: Leveling mascotas/minions (4-6h) - **ALTA**
6. 🏰 **Fase 3**: Mazmorras (12-15h) - **MEDIA**
7. 🎭 **Fase 4**: Clases (6-8h) - **MEDIA**
8. 🧩 **Fase 6**: FSM (8-10h) - **MEDIA-BAJA**
9. 🤖 **Fase 9**: IA Narrativa (15-20h) - **MEDIA-BAJA**
10. 🎨 **Fase 7**: Imágenes (12-15h) - **BAJA** (opcional)
11. 📱 **Fase 8**: Mini App (20-30h) - **BAJA** (proyecto avanzado)

### Tiempo Total Estimado
- **Núcleo esencial** (Fases 0-2): ~12-16 horas ✅ **COMPLETADO**
- **Con UI mejorada** (+ Fase 5): ~22-28 horas
- **Con features core** (+ Fases 3, 3.5, 4): ~44-57 horas
- **Experiencia completa** (+ Fases 6, 9): ~67-87 horas
- **Todo el contenido** (+ Fases 7, 8): ~99-132 horas

---

## 🎯 PRÓXIMOS PASOS INMEDIATOS

### Fase 5 (UI/UX) - Desglose de Tareas

**Semana 1: Arquitectura de Menús (4-5h)** ✅ **COMPLETADO**
1. ✅ Diseñar estructura jerárquica de 4 categorías
2. ✅ Crear nuevos callbacks para navegación
3. ✅ Refactorizar RpgCommand.cs con menú principal
4. ✅ Implementar menús: Aventura, Personaje, Ciudad, Ayuda
5. ✅ Testing de navegación

**Semana 2: Single Message Interaction (3-4h)** ⏳ **PENDIENTE**
1. ⏳ Modificar RpgCombatService para guardar MessageId
2. ⏳ Implementar EditMessage en lugar de SendMessage
3. ⏳ Crear método GenerateCombatView()
4. ⏳ Testing de combate con edición en tiempo real

**Semana 3: Transición a InlineKeyboard (3h)** ⏳ **PENDIENTE**
1. ⏳ Reemplazar ReplyKeyboardMarkup por InlineKeyboardMarkup
2. ⏳ Actualizar todos los comandos con InlineKeyboard
3. ⏳ Agregar barras de progreso animadas
4. ⏳ Testing general

---

## 🏁 CONCLUSIÓN

El bot tiene un **potencial enorme**. La base con Dapper, integración IA y sistema de combate ya te ponen por delante del 90% de los bots amateurs.

El siguiente paso lógico es la **limpieza de la interfaz (Fase 5)** y la **dinamicidad de los mensajes**. Esto hará que todas las features existentes se sientan mucho mejor y facilita la implementación de las fases futuras.

---

**¿Comenzamos con Fase 5 (UI/UX)?**
