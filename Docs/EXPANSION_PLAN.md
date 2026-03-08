# 📋 PLAN DE EXPANSIÓN Y DIFICULTAD RPG

> **Documento de Diseño:** Sistema de Progresión Avanzada, Mascotas, Habilidades Combinadas y Balance de Dificultad
> 
> **Fecha Inicio:** 12 de Febrero de 2026
> 
> **Estado:** ✅ 4 DE 5 FASES COMPLETADAS | 🚧 Fase 5 Pendiente
> 
> **Última Actualización:** 13 de Febrero de 2026

---

## 🎯 RESUMEN EJECUTIVO

### ✅ **COMPLETADO** (80% del proyecto)
- **Fase 1:** Aumento de Dificultad General (Balanceo de combate, XP exponencial, enemies buffados)
- **Fase 2:** Sistema de Mascotas (18 especies, 3 etapas evolutivas, combate integrado)
- **Fase 3:** Expansión de Clases Ocultas (17 clases, 80 pasivos únicos)
- **Fase 4:** Sistema de Habilidades Combinadas (32 combo skills desbloqueables por acciones)

### ❌ **PENDIENTE** (20% del proyecto)
- **Fase 5:** Sistema de Invocación Avanzada y Contenido Endgame
  - Minions y sistema de invocación avanzado ❌ Pendiente
  - Boss battles especiales ❌ No implementado
  - Zonas endgame (nivel 50+) ❌ No implementado

---

## 💾 **DEPLOYMENT STATUS**

### ✅ **Azure App Service - DESPLEGADO Y FUNCIONANDO**
- **URL:** https://bottelegram-rpg.azurewebsites.net
- **Región:** Configurable por recurso
- **Plan:** Basic B1
- **Estado:** 🟢 ONLINE
- **Último Deploy:** 13 de Febrero de 2026
- **Commits desde último deploy:** 0

**Variables de entorno configuradas:**
- ✅ `TELEGRAM_BOT_TOKEN`
- ✅ `GROQ_API_KEY`

**Comandos útiles:**
```bash
az webapp log tail --resource-group <rg> --name <app>
az webapp show --resource-group <rg> --name <app>
git push origin master      # Dispara deploy automático por GitHub Actions
az webapp restart --resource-group <rg> --name <app>
```

---

## 📊 ESTADO DE IMPLEMENTACIÓN

### ✅ **FASE 1: AUMENTO DE DIFICULTAD GENERAL**
**Estado:** ✅ **COMPLETADA**  
**Fecha Completada:** 12 de Febrero de 2026

#### Cambios Implementados:
- ✅ **Hit Chance Base:** Reducido de 85% a 65%
- ✅ **Stats de Enemigos:**
  * Fáciles: +120% HP/ATK/DEF (Lobo: 55 HP, 28 ATK)
  * Medios: +100% HP/ATK/DEF (Orco: 110 HP, 36 ATK)
  * Difíciles: +150% HP/ATK/DEF (Troll: 250 HP, 65 ATK)
- ✅ **Rewards:** -40% XP y Gold en todos los enemigos
- ✅ **Drop Rate:** Reducido de 15% a 8%
- ✅ **XP Exponencial:** Implementado `100 * Math.Pow(1.15, Level - 1)`
  * Nivel 1→2: 100 XP
  * Nivel 10→11: 303 XP
  * Nivel 20→21: 1,637 XP
  * Nivel 50→51: 108,366 XP

#### Archivos Modificados:
- `src/BotTelegram/RPG/Services/RpgCombatService.cs`
- `src/BotTelegram/RPG/Services/EnemyDatabase.cs`
- `src/BotTelegram/RPG/Services/EquipmentDatabase.cs`
- `src/BotTelegram/RPG/Models/RpgPlayer.cs`

---

### ✅ **FASE 2: SISTEMA DE MASCOTAS**
**Estado:** ✅ **COMPLETADA AL 100%** (Commit: c49740b)  
**Progreso:** 100%  
**Fecha Completada:** Enero 2026

#### ✅ Componentes Implementados:
- ✅ **Modelo RpgPet:** Sistema completo con Bond (0-1000), Loyalty (5 niveles), Stats, Abilities, XP
- ✅ **PetDatabase:** 6 familias de mascotas con 3 etapas evolutivas c/u (18 especies totales)  
  * 🐺 Caninos: Wolf → Wolf Alfa → Fenrir
  * 🐻 Osos: Bear → Armored Bear → Ursakar
  * 🐉 Dragones: Baby Dragon → Young Dragon → Ancestral Dragon
  * 🐱 Felinos: Wildcat → Shadow Panther → Spectral Smilodon
  * 🦅 Aves: Eagle → Royal Eagle → Phoenix
  * 🐍 Reptiles: Snake → Basilisk → Jörmungandr
- ✅ **PetTamingService:** Mecánicas de domado completas
  * `AttemptTame()`: 40% chance base + Charisma bonus + Weakness bonus
  * `PetBeast()`: Acariciar bestia (+bond, 15% instant tame)
  * `CalmBeast()`: Calmar durante combate (20 mana, 2 turnos passive)
  * `FeedPet()`: Alimentar (+20 bond, +30% HP)
- ✅ **Sistema de Loyalty:** 5 niveles con stat bonuses
  * Hostile (0-199): -30% stats
  * Neutral (200-399): 0% bonus
  * Friendly (400-599): +20% stats
  * Loyal (600-799): +50% stats
  * Devoted (800-1000): +100% stats (¡DOBLE PODER!)
- ✅ **Sistema de Evolución:** 3 etapas con requisitos (Level, Bond, Kills, Boss Kills)
- ✅ **Combate con Mascotas:** Sistema de turnos integrado (Jugador → Pet1 → Pet2 → Enemigo)
- ✅ **UI Completa:** Comando `/pets` con menús interactivos para ver, domar y gestionar mascotas
- ✅ **Sistema de XP:** Mascotas ganan XP en combates y suben de nivel
- ✅ **Habilidades Especiales:** Cada familia tiene habilidades únicas según su tipo

#### Archivos Creados/Modificados:
- `src/BotTelegram/RPG/Models/RpgPet.cs` (NEW)
- `src/BotTelegram/RPG/Services/PetDatabase.cs` (NEW)
- `src/BotTelegram/RPG/Services/PetTamingService.cs` (NEW)
- `src/BotTelegram/RPG/Commands/PetsCommand.cs` (NEW)
- `src/BotTelegram/RPG/Models/RpgPlayer.cs` (MODIFIED: +ActivePets, +PetInventory)
- `src/BotTelegram/RPG/Services/RpgCombatService.cs` (MODIFIED: Sistema de turnos con pets)

---

### ✅ **FASE 3: EXPANSIÓN DE CLASES OCULTAS**
**Estado:** ✅ **COMPLETADA** (Commit: cd12f39)  
**Progreso:** 100%  
**Fecha Completada:** Enero 2026

**Implementación:**
- ✅ **11 nuevas clases ocultas** agregadas (Fortress Knight, Immovable Mountain, Berserker Blood Rage, Arcane Siphoner, Life Weaver, Puppet Master, Time Bender, Elemental Overlord, Beast Lord, Lich King, Void Summoner)
- ✅ **80 pasivos únicos** implementados en PassiveDatabase.cs
- ✅ **Requisitos aumentados 2x-5x** para las 6 clases originales (Beast Tamer, Shadow Walker, Divine Prophet, Necromancer Lord, Elemental Sage, Blade Dancer)
- ✅ **Total: 17 clases ocultas, 80 pasivos**
- ✅ **UI completa:** Menús `/rpg_my_classes` y `/rpg_passives` con estado de desbloqueo
- ✅ **Líneas agregadas:** ~1500 líneas de código

**Clases por Categoría:**
- Tank: Fortress Knight, Immovable Mountain
- DPS Físico: Shadow Walker, Berserker Blood Rage, Blade Dancer
- DPS Mágico: Necromancer Lord, Arcane Siphoner, Elemental Sage, Elemental Overlord
- Soporte/Heal: Divine Prophet, Life Weaver
- Utility/Control: Time Bender, Puppet Master, Void Summoner
- Summoner/Tamer: Beast Tamer, Beast Lord, Lich King

**Archivos Modificados:**
- `src/BotTelegram/RPG/Services/HiddenClassDatabase.cs` (+700 líneas)
- `src/BotTelegram/RPG/Services/PassiveDatabase.cs` (+650 líneas)
- `src/BotTelegram/RPG/Models/Passive.cs` (+80 líneas)
- `src/BotTelegram/RPG/Models/HiddenClass.cs` (ENHANCED)
- `src/BotTelegram/Handlers/CallbackQueryHandler.cs` (UI menus)

---

## 📊 ANÁLISIS DEL ESTADO ACTUAL

### 🎮 Sistema Actual

#### **Clases Ocultas:**
- **Cantidad:** 6 clases (Beast Tamer, Shadow Walker, Divine Prophet, Necromancer Lord, Elemental Sage, Blade Dancer)
- **Requisitos Promedio:** 3-5 acciones con contadores entre 50-500
- **Dificultad Estimada:** **BAJA-MEDIA** (muy alcanzable en 1-2 semanas de juego activo)
- **Categorías:** Solo 2 categorías principales (Combate, Magia/Soporte)

#### **Sistema de Combate:**
- **Enemigos Fáciles:** 25-45 HP, 6-20 ATK, 2-15 DEF
- **Enemigos Medios:** 50-90 HP, 25-35 ATK, 10-25 DEF
- **Enemigos Difíciles:** 100-150 HP, 40-60 ATK, 20-40 DEF
- **Hit Chance Base:** 85% (demasiado alto)
- **Critical Chance Base:** ~10-15% (equipado) a ~30% (optimizado)
- **Progresión XP:** Lineal y poco desafiante

#### **Sistema de Acciones:**
- **Acciones Trackeadas:** ~40 acciones
- **Tipos:** Combate básico, exploración, crafting básico
- **Combinaciones:** No existen (ejemplo deseado: Acercarse 100 + Golpear 100 = Envestida)

#### **Sistema de Mascotas:**
- **Estado:** NO IMPLEMENTADO (mencionado en Beast Tamer pero sin funcionalidad)
- **Batalla con Mascotas:** No existe
- **Invocaciones:** No existe

#### **Habilidades:**
- **Skills Actuales:** ~15 habilidades básicas
- **Desbloqueo:** Solo por nivel y clase
- **Combinaciones:** No existen

---

## 🔥 PROBLEMAS IDENTIFICADOS

### 1. **Dificultad Demasiado Baja**
- ❌ Enemigos fáciles mueren en 2-3 golpes
- ❌ Hit chance de 85% hace imposible fallar
- ❌ Critical hits muy frecuentes (30% con equipment)
- ❌ Clases ocultas desbloqueables en días
- ❌ Sin curva de dificultad real

### 2. **Falta de Profundidad en Progresión**
- ❌ Solo 6 clases ocultas (poco contenido endgame)
- ❌ Sin clases especializadas (Tanks, Healers puros, Glass Cannons)
- ❌ Sin sistema de prestigio o reset
- ❌ Sin contenido post-level 50

### 3. **Sistema de Mascotas Incompleto**
- ❌ Beast Tamer menciona mascotas pero no existen
- ❌ Sin mecánicas de domado funcionales
- ❌ Sin combate con pets
- ❌ Sin invocaciones

### 4. **Falta de Sinergia Entre Sistemas**
- ❌ Acciones no desbloquean habilidades (solo cuentan para clases)
- ❌ Sin combinaciones de acciones (Acercarse + Golpear = Envestida)
- ❌ Pasivas no se aplican en combate real aún
- ❌ Equipment no tiene set bonuses

---

## 💡 PROPUESTAS DE MEJORA

### 🎯 **FASE 1: AUMENTO DE DIFICULTAD GENERAL**

#### A) **Balance de Combate**

##### **1. Reducir Hit Chance Base**
```csharp
// ACTUAL
double baseHitChance = 85.0; // Muy alto

// PROPUESTO
double baseHitChance = 65.0; // Base más realista
// Con stats optimizados puede llegar a 85-90%
// Sin optimizar: 55-70%
```

##### **2. Aumentar Stats de Enemigos (+50% base, +100% élite)**
```csharp
// EJEMPLO: Lobo Salvaje
// ACTUAL: HP=35, ATK=18, DEF=12
// PROPUESTO: HP=55, ATK=28, DEF=20

// EJEMPLO: Dragón Joven (Boss)
// ACTUAL: HP=180, ATK=55
// PROPUESTO: HP=350, ATK=90, DEF=50
```

##### **3. Reducir Ganancias de XP y Gold (-40%)**
```csharp
// ACTUAL: Lobo = 25 XP, 20 Gold
// PROPUESTO: Lobo = 15 XP, 12 Gold

// Justificación: Hace que cada nivel cueste más tiempo,
// aumenta valor del farmeo, incentiva estrategia
```

##### **4. Aumentar Costos de Skills (+50%)**
```csharp
// EJEMPLO: Fireball
// ACTUAL: 25 Mana, Cooldown 2
// PROPUESTO: 40 Mana, Cooldown 3

// EJEMPLO: Power Strike
// ACTUAL: 30 Stamina, Cooldown 3
// PROPUESTO: 45 Stamina, Cooldown 4
```

##### **5. Reducir Drop Rate de Loot**
```csharp
// ACTUAL: 15% drop chance
// PROPUESTO: 8% drop chance base
// PROPUESTO: 12% con Treasure Hunter passive
```

##### **6. Implementar Fórmulas de Scaling Exponencial**
```csharp
// XP requerido por nivel
// ACTUAL: Linear (100 * level)
// PROPUESTO: Exponencial
int RequiredXP(int level) => (int)(100 * Math.Pow(1.15, level - 1));
// Nivel 1->2: 100 XP
// Nivel 10->11: 300 XP
// Nivel 20->21: 1200 XP
// Nivel 50->51: 45000 XP
```

#### B) **Curva de Dificultad por Zonas**

```
ZONA 1: Puerto Esperanza (Nivel 1-5)
- Enemigos: 40-60 HP, 15-25 ATK
- Hit Chance: 65-70%
- Drop Rate: 6%

ZONA 2: Bosque Oscuro (Nivel 6-12)
- Enemigos: 70-110 HP, 30-45 ATK
- Hit Chance: 60-65%
- Drop Rate: 7%

ZONA 3: Montañas Gélidas (Nivel 13-20)
- Enemigos: 130-180 HP, 50-70 ATK
- Hit Chance: 55-60%
- Drop Rate: 8%

ZONA 4: Ruinas Antiguas (Nivel 21-30)
- Enemigos: 200-280 HP, 75-100 ATK
- Hit Chance: 50-55%
- Drop Rate: 9%

ZONA 5: Abismo Infernal (Nivel 31-50)
- Enemigos: 350-500 HP, 110-150 ATK
- Hit Chance: 45-50%
- Drop Rate: 10%

ZONA 6: Reino Celestial (Nivel 50+, Endgame)
- Enemigos: 600-1000 HP, 180-250 ATK
- Hit Chance: 40-45%
- Drop Rate: 12% (mejor loot quality)
```

---

### 🌟 **FASE 2: EXPANSIÓN DE CLASES OCULTAS**

#### **Nuevas Categorías de Clases**

##### **1. Tank Specialists (Especialistas en Defensa)**

**🛡️ FORTRESS KNIGHT (Caballero Fortaleza)**
```yaml
Emoji: 🛡️
Descripción: Maestro defensivo impenetrable. Puede bloquear daño masivo.
Requisitos:
  - block_damage: 5000          # Bloquear 5000 de daño total
  - perfect_block: 200          # Bloqueos perfectos (100% damage negated)
  - tank_hit: 1000              # Recibir 1000 golpes
  - survive_lethal: 50          # Sobrevivir a 50 golpes letales (>80% HP)
  - taunt_enemy: 300            # Provocar enemigos 300 veces
  - shield_bash: 150            # Golpear con escudo 150 veces
Pasivas Otorgadas:
  - unbreakable_defense: +50% block chance, +30 Physical Defense
  - damage_reflection: 25% del daño bloqueado se refleja
  - shield_mastery: Escudos otorgan +50% stats
Habilidades Desbloqueadas:
  - fortress_stance: Modo tanque (+100% DEF, -50% daño, 5 turnos)
  - shield_wall: Inmune a críticos por 3 turnos
  - guardian_aura: Aliados (pets) reciben -30% daño
Bonos de Stats:
  - Constitution: +30
  - Strength: +10
  - Wisdom: -5
```

**⚓ IMMOVABLE MOUNTAIN (Montaña Inamovible)**
```yaml
Emoji: ⛰️
Descripción: Nadie puede mover ni derribar esta roca viviente.
Requisitos:
  - damage_taken: 8000          # Recibir 8000 de daño total
  - survive_critical: 100       # Sobrevivir a 100 críticos
  - hp_below_10_survive: 30     # Sobrevivir con <10% HP 30 veces
  - no_dodge_combat: 200        # Completar 200 combates sin esquivar
  - heavy_armor_use: 500        # Usar armadura pesada 500 combates
Pasivas Otorgadas:
  - stone_skin: Reducción de daño fija 15 (antes de DEF)
  - last_stand: Al llegar a 1 HP, recupera 40% HP una vez por combate
  - immovable: Inmune a Stun y Knockback
Habilidades Desbloqueadas:
  - earthquake: Daño AoE, aturde enemigos
  - stone_shell: Invulnerabilidad 1 turno (Cooldown 10)
  - titan_grip: Armas pesadas no penalizan velocidad
Bonos de Stats:
  - Constitution: +40
  - Strength: +15
  - Dexterity: -10
```

##### **2. Glass Cannon Specialists (Alto Daño, Baja Defensa)**

**💥 BERSERKER BLOOD RAGE (Berserker Furia Sangrienta)**
```yaml
Emoji: 🩸
Descripción: Sacrifica defensa por daño devastador. Más peligroso cuanto menor HP tiene.
Requisitos:
  - critical_hit: 1000          # 1000 críticos (aumentado de 500)
  - hp_below_30_kill: 150       # Matar 150 enemigos con <30% HP
  - no_armor_combat: 100        # Combatir sin armadura 100 veces
  - consecutive_attacks: 500    # 500 ataques consecutivos sin fallar
  - overkill_damage: 200        # Hacer overkill (exceso daño) 200 veces
Pasivas Otorgadas:
  - blood_frenzy: +5% daño por cada 10% HP perdido (máx +50%)
  - reckless_abandon: +50% daño, -30% DEF
  - killing_spree: Cada kill otorga +10% daño por 3 turnos (stackeable x5)
Habilidades Desbloqueadas:
  - sacrifice: Consume 40% HP, próximo ataque hace 400% daño
  - rampage: 6 ataques rápidos (60% daño cada uno)
  - blood_pact: Convierte HP en daño extra (2 HP = 1 ATK)
Bonos de Stats:
  - Strength: +35
  - Constitution: -15
  - Dexterity: +20
```

**⚡ ARCANE SIPHONER (Sifón Arcano)**
```yaml
Emoji: 🔮
Descripción: Roba mana de enemigos y convierte todo en daño mágico devastador.
Requisitos:
  - magic_attack: 1200          # 1200 ataques mágicos
  - mana_spent: 10000           # Gastar 10000 de mana total
  - low_mana_cast: 200          # Castear con <20% mana 200 veces
  - mana_drain: 300             # Drenar mana de enemigos 300 veces
  - spell_critical: 400         # 400 críticos mágicos
Pasivas Otorgadas:
  - arcane_overflow: Cada spell que excede MaxMana hace +50% daño
  - mana_burn: Spells consumen HP si no hay mana (2 HP = 1 Mana)
  - spell_amplification: +60% daño mágico, -30% Physical Defense
Habilidades Desbloqueadas:
  - mana_void: Drena todo mana enemigo, daño = mana robado x3
  - arcane_cascade: 8 mini-spells (30% daño cada uno, 5 mana c/u)
  - spell_leech: Siguiente spell recupera mana = daño hecho / 4
Bonos de Stats:
  - Intelligence: +45
  - Constitution: -10
  - Wisdom: +25
```

##### **3. Support/Healer Specialists**

**🌸 LIFE WEAVER (Tejedor de Vida)**
```yaml
Emoji: 🌸
Descripción: Maestro absoluto de la curación. Puede revivir y regenerar infinitamente.
Requisitos:
  - heal_cast: 2000             # Curar 2000 veces (aumentado de 500)
  - hp_restored: 50000          # Restaurar 50000 HP total
  - full_heal: 300              # Curar de 0% a 100% en un cast, 300 veces
  - meditation: 500             # Meditar 500 veces (aumentado de 300)
  - survive_poison: 100         # Sobrevivir a envenenamiento 100 veces
  - no_damage_turn: 800         # 800 turnos sin atacar (solo curar)
Pasivas Otorgadas:
  - divine_touch: Heals curan +100% (total +150% con Divine Prophet)
  - regeneration_aura: Recupera 10% HP cada turno
  - life_link: Al morir, revive con 60% HP (Cooldown: 1 por combate)
Habilidades Desbloqueadas:
  - mass_heal: Cura AoE (jugador + pets) 80% MaxHP
  - resurrection: Revive pet con 100% HP
  - sanctuary: Zona inmune a daño por 2 turnos
Bonos de Stats:
  - Wisdom: +40
  - Intelligence: +20
  - Charisma: +15
```

**🎭 PUPPET MASTER (Maestro Titiritero)**
```yaml
Emoji: 🎭
Descripción: Controla mentes y cuerpos. Convierte enemigos en aliados temporales.
Requisitos:
  - mind_control: 200           # Controlar mentalmente 200 enemigos
  - confusion_inflict: 300      # Confundir enemigos 300 veces
  - charm_beast: 250            # Encantar bestias 250 veces
  - puppet_kill: 150            # Kills hechos por enemigos controlados
  - manipulation: 400           # Manipular acciones enemigas 400 veces
Pasivas Otorgadas:
  - master_manipulator: +30% duración de control mental
  - puppet_strings: Enemigos controlados hacen +50% daño
  - mind_immunity: Inmune a control mental y confusión
Habilidades Desbloqueadas:
  - dominate: Controla enemigo por 4 turnos
  - mass_confusion: Confunde todos los enemigos (AoE)
  - possession: Posees enemigo, controlas sus habilidades
Bonos de Stats:
  - Charisma: +30
  - Intelligence: +25
  - Wisdom: +10
```

##### **4. Hybrid/Utility Specialists**

**🌀 TIME BENDER (Manipulador Temporal)**
```yaml
Emoji: ⏰
Descripción: Controla el flujo del tiempo. Puede acelerar, ralentizar y repetir acciones.
Requisitos:
  - dodge_success: 800          # Esquivar 800 ataques (aumentado)
  - speed_advantage: 500        # Ganar 500 turnos por velocidad
  - double_turn: 200            # Actuar 2 veces por turno 200 veces
  - time_stop_use: 100          # Usar habilidades de tiempo 100 veces
  - perfect_timing: 300         # Contraataques perfectos 300 veces
Pasivas Otorgadas:
  - temporal_flux: +50% velocidad base
  - foresight: Ve próximo movimiento enemigo
  - time_loop: 10% chance de repetir acción gratis
Habilidades Desbloqueadas:
  - haste: Actúa 3 veces en 1 turno
  - time_stop: Enemigo pierde próximo turno
  - rewind: Revierte último turno (recupera HP/Mana perdido)
Bonos de Stats:
  - Dexterity: +35
  - Intelligence: +20
  - Wisdom: +15
```

**🌊 ELEMENTAL OVERLORD (Señor Elemental)**
```yaml
Emoji: 🌊🔥❄️⚡
Descripción: Controla todos los elementos simultáneamente. Puede crear tormentas elementales.
Requisitos:
  - fire_spell_cast: 500        # 500 hechizos de fuego (aumentado)
  - water_spell_cast: 500       # 500 hechizos de agua (aumentado)
  - earth_spell_cast: 500       # 500 hechizos de tierra (aumentado)
  - air_spell_cast: 500         # 500 hechizos de aire (aumentado)
  - combo_spell: 300            # 300 combos elementales (aumentado)
  - elemental_mastery: 250      # 250 kills con ventaja elemental
Pasivas Otorgadas:
  - elemental_fusion: Spells combinan 2 elementos
  - elemental_immunity: Inmune a daño elemental
  - primal_force: Daño elemental +80%
Habilidades Desbloqueadas:
  - elemental_storm: AoE masivo los 4 elementos
  - elemental_avatar: Transforma en elemental puro (5 turnos)
  - element_swap: Cambia debilidades/resistencias enemigo
Bonos de Stats:
  - Intelligence: +50
  - Wisdom: +30
  - Constitution: +10
```

##### **5. Summoner/Tamer Specialists (SISTEMA DE MASCOTAS)**

**🐲 BEAST LORD (Señor de las Bestias)**
```yaml
Emoji: 🐲
Descripción: Evolución de Beast Tamer. Puede tener 3 mascotas simultáneas y fusionarlas.
Requisitos:
  - pet_beast: 500              # Acariciar 500 bestias (aumentado)
  - calm_beast: 300             # Calmar 300 bestias (aumentado)
  - tame_beast: 500             # Domar 500 bestias (aumentado)
  - beast_kills: 800            # Matar 800 bestias (aumentado)
  - pet_bond_max: 100           # Llevar 100 mascotas a bond máximo (nuevo)
  - pet_evolution: 50           # Evolucionar 50 mascotas (nuevo)
Pasivas Otorgadas:
  - beast_army: +2 slots de mascota (total 3)
  - alpha_dominance: Mascotas hacen +100% daño
  - beast_fusion: Puede fusionar 2 mascotas temporalmente
Habilidades Desbloqueadas:
  - beast_stampede: Las 3 mascotas atacan juntas
  - primal_roar: Mascotas entran en furia (+150% ATK, 3 turnos)
  - beast_sacrifice: Sacrifica mascota para revivir jugador
Bonos de Stats:
  - Charisma: +30
  - Wisdom: +25
  - Dexterity: +15
```

**💀 LICH KING (Rey Lich)**
```yaml
Emoji: 💀
Descripción: Evolución de Necromancer. Ejército de no-muertos imparable.
Requisitos:
  - summon_undead: 800          # Invocar 800 no-muertos (aumentado)
  - dark_magic_cast: 1000       # 1000 hechizos oscuros (aumentado)
  - life_drain: 600             # Drenar vida 600 veces (aumentado)
  - undead_kills: 500           # Matar 500 no-muertos (aumentado)
  - sacrifice: 200              # Sacrificar minions 200 veces (nuevo)
  - desecrate: 300              # Profanar 300 veces (aumentado)
Pasivas Otorgadas:
  - undead_mastery: +3 slots de minion (total 5 minions)
  - death_aura: Enemigos pierden 5% MaxHP cada turno
  - phylactery: Si mueres con >3 minions, revives con 50% HP
Habilidades Desbloqueadas:
  - army_of_dead: Invoca 5 esqueletos
  - death_and_decay: AoE masivo que crea minions de cadáveres
  - lich_form: Transforma en lich (inmune a físico, +200% magic)
Bonos de Stats:
  - Intelligence: +40
  - Wisdom: +20
  - Constitution: -20 (eres no-muerto)
```

**👁️ VOID SUMMONER (Invocador del Vacío)**
```yaml
Emoji: 👁️
Descripción: Invoca criaturas del vacío. Pactos peligrosos con entidades cósmicas.
Requisitos:
  - void_ritual: 300            # Realizar rituales del vacío 300 veces (nuevo)
  - summon_aberration: 400      # Invocar aberraciones 400 veces (nuevo)
  - pact_damage: 5000           # Recibir 5000 daño de pactos (nuevo)
  - sacrifice_hp: 10000         # Sacrificar 10000 HP en rituales (nuevo)
  - void_gaze: 200              # Mirar al vacío 200 veces (nuevo, causa daño)
Pasivas Otorgadas:
  - eldritch_pact: Invocaciones cuestan HP en vez de mana
  - void_touched: +100% daño void, -50% sanity
  - beyond_death: Revive como aberración si mueres (1 vez por día)
Habilidades Desbloqueadas:
  - summon_horror: Invoca entidad cósmica (hace 300% daño, incontrolable)
  - void_gate: Portal que invoca aberraciones aleatorias
  - eldritch_blast: Rayo void que ignora defensas
Bonos de Stats:
  - Intelligence: +50
  - Wisdom: -20 (sanity loss)
  - Charisma: +30 (para pactos)
```

---

### 🏆 **TOTAL DE CLASES OCULTAS PROPUESTAS**

```
Clases Actuales: 6
Clases Nuevas: 10
TOTAL: 16 clases ocultas

Distribución por Categoría:
- Tank: 2 (Fortress Knight, Immovable Mountain)
- DPS Físico: 3 (Shadow Walker, Berserker, Blade Dancer)
- DPS Mágico: 3 (Necromancer Lord, Arcane Siphoner, Elemental Overlord)
- Soporte/Heal: 2 (Divine Prophet, Life Weaver)
- Utility/Control: 3 (Time Bender, Puppet Master, Void Summoner)
- Summoner/Tamer: 3 (Beast Tamer, Beast Lord, Lich King)
```

---

### 🐾 **FASE 3: SISTEMA DE MASCOTAS (PET SYSTEM)**

#### **A) Mecánicas de Domado**

##### **1. Acciones de Interacción con Bestias**

```csharp
// Nuevas acciones durante exploración/combate con bestias

ACCIÓN: "🐾 Acariciar" (Pet Beast)
- Disponible: Después de victoria contra bestia, si bestia <30% HP
- Efecto: +5 bond con bestia, 15% chance de domar instantáneamente
- Trackea: pet_beast

ACCIÓN: "🎶 Calmar" (Calm Beast)
- Disponible: Durante combate contra bestia agresiva
- Efecto: Bestia cambia a comportamiento pasivo, no ataca 2 turnos
- Costo: 20 Mana
- Trackea: calm_beast

ACCIÓN: "⛓️ Domar" (Tame Beast) - SKILL
- Disponible: Si tienes skill "Tame Beast"
- Efecto: 40% chance de domar bestia (aumenta con Charisma)
- Requisitos: Bestia debe estar <50% HP
- Costo: 50 Stamina, 30 Mana
- Trackea: tame_beast

ACCIÓN: "🍖 Alimentar" (Feed Beast)
- Disponible: Si tienes comida en inventario
- Efecto: +20 bond, cura 30% HP de bestia
- Trackea: feed_beast
```

##### **2. Sistema de Bond (Vínculo)**

```csharp
public class RpgPet
{
    public string Id { get; set; }               // Unique ID
    public string Name { get; set; }             // Nombre personalizado
    public string Species { get; set; }          // "Wolf", "Bear", "Dragon"
    public int Level { get; set; } = 1;
    
    // Bond System
    public int Bond { get; set; } = 0;           // 0-1000
    public PetLoyalty Loyalty { get; set; }      // Hostile/Neutral/Friendly/Loyal/Devoted
    
    // Combat Stats
    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    
    // Skills
    public List<string> Abilities { get; set; } = new();
    
    // Evolution
    public int EvolutionStage { get; set; } = 1; // 1=Basic, 2=Advanced, 3=Ultimate
    public int EvolutionXP { get; set; } = 0;
    
    // Behavior
    public PetBehavior Behavior { get; set; }    // Aggressive/Defensive/Supportive
}

public enum PetLoyalty
{
    Hostile = 0,      // 0-199 bond - Puede atacarte
    Neutral = 1,      // 200-399 bond - Obedece básico
    Friendly = 2,     // 400-599 bond - Obedece, +20% stats
    Loyal = 3,        // 600-799 bond - Obedece siempre, +50% stats
    Devoted = 4       // 800-1000 bond - Sacrifica vida por ti, +100% stats
}
```

##### **3. Combate con Mascotas**

```
SISTEMA DE TURNOS CON MASCOTAS:

Turno 1: Jugador elige acción
↓
Turno 2: Pet 1 ataca (automático según Behavior)
↓
Turno 3: Pet 2 ataca (si tienes 2 mascotas)
↓
Turno 4: Enemigo ataca (elige target: jugador 60%, pet 40%)
↓
Repite

CONTROLES DE MASCOTAS:
- Botón "🐾 Órdenes" muestra:
  * "⚔️ Atacar" - Pet ataca con habilidad básica
  * "🛡️ Defender" - Pet bloquea 50% daño dirigido a jugador
  * "🏃 Huir" - Pet escapa del combate
  * "💊 Curar" - Pet usa objeto de curación (si tiene)
  * "⚡ Habilidad" - Menú de habilidades especiales del pet

MECÁNICAS ESPECIALES:
- Combo Owner+Pet: Si ambos atacan mismo turno, +25% daño
- Pet Shield: Si Loyalty=Devoted, pet bloquea ataques letales (revive con 1 HP después)
- Beast Fury: Si jugador <30% HP, pets enfurecen (+100% ATK, -50% DEF)
```

##### **4. Evolución de Mascotas**

```yaml
EJEMPLO: Lobo → Lobo Alfa → Fenrir

ETAPA 1: Lobo Salvaje
- Stats Base: 50 HP, 25 ATK, 15 DEF
- Habilidades: Mordisco (120% daño)
- Requisitos Evolución: Level 15, Bond 400, 50 kills

ETAPA 2: Lobo Alfa
- Stats: 120 HP, 60 ATK, 35 DEF
- Habilidades: Mordisco, Garra Salvaje (150% daño + Bleeding), Aullido (buff +20% ATK team)
- Requisitos Evolución: Level 35, Bond 700, 200 kills, 50 boss kills

ETAPA 3: Fenrir (Legendario)
- Stats: 280 HP, 140 ATK, 80 DEF
- Habilidades: Mordisco, Garra, Aullido, Furia Ancestral (300% daño AoE), Lobo Espectral (evasión +50% 3 turnos)
- Pasiva: "Pack Leader" - Otros pets wolf reciben +50% stats
```

#### **B) Sistema de Invocación (Summoner)**

```csharp
public class SummonMechanic
{
    // INVOCAR MINION
    public RpgPet SummonMinion(RpgPlayer player, string minionType)
    {
        // Costo según tipo
        int manaCost = minionType switch
        {
            "skeleton" => 40,
            "zombie" => 60,
            "ghost" => 80,
            "lich" => 150,
            _ => 30
        };
        
        if (player.Mana < manaCost) return null;
        
        player.Mana -= manaCost;
        
        // Crear minion
        var minion = CreateMinionByType(minionType);
        minion.Level = player.Level / 2; // Minions son mitad de nivel
        
        // Limitar cantidad según clase
        int maxMinions = player.ActiveHiddenClass switch
        {
            "necromancer_lord" => 3,
            "lich_king" => 5,
            _ => 1
        };
        
        if (player.ActivePets.Count >= maxMinions)
        {
            // Reemplazar minion más débil
            var weakest = player.ActivePets.OrderBy(p => p.HP).First();
            player.ActivePets.Remove(weakest);
        }
        
        player.ActivePets.Add(minion);
        TrackAction(player, $"summon_{minionType}");
        
        return minion;
    }
    
    // SACRIFICAR MINION (Heal masivo o daño extra)
    public void SacrificeMinion(RpgPlayer player, RpgPet minion)
    {
        // Heal = 50% de HP del minion
        int healAmount = minion.MaxHP / 2;
        player.HP = Math.Min(player.HP + healAmount, player.MaxHP);
        
        // Bonus temporal +20% daño
        player.StatusEffects.Add(new StatusEffect
        {
            Type = StatusEffectType.Empowered,
            Duration = 3,
            Intensity = 20
        });
        
        player.ActivePets.Remove(minion);
        TrackAction(player, "sacrifice_minion");
    }
}
```

---

### ⚔️ **FASE 4: SISTEMA DE HABILIDADES COMBINADAS**
**Estado:** ✅ **COMPLETADA** (Commit: c02df21)  
**Progreso:** 100%  
**Fecha Completada:** 13 de Febrero de 2026

#### ✅ Componentes Implementados:
- ✅ **SkillUnlockDatabase.cs:** 32 combo skills con requisitos de acciones múltiples
- ✅ **ActionTrackerService:** Método CheckComboSkillUnlocks() para auto-detección
- ✅ **SkillDatabase.cs:** 32 nuevas habilidades combinadas con stats completos
- ✅ **CallbackQueryHandler.cs:** UI completa con 3 menús (progreso, lista completa, requisitos)
- ✅ **RpgCommand.cs:** Botón "🌟 Skills Combinadas" en menú principal
- ✅ **GetProgressBar():** Método helper para barras de progreso visuales

#### 📊 Skills Implementadas (32 total):
- **Combate (5):** charge_strike, rampage, execute, whirlwind, blood_strike
- **Magia (5):** meteor_storm, arcane_burst, mana_void, elemental_fury, chaos_bolt
- **Defensa (5):** iron_fortress, shield_wall, last_stand, counter_strike, guardian_aura
- **Sigilo (5):** assassinate, shadow_dance, backstab_mastery, vanishing_strike, lethal_precision
- **Curación (5):** divine_intervention, mass_resurrection, holy_nova, life_transfer, sanctuary
- **Invocación (4):** beast_stampede, army_of_dead, pet_fusion, dark_pact
- **Híbridas (2):** battle_meditation, time_warp

---

#### **Concepto: Desbloqueoss por Combinación de Acciones**

```yaml
EJEMPLO 1: ENVESTIDA (Charge Attack)
Requisitos:
  - approach_enemy: 100         # Acercarse al enemigo 100 veces
  - heavy_attack: 100           # Ataques pesados 100 veces
Habilidad Desbloqueada:
  Nombre: "Envestida"
  Efecto: Corre hacia el enemigo y golpea (200% daño + Stun 50%)
  Costo: 40 Stamina
  Cooldown: 4 turnos

EJEMPLO 2: DANZA DE ESPADAS (Sword Dance)
Requisitos:
  - dodge_success: 200          # Esquivar 200 ataques
  - consecutive_attacks: 200    # 200 ataques consecutivos
  - critical_hit: 150           # 150 críticos
Habilidad Desbloqueada:
  Nombre: "Danza de Espadas"
  Efecto: 7 ataques rápidos (80% daño c/u), +15% critical c/u
  Costo: 50 Stamina
  Cooldown: 6 turnos

EJEMPLO 3: MEDITAR EN COMBATE (Battle Meditation)
Requisitos:
  - meditation: 300             # Meditar 300 veces
  - no_damage_turn: 200         # 200 turnos sin recibir daño
  - mana_regen: 5000            # Regenerar 5000 mana total
Habilidad Desbloqueada:
  Nombre: "Meditación de Batalla"
  Efecto: Recupera 100% Mana, +50% Magic Power próximo spell
  Costo: 0 (pero pierdes turno)
  Cooldown: 8 turnos

EJEMPLO 4: ROBO DE VIDA MEJORADO (Vampiric Strike)
Requisitos:
  - life_drain: 200             # Drenar vida 200 veces
  - critical_hit: 300           # 300 críticos
  - hp_below_30_kill: 100       # Matar con <30% HP 100 veces
Habilidad Desbloqueada:
  Nombre: "Golpe Vampírico"
  Efecto: 180% daño, recupera 80% del daño como HP
  Costo: 35 Stamina
  Cooldown: 4 turnos

EJEMPLO 5: EXPLOSIÓN ELEMENTAL (Elemental Burst)
Requisitos:
  - fire_spell_cast: 150
  - water_spell_cast: 150
  - earth_spell_cast: 150
  - air_spell_cast: 150
Habilidad Desbloqueada:
  Nombre: "Explosión Elemental"
  Efecto: AoE masivo con los 4 elementos (250% daño mágico)
  Costo: 100 Mana
  Cooldown: 8 turnos

EJEMPLO 6: TELETRANSPORTE (Blink Strike)
Requisitos:
  - dodge_success: 300          # Esquivar 300 ataques
  - backstab: 100               # 100 backstabs
  - stealth_kill: 80            # 80 stealth kills
Habilidad Desbloqueada:
  Nombre: "Golpe Teletransportado"
  Efecto: Teletransportas detrás del enemigo, 250% daño crítico garantizado
  Costo: 40 Stamina, 20 Mana
  Cooldown: 5 turnos

EJEMPLO 7: FORTALEZA DE FE (Fortress of Faith)
Requisitos:
  - heal_cast: 400              # Curar 400 veces
  - block_damage: 3000          # Bloquear 3000 de daño
  - survive_lethal: 50          # Sobrevivir a 50 golpes letales
Habilidad Desbloqueada:
  Nombre: "Fortaleza de Fe"
  Efecto: Barrera que absorbe 500 daño, dura 5 turnos
  Costo: 80 Mana
  Cooldown: 10 turnos

EJEMPLO 8: TORMENTA DE INVOCACIONES (Summoning Storm)
Requisitos:
  - summon_undead: 300
  - sacrifice_minion: 100
  - pet_bond_max: 50
Habilidad Desbloqueada:
  Nombre: "Tormenta de Invocaciones"
  Efecto: Invoca 5 minions aleatorios instantáneamente
  Costo: 150 Mana, 50% HP actual
  Cooldown: 12 turnos
```

#### **Total de Habilidades Combinadas Propuestas: 30+ skills**

```
Sistema:
1. Jugador realiza acciones normales durante juego
2. ActionTrackerService detecta cuando se cumplen múltiples requisitos
3. Auto-desbloquea habilidad y notifica jugador
4. Skill aparece en menú "⚔️ Skills" disponible en combate
```

---

### 📈 **FASE 5: NUEVAS ACCIONES TRACKEABLES**

#### **Acciones Actuales: ~40**
#### **Acciones Nuevas Propuestas: +60**

```yaml
# ═══════════════════════════════════════
# COMBATE AVANZADO
# ═══════════════════════════════════════
approach_enemy: Acercarse al enemigo antes de atacar
retreat: Retirarse/huir del enemigo
charge_attack: Envestida/embestida
heavy_attack: Ataque pesado (sacrifica velocidad por daño)
light_attack: Ataque rápido (menos daño, más velocidad)
precise_attack: Ataque preciso (aumenta accuracy)
reckless_attack: Ataque temerario (+daño, -defensa)
defensive_attack: Ataque defensivo (-daño, +defensa)
consecutive_attacks: Ataques consecutivos sin fallar
combo_3x: Combo de 3 ataques
combo_5x: Combo de 5 ataques
combo_10x: Combo de 10 ataques
combo_20x: Combo de 20 ataques
overkill_damage: Daño excesivo (matar con >200% HP restante)
no_damage_combat: Completar combate sin recibir daño
no_critical_combat: Completar combate sin hacer crítico
speed_advantage: Ganar turno por velocidad
double_turn: Actuar 2 veces en 1 turno

# ═══════════════════════════════════════
# DEFENSA Y SUPERVIVENCIA
# ═══════════════════════════════════════
block_damage: Total de daño bloqueado
perfect_block: Bloqueo perfecto (100% daño negado)
parry: Contragolpe exitoso
perfect_parry: Contragolpe perfecto (devuelve daño)
tank_hit: Recibir golpe deliberadamente (tanking)
survive_lethal: Sobrevivir a golpe letal (>80% HP en un hit)
survive_critical: Sobrevivir a crítico enemigo
hp_below_10_survive: Sobrevivir con <10% HP
hp_below_30_kill: Matar enemigo con <30% HP
low_hp_combat: Completar combate con <50% HP todo el tiempo
no_dodge_combat: Completar combate sin esquivar nunca
damage_taken: Total de daño recibido
shield_bash: Golpear con escudo
taunt_enemy: Provocar enemigo (forzar target)

# ═══════════════════════════════════════
# MAGIA Y MANA
# ═══════════════════════════════════════
fire_spell_cast: Hechizos de fuego
water_spell_cast: Hechizos de agua
earth_spell_cast: Hechizos de tierra
air_spell_cast: Hechizos de viento (aire)
ice_spell_cast: Hechizos de hielo
lightning_spell_cast: Hechizos de rayo
dark_magic_cast: Magia oscura
holy_magic_cast: Magia sagrada
void_magic_cast: Magia del vacío
combo_spell: Combo de 2+ elementos
spell_critical: Crítico mágico
mana_spent: Total de mana gastado
mana_regen: Total de mana regenerado
low_mana_cast: Castear con <20% mana
mana_drain: Drenar mana de enemigo
overcharge_spell: Cargar spell >100% mana

# ═══════════════════════════════════════
# INVOCACIÓN Y MASCOTAS
# ═══════════════════════════════════════
summon_undead: Invocar no-muerto
summon_elemental: Invocar elemental
summon_beast: Invocar bestia
summon_aberration: Invocar aberración
sacrifice_minion: Sacrificar minion
pet_bond_max: Llevar bond de pet a máximo (1000)
pet_evolution: Evolucionar mascota
pet_combo_kill: Kill hecho por jugador+pet combo
tame_boss: Domar boss (muy difícil)

# ═══════════════════════════════════════
# STEALTH Y ENGAÑO
# ═══════════════════════════════════════
stealth_approach: Acercamiento sigiloso
stealth_kill: Kill desde stealth
backstab: Ataque por la espalda
vanish: Desaparecer/invisibilidad
shadow_travel: Viajar por sombras
assassination: Asesinar enemy en 1 hit

# ═══════════════════════════════════════
# CRAFTING Y RECURSOS
# ═══════════════════════════════════════
craft_item: Craftear objeto
upgrade_equipment: Mejorar equipo
enchant_equipment: Encantar equipo
forge_weapon: Forjar arma
gather_herbs: Recolectar hierbas
mine_ore: Minar mineral
fish: Pescar
cook_food: Cocinar comida

# ═══════════════════════════════════════
# SOCIAL Y EXPLORACIÓN
# ═══════════════════════════════════════
trade_npc: Comerciar con NPC
negotiate: Negociar precio
persuade: Persuadir NPC
intimidate: Intimidar NPC
explore_zone: Explorar nueva zona
discover_secret: Descubrir secreto
solve_puzzle: Resolver puzzle
complete_quest: Completar misión

# ═══════════════════════════════════════
# RITUALES Y ESPECIALES
# ═══════════════════════════════════════
void_ritual: Ritual del vacío
dark_ritual: Ritual oscuro
divine_ritual: Ritual divino
blood_sacrifice: Sacrificio de sangre
soul_harvest: Cosechar almas
desecrate: Profanar lugar sagrado
consecrate: Consagrar lugar profano
time_stop_use: Detener tiempo
mind_control: Controlar mente
possession: Posesión de cuerpo
```

---

## 📋 ROADMAP DE IMPLEMENTACIÓN

### 🗓️ **FASE 1: BALANCE Y DIFICULTAD (Est: 3-5 días)**

#### **Paso 1.1: Rebalanceo de Combate**
```
Archivos a modificar:
- RpgCombatService.cs
- EnemyDatabase.cs
- RpgPlayer.cs (fórmulas de scaling)

Cambios:
✅ Reducir baseHitChance de 85% → 65%
✅ Aumentar stats enemigos [fácil: +30%, medio: +50%, difícil: +80%]
✅ Reducir XP rewards -40%
✅ Reducir Gold rewards -40%
✅ Reducir loot drop de 15% → 8%
✅ Aumentar skill costs +50%
✅ Implementar scaling exponencial XP: RequiredXP = 100 * (1.15 ^ (level-1))

Tiempo estimado: 1 día
```

#### **Paso 1.2: Curva de Dificultad por Zonas**
```
Archivos a modificar:
- EnemyDatabase.cs (separar por zonas)
- RpgService.cs (sistema de zonas)

Crear:
- LocationDatabase.cs (nueva)

Implementar:
✅ 6 zonas con dificultad escalada
✅ Sistema de level requirements por zona
✅ Sistema de unlock de zonas (debes completar previa)
✅ Enemies específicos por zona

Tiempo estimado: 2 días
```

#### **Paso 1.3: Testing de Balance**
```
✅ Testear progresión nivel 1-10
✅ Testear progresión nivel 11-25
✅ Testear progresión nivel 26-50
✅ Ajustar valores si es muy difícil/fácil

Tiempo estimado: 1-2 días
```

---

### 🗓️ **FASE 2: SISTEMA DE MASCOTAS (Est: 5-7 días)**

#### **Paso 2.1: Modelos y Bases de Datos**
```
Crear nuevos archivos:
- RPG/Models/RpgPet.cs
- RPG/Models/PetSpecies.cs
- RPG/Services/PetDatabase.cs

Modificar:
- RpgPlayer.cs (agregar List<RpgPet> ActivePets)

Implementar:
✅ Modelo RpgPet con stats, bond, loyalty, evolution
✅ Enum PetLoyalty, PetBehavior
✅ PetDatabase con 15+ especies (Wolf, Bear, Dragon, etc.)
✅ Sistema de bond (0-1000)

Tiempo estimado: 1 día
```

#### **Paso 2.2: Mecánicas de Domado**
```
Crear:
- RPG/Services/PetTamingService.cs

Modificar:
- ActionTrackerService.cs (nuevas acciones pet_beast, calm_beast, etc.)
- CallbackQueryHandler.cs (botones de interacción con bestias)

Implementar:
✅ Acción"🐾 Acariciar" disponible post-combate vs bestias
✅ Acción "🎶 Calmar" durante combate
✅ Skill "⛓️ Domar" con %success basado en Charisma
✅ Acción "🍖 Alimentar"
✅ Sistema de Bond (acciones suman puntos)

Tiempo estimado: 2 días
```

#### **Paso 2.3: Combate con Mascotas**
```
Modificar:
- RpgCombatService.cs (sistema de turnos con pets)
- CallbackQueryHandler.cs (menú "🐾 Órdenes")

Implementar:
✅ Turnos: Jugador → Pet1 → Pet2 → Enemigo
✅ IA de mascotas según PetBehavior
✅ Target selection (enemigo puede atacar pets)
✅ Menú de órdenes (Atacar, Defender, Huir, Curar, Habilidad)
✅ Combos Owner+Pet (+25% daño si atacan mismo turno)
✅ Pet Shield (Loyalty=Devoted bloquea lethals)
✅ Beast Fury (pets +100% ATK si owner <30% HP)

Tiempo estimado: 2-3 días
```

#### **Paso 2.4: Evolución de Mascotas**
```
Crear:
- RPG/Services/PetEvolutionService.cs

Implementar:
✅ Sistema de XP de pets (ganan XP cuando matan)
✅ Sistema de nivelación de pets (Level 1-50)
✅ Evoluciones en 3 etapas (Basic → Advanced → Ultimate)
✅ Requisitos de evolución (Level, Bond, Kills)
✅ Notificación cuando pet puede evolucionar
✅ Menú "🐾 Mis Mascotas" en exploración

Tiempo estimado: 1-2 días
```

---

### 🗓️ **FASE 3: EXPANSIÓN DE CLASES OCULTAS (Est: 4-6 días)**

#### **Paso 3.1: Nuevas Acciones Trackeables**
```
Modificar:
- ActionTrackerService.cs (detectar 60 nuevas acciones)
- RpgCombatService.cs (track acciones durante combate)
- CallbackQueryHandler.cs (track acciones durante exploración)

Implementar:
✅ 60 nuevas acciones (approach_enemy, heavy_attack, shield_bash, etc.)
✅ Tracking automático durante combate
✅ GetActionName() expandido con nuevas traducciones

Tiempo estimado: 1 día
```

#### **Paso 3.2: Nuevas Clases Ocultas**
```
Modificar:
- HiddenClassDatabase.cs (agregar 10 nuevas clases)

Implementar:
✅ 2 Tank classes (Fortress Knight, Immovable Mountain)
✅ 2 Glass Cannon classes (Berserker Blood Rage, Arcane Siphoner)
✅ 2 Support classes (Life Weaver, Puppet Master)
✅ 2 Hybrid classes (Time Bender, Elemental Overlord)
✅ 2 Summoner classes (Beast Lord, Lich King, Void Summoner)
✅ TOTAL: 16 clases (6 actuales + 10 nuevas)

Tiempo estimado: 2 días
```

#### **Paso 3.3: Nuevas Pasivas**
```
Modificar:
- PassiveDatabase.cs (agregar 40 nuevas pasivas)

Implementar:
✅ Pasivas Tank (Stone Skin, Unbreakable Defense, Last Stand)
✅ Pasivas Berserker (Blood Frenzy, Reckless Abandon, Killing Spree)
✅ Pasivas Support (Divine Touch, Regeneration Aura, Life Link)
✅ Pasivas Control (Temporal Flux, Foresight, Time Loop)
✅ Pasivas Summoner (Alpha Dominance, Beast Fusion, Undead Mastery)
✅ TOTAL: 70+ pasivas (30 actuales + 40 nuevas)

Tiempo estimado: 1 día
```

#### **Paso 3.4: UI para Nuevas Clases**
```
Modificar:
- CallbackQueryHandler.cs (menú rpg_progress, rpg_my_classes)

Verificar:
✅ Menú "Progreso" muestra 16 clases correctamente
✅ Menú "Mis Clases" permite activar cualquiera de las 16
✅ GetActionName traduce las 60+ nuevas acciones
✅ Descripciones de clases son claras

Tiempo estimado: 1 día
```

---

### 🗓️ **FASE 4: HABILIDADES COMBINADAS (Est: 3-4 días)**

#### **Paso 4.1: Sistema de Requirements Múltiples**
```
Crear:
- RPG/Models/CombinedSkillRequirement.cs
- RPG/Services/CombinedSkillDatabase.cs

Modificar:
- ActionTrackerService.cs (verificar múltiples requisitos simultáneos)

Implementar:
✅ Estructura para skills con 2-4 requisitos
✅ Verificación de todas las condiciones
✅ Auto-unlock cuando se cumplen
✅ Notificación de desbloqueo

Tiempo estimado: 1 día
```

#### **Paso 4.2: 30+ Habilidades Combinadas**
```
Modificar:
- CombinedSkillDatabase.cs

Implementar:
✅ 8 skills showcase (Envestida, Danza de Espadas, etc.)
✅ 22+ skills adicionales (Fortress of Faith, Summoning Storm, etc.)
✅ Skills de todas las categorías (Tank, DPS, Support, Utility)

Tiempo estimado: 1 día
```

#### **Paso 4.3: Integración en Combate**
```
Modificar:
- RpgCombatService.Actions.cs (agregar nuevas skills)
- SkillDatabase.cs (agregar combined skills)
- CallbackQueryHandler.cs (menú skills muestra combined)

Implementar:
✅ Ejecución de combined skills en combate
✅ Efectos especiales (AoE, Multi-hit, Buffs)
✅ Animaciones de texto descriptivas

Tiempo estimado: 1-2 días
```

---

### 🗓️ **FASE 5: INVOCACIÓN AVANZADA (Est: 3-4 días)**

#### **Paso 5.1: Sistema de Minions**
```
Crear:
- RPG/Models/Minion.cs (hereda de RpgPet)
- RPG/Services/MinionDatabase.cs

Implementar:
✅ Tipos de minions (Skeleton, Zombie, Ghost, Lich, etc.)
✅ Stats scaling basados en nivel del jugador
✅ Límites por clase (Necromancer=3, Lich King=5)
✅ Duración temporal vs permanente

Tiempo estimado: 1 día
```

#### **Paso 5.2: Habilidades de Invocación**
```
Modificar:
- RpgCombatService.Actions.cs
- SkillDatabase.cs

Implementar:
✅ Skill "Invocar Esqueleto" (40 mana)
✅ Skill "Invocar Zombie" (60 mana)
✅ Skill "Ejército de Muertos" (150 mana, 5 skeletons)
✅ Skill "Invocar Horror" (120 mana + 40% HP, aberración incontrolable)
✅ Skill "Sacrificar Minion" (heal + buff temporal)

Tiempo estimado: 1 día
```

#### **Paso 5.3: Combate con Múltiples Minions**
```
Modificar:
- RpgCombatService.cs

Implementar:
✅ Turnos: Jugador → Minion1 → Minion2 → Minion3... → Enemigo
✅ Minions actúan automáticamente (IA simple)
✅ Jugador puede ordenar sacrificio de minion
✅ Enemigo AoE puede dañar múltiples minions
✅ UI muestra lista de minions activos con HP

Tiempo estimado: 1-2 días
```

---

### 🗓️ **FASE 6: POLISH Y TESTING (Est: 3-5 días)**

#### **Paso 6.1: Aplicar Pasivas en Combate**
```
Modificar:
- RpgCombatService.cs (aplicar pasivas en cálculos)

Implementar:
✅ Life Steal: +15% lifesteal en PlayerAttack
✅ Bloodlust: +daño según HP perdido
✅ Thorns: 20% reflect en EnemyAttack
✅ Critical Mastery: +10% crit chance
✅ Regeneration: 5% HP por turno
✅ Stone Skin: -15 flat damage
✅ Y 60+ pasivas más...

Tiempo estimado: 2 días
```

#### **Paso 6.2: Balance Final**
```
✅ Testear todas las 16 clases ocultas
✅ Verificar requisitos son alcanzables pero difíciles
✅ Ajustar contadores si muy fácil/difícil
✅ Testear mascotas (bond, evolución, combate)
✅ Testear combined skills (requisitos, efectos)
✅ Testear invocaciones (límites, sacrificios)

Tiempo estimado: 2-3 días
```

#### **Paso 6.3: Documentación**
```
Crear:
- Docs/PET_SYSTEM.md
- Docs/COMBINED_SKILLS.md
- Docs/DIFFICULTY_GUIDE.md

Actualizar:
- Docs/HIDDEN_CLASSES_SYSTEM.md (nuevas clases)
- Docs/FEATURES_ROADMAP.md

Tiempo estimado: 1 día
```

---

## 📊 RESUMEN EJECUTIVO

### 📈 **Métricas de Expansión**

| **Sistema** | **Actual** | **Propuesto** | **Incremento** |
|-------------|------------|---------------|----------------|
| Clases Ocultas | 6 | 16 | **+167%** |
| Pasivas | 30 | 70+ | **+133%** |
| Skills Totales | 15 | 45+ | **+200%** |
| Acciones Trackeadas | 40 | 100+ | **+150%** |
| Especies de Mascotas | 0 | 15+ | **+∞** |
| Tipos de Minions | 0 | 10+ | **+∞** |
| Zonas jugables | 1 | 6 | **+500%** |

### ⏱️ **Tiempo Total Estimado**

```
Fase 1 (Balance): 3-5 días
Fase 2 (Mascotas): 5-7 días
Fase 3 (Clases): 4-6 días
Fase 4 (Combined Skills): 3-4 días
Fase 5 (Invocación): 3-4 días
Fase 6 (Polish): 3-5 días

TOTAL: 21-31 días de desarrollo
(Aproximadamente 1 mes de trabajo)
```

### 🎯 **Rango de Dificultad Final**

```
FÁCIL (Level 1-10):
- Enemigos: 40-100 HP, 15-35 ATK
- Hit Chance: 60-70%
- Drop Rate: 6%
- Tiempo promedio por nivel: 30-45 min

MEDIO (Level 11-25):
- Enemigos: 110-180 HP, 40-70 ATK  
- Hit Chance: 55-65%
- Drop Rate: 7-8%
- Tiempo promedio por nivel: 1-1.5 horas

DIFÍCIL (Level 26-40):
- Enemigos: 200-350 HP, 75-120 ATK
- Hit Chance: 50-60%
- Drop Rate: 9%
- Tiempo promedio por nivel: 1.5-2 horas

ENDGAME (Level 41-50+):
- Enemigos: 400-1000 HP, 130-250 ATK
- Hit Chance: 45-55%
- Drop Rate: 10-12%
- Tiempo promedio por nivel: 2-4 horas

CLASES OCULTAS:
- Tiempo mínimo desbloqueo: 2-3 semanas juego activo
- Clases tempranas (Beast Tamer): 1-2 semanas
- Clases avanzadas (Lich King): 4-6 semanas
- Clases endgame (Void Summoner): 2-3 meses
```

---

## 💬 SUGERENCIAS ADICIONALES

### 🌟 **Sistemas Futuros (Post-Expansión)**

1. **Sistema de Guildas/Clanes**
   - Guildas de jugadores
   - Guild wars (PvP)
   - Boss raids cooperativos

2. **Sistema de Prestigio**
   - Reset de nivel pero mantén clases ocultas
   - Prestigio levels con bonos permanentes
   - Cosmetics y títulos exclusivos

3. **Sistema de Dungeons**
   - Dungeons procedurales
   - Dificultad escalable (Normal/Hard/Nightmare)
   - Loot único de dungeons

4. **Sistema de Raids**
   - Boss battles cooperativos (4-8 jugadores)
   - Mecánicas complejas (fases, adds, enrage)
   - Loot legendario

5. **Sistema de Crafting Avanzado**
   - Recipe unlocks
   - Material gathering
   - Equipment forging
   - Encantamientos

6. **Sistema de Housing**
   - Base personal
   - Decoraciones
   - Storage expansions
   - Museo de trofeos

### 🎨 **Mejoras de UI/UX**

1. **Logs de Combate Mejorados**
   - Separar por turnos con líneas divisorias
   - Color coding (daño rojo, heal verde, crítico dorado)
   - Resumen al final (Total damage, DPS, healing)

2. **Estadísticas del Jugador**
   - Menú "📊 Estadísticas"
   - Total kills, daño hecho, healing, gold ganado
   - Récords (mayor crítico, más combo, etc.)
   - Achievements

3. **Leaderboards**
   - Top jugadores por nivel
   - Top por clases desbloqueadas
   - Top por boss kills
   - Top por gold acumulado

4. **Tutorial Mejorado**
   - Guía paso a paso para nuevos jugadores
   - Tooltips en menús
   - Sistema de hints

### 🔧 **Mejoras Técnicas**

1. **Optimización de Almacenamiento**
   - Migrar de JSON a SQLite
   - Reducir tamaño de saves
   - Backups automáticos

2. **Sistema de Logs**
   - Logs detallados de acciones del jugador
   - Debugging mejorado
   - Telemetría de balance

3. **Sistema de Eventos**
   - Eventos temporales (Halloween, Navidad)
   - Enemigos especiales
   - Rewards limitados

---

## ✅ PRÓXIMOS PASOS RECOMENDADOS

### � **PRIORIDAD ALTA - FASE 4: Sistema de Habilidades Combinadas**

**Objetivo:** Implementar 30+ nuevas skills desbloqueables mediante combinaciones de acciones

**Tareas Pendientes:**
1. ❌ **Crear SkillUnlockDatabase.cs**
   - Definir 30 skills combinadas
   - Requisitos: Combinaciones de 2-4 acciones diferentes
   - Ejemplo: `approach_enemy(100) + heavy_attack(100) = Envestida`

2. ❌ **Modificar ActionTrackerService**
   - Detector de combinaciones cumplidas
   - Auto-desbloqueo de skills al completar requisitos
   - Notificación al jugador

3. ❌ **Implementar Skills en Combate**
   - Integrar nuevas skills en menú de combate
   - Efectos especiales (AoE, DoT, buff/debuff)
   - Cooldowns y costos de mana/stamina

4. ❌ **UI para Skills Combinadas**
   - Menú "⚔️ Skills Combinadas" en `/rpg`
   - Mostrar progreso de requisitos (35/100 approach_enemy)
   - Lista de skills desbloqueadas vs bloqueadas

**Estimado:** 3-4 días de desarrollo

---

### 📋 **PRIORIDAD MEDIA - FASE 5: Expansión de Acciones**

**Objetivo:** Ampliar de ~40 acciones actuales a 100+ acciones trackeables

**Tareas Pendientes:**
1. ⏳ **Agregar 60 nuevas acciones** (actualmente ~40/100)
   - Combate avanzado: consecutive_attacks, combo_3x, overkill_damage
   - Defensa: perfect_block, parry, tank_hit
   - Magia elemental: fire/water/earth/air_spell_cast
   - Stealth: stealth_approach, assassination
   - Crafting: forge_weapon, enchant_equipment
   - Social: persuade, intimidate, negotiate

2. ❌ **Sistema de Zonas/Localizaciones**
   - LocationDatabase.cs con 6+ zonas
   - Requisitos de nivel para acceder
   - Enemigos específicos por zona
   - Loot tables por zona

3. ❌ **Boss Battles Especiales**
   - 10+ bosses únicos con mecánicas especiales
   - Multi-fase battles
   - Guaranteed legendary loot
   - Achievements por boss derrotado

**Estimado:** 4-5 días de desarrollo

---

## 📊 **ESTADO ACTUAL DEL PROYECTO**

### ✅ **LO QUE FUNCIONA PERFECTAMENTE**
- Sistema de combate base con 15+ skills
- 17 clases ocultas con requisitos aumentados (2x-5x)
- 80 pasivos únicos aplicándose correctamente
- Sistema de mascotas completo (18 especies, evoluciones, combate integrado)
- Tracking de ~40 acciones diferentes
- UI completa e interactiva con botones inline
- Bot desplegado en Azure App Service funcionando 24/7
- Balance de dificultad ajustado (XP exponencial, enemies +120-150% stats)

### ⏳ **LO QUE ESTÁ A MEDIAS**
- Tracking de acciones (40/100 implementadas)
- Skills combinadas (concepto definido en documento, sin código)
- Sistema de zonas (mencionado en diseño, sin implementar)

### ❌ **LO QUE FALTA COMPLETAMENTE**
- SkillUnlockDatabase.cs (30+ skills combinadas)
- LocationDatabase.cs (sistema de zonas)
- Boss battles con mecánicas especiales
- Leaderboards y estadísticas globales
- Sistema de eventos temporales

---

## 🎯 **ROADMAP SUGERIDO (Próximas 2 semanas)**

### **Semana 1: Fase 4 - Habilidades Combinadas**
- Día 1-2: Crear SkillUnlockDatabase con 30 skills
- Día 3-4: Implementar detector de combinaciones
- Día 5-6: Integrar skills en combate
- Día 7: Testing y ajustes de balance

### **Semana 2: Fase 5 - Expansión de Acciones**
- Día 1-2: Agregar 30 nuevas acciones trackeables
- Día 3-4: Implementar LocationDatabase con 6 zonas
- Día 5-6: Crear 10 boss battles especiales
- Día 7: Testing general y deploy a Azure

---

## 📝 **NOTAS DE DESARROLLO**

### **Commits Importantes:**
```
4525537 - FASE 1: Rebalanceo completo de dificultad ✅
c49740b - FASE 2: Sistema de Mascotas 100% ✅
cd12f39 - FASE 3: Expansión de 17 Clases Ocultas ✅
1ac6b74 - Fix: Deploy en infraestructura de producción ✅
```

### **Archivos Clave del Proyecto:**
```
src/BotTelegram/RPG/
├── Models/
│   ├── RpgPlayer.cs           ✅ (Level, XP, Stats, Inventory, Pets)
│   ├── RpgPet.cs              ✅ (Bond, Loyalty, Evolution, Combat)
│   ├── HiddenClass.cs         ✅ (17 clases definidas)
│   ├── Passive.cs             ✅ (80 pasivos únicos)
│   ├── RpgSkill.cs            ✅ (15 skills base)
│   └── RpgEnemy.cs            ✅ (13+ enemigos balanceados)
├── Services/
│   ├── RpgCombatService.cs    ✅ (Combate + Pets integrado)
│   ├── ActionTrackerService.cs ⏳ (40/100 acciones)
│   ├── HiddenClassDatabase.cs ✅ (17 clases)
│   ├── PassiveDatabase.cs     ✅ (80 pasivos)
│   ├── PetDatabase.cs         ✅ (18 especies)
│   ├── PetTamingService.cs    ✅ (Domado completo)
│   ├── SkillDatabase.cs       ✅ (15 skills base)
│   ├── EquipmentDatabase.cs   ✅ (20+ items)
│   ├── SkillUnlockDatabase.cs ❌ (NO EXISTE - FASE 4 PENDIENTE)
│   └── LocationDatabase.cs    ❌ (NO EXISTE - FASE 5 PENDIENTE)
└── Commands/
    ├── RpgCommand.cs          ✅ (Menú principal)
    ├── PetsCommand.cs         ✅ (Gestión de mascotas)
    ├── RpgStatsCommand.cs     ✅ (Stats del jugador)
    ├── RpgSkillsCommand.cs    ✅ (Skills disponibles)
    └── RpgCountersCommand.cs  ✅ (Action counters)
```

---

## ✅ **CONCLUSIÓN Y ESTADO FINAL**

### **Progreso Total del Proyecto:** 60% completado ✅

El sistema RPG está **funcional, balanceado y desplegado en producción (Azure App Service)**. 

**Fases Completadas (3/5):**
- ✅ **Fase 1:** Dificultad ajustada (XP exponencial, enemies buffados)
- ✅ **Fase 2:** Sistema de mascotas completo (18 especies, combate integrado)
- ✅ **Fase 3:** 17 clases ocultas + 80 pasivos únicos

**Fases Pendientes (2/5):**
- 🔄 **Fase 4:** Habilidades Combinadas (30+ skills nuevas)
- 🔄 **Fase 5:** Expansión de Acciones (60 acciones + zonas + bosses)

**Tiempo estimado para completar al 100%:** 7-10 días de desarrollo activo

---

**Fin del documento** | Última actualización: 13 de Febrero de 2026  
**Bot Status:** 🟢 ONLINE en https://bottelegram-rpg.azurewebsites.net
