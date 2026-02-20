# Plan de Mejora: Sistema de Progresión Orgánica y Combate Profundo
## 🎯 Objetivo
Mejorar el sistema de progresión para que cada jugador crezca según su estilo de juego, premiando el esfuerzo con desbloqueos únicos. Hacer que las decisiones en combate sean más importantes y estratégicas sin romper el balance actual.
## 📊 Análisis del Sistema Actual
### ✅ Lo que funciona bien
1. **ActionTrackerService** - Sistema robusto de tracking con 12+ tipos de acciones
2. **Clases Ocultas** - 6 clases con requisitos únicos (Beast Tamer, Shadow Walker, Divine Prophet, Necromancer Lord, Elemental Sage, Blade Dancer)
3. **Pasivas** - 12+ pasivas desbloqueables basadas en acciones específicas
4. **Skills Combinadas** - ~25 skills desbloqueables por combinaciones de acciones
5. **51 Clases Desbloqueables** - Sistema de Fase 4 funcionando
### ⚠️ Áreas de Mejora Identificadas
#### Problema 1: Tracking de Acciones Incompleto en Combate
**Ubicación:** `RpgCombatService.Actions.cs`
* Solo hay 4 menciones de tracking: `physical_attack`, `magic_attack`, `critical_hit`
* Faltan muchas acciones importantes: `dodge_success`, `block_success`, `counter_attack`, `perfect_parry`, etc.
* El sistema de desbloqueos requiere acciones que no se están trackeando
#### Problema 2: Combate con Decisiones Poco Impactantes
* Las acciones de combate no tienen suficiente diferenciación estratégica
* No hay sistema de "build" visible (guerrero tanque vs guerrero DPS vs guerrero crítico)
* Falta feedback sobre qué acciones te acercan a desbloqueos
#### Problema 3: Progresión Invisible
* El jugador no sabe qué está cerca de desbloquear
* No hay incentivo para variar el estilo de juego
* Las pasivas se desbloquean "en secreto"
## 🔧 Solución Propuesta
### Fase 1: Completar el Tracking de Combate (2-3 horas)
#### 1.1 Agregar Tracking a Todas las Acciones de Combate
**Archivo:** `src/BotTelegram/RPG/Services/RpgCombatService.Actions.cs`
**Acciones a trackear:**
```csharp
// Ataques básicos
- "physical_attack" ✅ (ya existe)
- "magic_attack" ✅ (ya existe) 
- "critical_hit" ✅ (ya existe)
// Defensas (AGREGAR)
- "dodge_success" - Esquivar exitoso
- "dodge_fail" - Falló el esquive
- "block_success" - Bloqueo exitoso
- "block_fail" - Falló el bloqueo
- "perfect_parry" - Bloqueo que reduce 100% daño
// Contraataques (AGREGAR)
- "counter_attack" - Contraataque exitoso
- "riposte" - Contraataque después de parry perfecto
// Combos (AGREGAR)
- "combo_3x" - Combo de 3+ hits
- "combo_5x" - Combo de 5+ hits
- "combo_10x" - Combo de 10+ hits
- "combo_20x" - Combo de 20+ hits
// Skills especiales (AGREGAR)
- "skill_used" - Cualquier skill usada
- "ultimate_used" - Skill ultimate usada
- "heal_cast" - Hechizo de curación
- "buff_cast" - Aplicar buff
- "debuff_cast" - Aplicar debuff
// Resultados (AGREGAR)
- "enemy_kill" - Matar enemigo
- "survived_battles" - Sobrevivir combate
- "low_hp_victory" - Ganar con <30% HP
- "flawless_victory" - Ganar sin recibir daño
// Daño (AGREGAR)
- "damage_dealt" - Total daño infligido
- "damage_taken" - Total daño recibido
- "damage_blocked" - Total daño bloqueado
- "damage_dodged" - Total daño esquivado
// Recursos (AGREGAR)
- "mana_spent" - Mana gastado
- "mana_regen" - Mana regenerado
- "hp_healed" - HP curado
```
**Implementación:**
1. Buscar cada acción de combate en `RpgCombatService.Actions.cs`
2. Agregar `tracker.TrackAction(player, "action_id", count)` después de cada acción exitosa
3. Para acciones acumulativas (daño), usar el parámetro `count`
#### 1.2 Ejemplo de Implementación
```csharp
// Antes
public CombatResult Dodge(RpgPlayer player, Enemy enemy)
{
    var result = new CombatResult();
    var dodgeChance = CalculateDodgeChance(player);
    var roll = Random.Shared.Next(1, 101);
    
    if (roll <= dodgeChance)
    {
        result.Success = true;
        result.Message = "💨 ¡Esquivaste el ataque!";
        return result;
    }
    // ...
}
// Después
public CombatResult Dodge(RpgPlayer player, Enemy enemy)
{
    var tracker = new ActionTrackerService(_rpgService);
    var result = new CombatResult();
    var dodgeChance = CalculateDodgeChance(player);
    var roll = Random.Shared.Next(1, 101);
    
    if (roll <= dodgeChance)
    {
        result.Success = true;
        result.Message = "💨 ¡Esquivaste el ataque!";
        
        // NUEVO: Trackear esquive exitoso
        tracker.TrackAction(player, "dodge_success");
        
        // NUEVO: Trackear daño esquivado
        var wouldBeDamage = CalculateEnemyDamage(enemy);
        tracker.TrackAction(player, "damage_dodged", wouldBeDamage);
        
        return result;
    }
    else
    {
        // NUEVO: Trackear fallo de esquive
        tracker.TrackAction(player, "dodge_fail");
    }
    // ...
}
```
### Fase 2: Mejorar Profundidad del Combate (4-6 horas)
#### 2.1 Sistema de Stance/Postura
**Nuevo concepto:** El jugador puede elegir una postura que afecta su estilo de combate
**Posturas propuestas:**
```csharp
public enum CombatStance
{
    Balanced,      // Sin bonos ni penalizaciones (default)
    Aggressive,    // +20% daño, -20% defensa
    Defensive,     // +30% defensa, -15% daño
    Berserker,     // +40% daño, no puede bloquear/esquivar
    Evasive,       // +30% esquive, -20% HP
    Counter,       // +50% contraataque, -10% ataque
    Arcane         // +25% daño mágico, +20% costo de mana
}
```
**Implementación:**
* Agregar `CombatStance ActiveStance` al modelo `RpgPlayer`
* Crear botón en menú de combate: "⚙️ Cambiar Postura"
* Aplicar modificadores en cálculos de daño/defensa
* **IMPORTANTE:** Cambiar postura NO consume turno (se hace antes del turno)
#### 2.2 Sistema de Momentum/Flujo
**Concepto:** Recompensa por mantener un estilo consistente
```csharp
public class CombatMomentum
{
    public int AttackStreak { get; set; }      // Ataques consecutivos
    public int DefenseStreak { get; set; }     // Defensas consecutivas  
    public int DodgeStreak { get; set; }       // Esquives consecutivos
    public int MagicStreak { get; set; }       // Magias consecutivas
    
    public int CurrentBonus { get; set; }      // +2% por cada acción en streak
    public int MaxBonus { get; set; } = 30;    // Máximo +30%
}
```
**Mecánica:**
* Cada acción del mismo tipo aumenta el streak
* Streak otorga +2% efectividad por acción (max +30%)
* Cambiar de tipo resetea el streak
* Incentiva especialización en combate
#### 2.3 Sistema de Weak Points / Puntos Débiles
**Concepto:** Enemigos tienen puntos débiles que cambian cada turno
```csharp
public class EnemyWeakness
{
    public DamageType WeakTo { get; set; }     // Physical, Magical, Fire, etc.
    public int BonusDamage { get; set; }       // +50% si aciertas el weak point
    public int TurnsRemaining { get; set; }    // Cambia cada 2-3 turnos
}
```
**Feedback en combate:**
```warp-runnable-command
🎯 PUNTO DÉBIL DETECTADO
🔥 El enemigo es vulnerable a FUEGO este turno (+50% daño)
⏱️ Cambiará en 2 turnos
```
**Impacto:**
* Recompensa observación y adaptación
* Hace que elegir el ataque correcto sea crítico
* Incentiva tener skills de diferentes tipos
#### 2.4 Sistema de Combos Visuales
**Concepto:** Mostrar progreso de combo en tiempo real
```warp-runnable-command
━━━━━━━━━━━━━━━━━━━━━
⚔️ COMBO x5  [████████░░] +10% daño
⚡ 3 hits más para desbloquear Whirlwind!
━━━━━━━━━━━━━━━━━━━━━
```
### Fase 3: Progresión Visible y Motivante (3-4 horas)
#### 3.1 Panel de Progreso Hacia Desbloqueos
**Nuevo comando:** `/progreso` o botón "📈 Mi Progreso"
**Muestra:**
```warp-runnable-command
📊 TU PROGRESO DE DESBLOQUEOS
🔓 PRÓXIMO A DESBLOQUEAR:
🌟 Shadow Strike (Skill)
   ✅ critical_hit: 215/200
   🔸 dodge_success: 87/150 (58%)
   ━━━━━━░░░░ 78%
   
🐺 Beast Tamer (Clase Oculta)
   ✅ pet_beast: 250/250
   ✅ calm_beast: 180/150
   🔸 tame_beast: 312/500 (62%)
   🔸 meditation: 156/200 (78%)
   🔸 beast_kills: 543/800 (68%)
   ━━━━━━━░░░ 73%
💎 Iron Skin (Pasiva)
   🔸 damage_taken: 687/1000 (69%)
   ━━━━━━░░░░ 69%
```
#### 3.2 Notificaciones de Progreso en Combate
**Durante combate, mostrar:**
```warp-runnable-command
⚔️ Atacaste al Bandido
🩸 42 de daño
📊 Progreso:
• critical_hit: 215/200 ✅ COMPLETADO
• physical_attack: 87/100 (87%)
🎉 ¡NUEVA SKILL DESBLOQUEADA!
⚡ Shadow Strike
   "Ataque desde las sombras (200% daño)"
```
#### 3.3 Sistema de Achievements / Logros
**Nuevo archivo:** `AchievementDatabase.cs`
```csharp
public class Achievement
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Emoji { get; set; }
    public AchievementTier Tier { get; set; }  // Bronze, Silver, Gold, Platinum
    public Dictionary<string, int> Requirements { get; set; }
    public AchievementReward Reward { get; set; }
}
public enum AchievementTier { Bronze, Silver, Gold, Platinum, Diamond }
public class AchievementReward
{
    public int GoldBonus { get; set; }
    public int XPBonus { get; set; }
    public string? UnlockSkill { get; set; }
    public string? UnlockPassive { get; set; }
    public string? UnlockTitle { get; set; }  // Título decorativo
}
```
**Ejemplos:**
```csharp
new Achievement
{
    Id = "crit_master_bronze",
    Name = "Crítico Aprendiz",
    Emoji = "💥",
    Tier = AchievementTier.Bronze,
    Requirements = new() { { "critical_hit", 100 } },
    Reward = new() { GoldBonus = 500, XPBonus = 200 }
},
new Achievement
{
    Id = "crit_master_platinum",
    Name = "Maestro del Crítico",
    Emoji = "💎",
    Tier = AchievementTier.Platinum,
    Requirements = new() { { "critical_hit", 10000 } },
    Reward = new() { 
        GoldBonus = 50000, 
        XPBonus = 10000,
        UnlockPassive = "lethal_precision",  // +15% crit damage
        UnlockTitle = "el Implacable"
    }
}
```
### Fase 4: Build Diversity / Diversidad de Builds (5-7 horas)
#### 4.1 Sistema de Especialización
**Concepto:** El jugador ve estadísticas de su "build" actual
```warp-runnable-command
📊 TU PERFIL DE COMBATE
Estilo dominante: ⚔️ GUERRERO CRÍTICO
Distribución de acciones:
━━━━━━━━━━━━━━━━━━━━
⚔️ Ataque Físico    ████████░░ 45%
💨 Esquive          ███░░░░░░░ 15%
🛡️ Bloqueo         ██░░░░░░░░ 10%
🔮 Magia           ████░░░░░░ 20%
🧪 Items/Soporte   ██░░░░░░░░ 10%
Puntos fuertes:
✅ Alto daño burst (+35%)
✅ Críticos frecuentes (+22% chance)
✅ Evasión decente
Puntos débiles:
⚠️ Defensa baja (-15%)
⚠️ Sin sustain (0% lifesteal)
Recomendaciones:
💡 Considera desbloquear "Life Steal" (200 kills)
💡 Intenta bloquear más para desbloquear "Iron Fortress"
```
#### 4.2 Arquetipos Sugeridos
**Sistema que detecta y sugiere builds:**
```csharp
public enum Archetype
{
    GlassCannon,      // Alto daño, baja defensa
    Tank,             // Alto HP/defensa, bajo daño
    Bruiser,          // Balance daño/defensa
    Assassin,         // Alto crítico, esquive
    Mage,             // Magia pura
    Battlemage,       // Magia + físico
    Support,          // Curaciones/buffs
    Hybrid            // Sin especialización clara
}
```
**Detección automática:**
* Analiza las últimas 100 acciones del jugador
* Sugiere skills/pasivas que complementen su estilo
* Muestra qué arquetipos están disponibles
### Fase 5: Mejoras de UX en Combate (2-3 horas)
#### 5.1 Información Detallada en Hover (Botones)
**Cuando el jugador ve las opciones de combate:**
```warp-runnable-command
⚔️ Atacar
   Daño estimado: 45-67
   Precisión: 85%
   Costo: 10 energía
   Trackea: physical_attack
   
🛡️ Bloquear
   Reducción: 40-60%
   Probabilidad: 70%
   Costo: 5 energía
   Trackea: block_success
   Bonus: +30% contraataque
```
#### 5.2 Combat Log Expandido
**Guardar historial de últimos 10 turnos:**
```warp-runnable-command
📜 HISTORIAL DE COMBATE
Turno 5: Atacaste - 52 dmg ⚔️
Turno 4: Bloqueaste - 28 dmg reducido 🛡️
Turno 3: Crítico! - 94 dmg 💥
Turno 2: Esquivaste ✨
Turno 1: Usaste poción - +30 HP 🧪
📊 Estadísticas del combate:
• Daño total: 298
• Daño recibido: 156  
• Críticos: 2
• Precisión: 80%
```
#### 5.3 Predicción de Resultados
**Mostrar probabilidades antes de actuar:**
```warp-runnable-command
¿Qué quieres hacer?
⚔️ Atacar
   🎲 85% de acertar
   💀 67% de matar en este turno
   ⚡ 12% de crítico
   
🔮 Fireball  
   🎲 95% de acertar
   💀 92% de matar en este turno
   ⚡ 8% de crítico
   💠 Cuesta 30 mana (te quedarían 45)
```
## 📋 Plan de Implementación Recomendado
### Prioridad ALTA (Hacer primero)
1. ⏳ **Fase 1.1**: Completar tracking de combate (2-3h)
2. ⏳ **Fase 3.1**: Panel de progreso visible (2-3h)
3. ⏳ **Fase 3.2**: Notificaciones en combate (1-2h)
**Impacto:** El jugador VERÁ su progreso y entenderá qué hacer
**Esfuerzo:** 5-8 horas total
**Riesgo:** BAJO - No rompe nada existente
### Prioridad MEDIA (Hacer después)
4. ⏳ **Fase 2.1**: Sistema de Stance (3-4h)
5. ⏳ **Fase 2.4**: Combos visuales (2h)
6. ⏳ **Fase 4.1**: Perfil de combate (3-4h)
**Impacto:** Combate más estratégico y personalizado
**Esfuerzo:** 8-10 horas total
**Riesgo:** MEDIO - Requiere balanceo
### Prioridad BAJA (Opcional)
7. ⏸️ **Fase 2.2**: Sistema de Momentum (4-5h)
8. ⏸️ **Fase 2.3**: Weak points (3-4h)
9. ⏸️ **Fase 3.3**: Achievements (5-6h)
10. ⏸️ **Fase 5**: Mejoras de UX (4-5h)
**Impacto:** Pulido adicional
**Esfuerzo:** 16-20 horas total
**Riesgo:** BAJO-MEDIO
## 🎯 Resultado Esperado
### Antes
* Jugador ataca repetidamente sin pensar
* No sabe qué desbloquear ni cómo
* Combate es: atacar > atacar > atacar > skill > atacar
* Todas las builds se sienten iguales
### Después
* Jugador toma decisiones informadas ("Si bloqueo 20 veces más, desbloqueo Iron Fortress")
* Ve su progreso en tiempo real
* Combate tiene variedad: stance > weak point > combo > momentum
* Cada jugador tiene un build único y visible
* Las acciones tienen peso y significado
## 🔄 Compatibilidad
**✅ No rompe nada:**
* Todo el código nuevo es aditivo
* No modifica fórmulas de daño core
* No cambia sistema de clases existente
* Compatible con fase 12 actual
**⚠️ Requiere:**
* Migración de jugadores para agregar nuevos campos (CombatStance, Momentum, etc.)
* Recalcular progreso de algunos jugadores existentes
## 💡 Extras Opcionales
### Títulos Decorativos
```warp-runnable-command
José "el Implacable" Céspedes
Nivel 45 Shadow Walker
```
### Sistema de Reputación por Estilo
```warp-runnable-command
🏆 REPUTACIÓN
Gremio de Asesinos: ⭐⭐⭐⭐⭐ (Maestro)
  +15% daño crítico en misiones de gremio
  
Templo de Magos: ⭐⭐⭐░░ (Adepto)
  -10% costo de mana en ciudad
```
### Mentor System
```warp-runnable-command
Has dominado el estilo CRÍTICO
💡 Puedes entrenar a otros jugadores (futuro PvE cooperativo)
```

---

## ✅ Versión Ejecutable (Implementación Real)

### 1) Diccionario Maestro de `actionId` (evitar colisiones)

Usar un único catálogo de IDs para tracking y desbloqueos. Reglas:
- Nunca reutilizar IDs semánticamente distintos.
- Evitar duplicados como `fish` (crafting vs acción de ciudad).
- Mantener compatibilidad con IDs legacy ya usados en `ActionTrackerService`.

**IDs canónicos sugeridos para nuevas acciones de Fase 12+**

```csharp
// Aventura
"adventure_risky"
"adventure_stealth"
"adventure_social"

// Personaje
"train_mind"
"train_body"
"study"

// Mundo/Ciudad
"deep_meditation"   // evita colisión con "meditation"
"fish_action"       // intento
"fish_catch"        // éxito
"investigate"

// Social/Ciudad
"trade"
"diplomacy"
"tavern"
```

### 2) Orden exacto de implementación por sprint

## Sprint 1 (Impacto alto, bajo riesgo)
Objetivo: tracking completo + progreso visible.

**Paso 1 — Completar tracking de combate**
1. `src/BotTelegram/RPG/Services/RpgCombatService.Actions.cs`
2. `src/BotTelegram/RPG/Services/ActionTrackerService.cs`
3. `src/BotTelegram/Handlers/CallbackQueryHandler.cs` (si hay callbacks de combate que no trackean)

**Paso 2 — Progreso visible**
4. `src/BotTelegram/RPG/Commands/RpgStatsCommand.cs` o comando equivalente de progreso
5. `src/BotTelegram/RPG/Commands/RpgCommand.cs` (botón `📈 Mi Progreso`)
6. `src/BotTelegram/Core/CommandRouter.cs` (si se agrega `/progreso`)

**Paso 3 — Notificaciones de desbloqueo**
7. `src/BotTelegram/RPG/Services/SkillUnlockDatabase.cs`
8. `src/BotTelegram/RPG/Services/PassiveUnlockDatabase.cs`
9. `src/BotTelegram/RPG/Services/ClassUnlockDatabase.cs`

## Sprint 2 (Profundidad de combate)
Objetivo: stance + combos visuales + perfil build.

1. `src/BotTelegram/RPG/Models/RpgPlayer.cs` (campos nuevos)
2. `src/BotTelegram/RPG/Models/` (nuevo enum `CombatStance` si aplica)
3. `src/BotTelegram/RPG/Services/RpgCombatService.*.cs` (modificadores stance/combos)
4. `src/BotTelegram/Handlers/CallbackQueryHandler.cs` (callbacks stance)
5. `src/BotTelegram/RPG/Commands/RpgCommand.cs` (UI postura y feedback)

### 3) Campos nuevos y compatibilidad (JSON)

Agregar en `RpgPlayer` con defaults seguros:

```csharp
public string ActiveStance { get; set; } = "Balanced";
public int AttackStreak { get; set; } = 0;
public int DefenseStreak { get; set; } = 0;
public int DodgeStreak { get; set; } = 0;
public int MagicStreak { get; set; } = 0;
public int MomentumBonus { get; set; } = 0;
```

Notas:
- JSON existente seguirá cargando por defaults (sin migración destructiva).
- No renombrar campos legacy sin estrategia de backward compatibility.

### 4) Criterios de aceptación por fase

#### A. Tracking completo (Done cuando…)
- Se registran mínimo 20 acciones de combate distintas.
- `dotnet build --no-restore` = 0 errores.
- Se valida manualmente 1 combate por cada acción crítica (`dodge_success`, `block_success`, `counter_attack`, `combo_3x`, `enemy_kill`).

#### B. Progreso visible (Done cuando…)
- Existe vista de progreso con al menos 3 objetivos cercanos (skill/pasiva/clase).
- Cada objetivo muestra progreso `actual/requerido` y barra %.
- Al completar requisito, se emite notificación de desbloqueo en sesión.

#### C. Stance y profundidad (Done cuando…)
- Cambio de postura disponible en UI de combate.
- La postura modifica daño/defensa/esquive según definición.
- No consume turno (validado con test manual en combate).

### 5) Checklist técnico (copiar/pegar en cada iteración)

- [ ] IDs de acciones sin duplicados semánticos.
- [ ] `GetActionName()` actualizado con nuevas acciones.
- [ ] `SkillUnlockDatabase`/`PassiveUnlockDatabase` usan los IDs correctos.
- [ ] Menú y callbacks registrados (`RpgCommand` + `CallbackQueryHandler`).
- [ ] Validación FSM (`StateManager.IsActionAllowed`) aplicada donde corresponda.
- [ ] Build en verde (0 errores).
- [ ] `BuildInfo.cs` actualizado (`FallbackCommit`, `FallbackDate`, `BotVersion`).

### 6) Checklist de QA manual mínimo

1. Ejecutar 5 combates usando: atacar, bloquear, esquivar, skill, item.
2. Confirmar incremento en contadores de acciones esperadas.
3. Verificar que el panel de progreso refleja los cambios inmediatamente.
4. Confirmar al menos 1 desbloqueo real de skill/pasiva/clase.
5. Verificar que no se rompe PvP/Guild por campos nuevos en jugador.

### 7) Riesgos y mitigación

- **Riesgo:** inflación de oro/XP por nuevas acciones.
    - **Mitigación:** límites por energía + validaciones de cooldown por acción.
- **Riesgo:** desbloqueos demasiado rápidos por umbrales bajos.
    - **Mitigación:** ajustar requisitos en `SkillUnlockDatabase` por telemetría de 1 semana.
- **Riesgo:** ruido de notificaciones en combate.
    - **Mitigación:** notificar solo hitos (25%, 50%, 75%, 100%) y desbloqueo final.

### 8) Definición de “Listo para producción”

Se considera listo cuando:
- Build estable (0 errores) y sin regresiones críticas de combate.
- Al menos 10 jugadores reales pueden progresar sin bloqueos de unlock.
- Logs confirman tracking consistente de acciones durante 24h.
- Documentación de roadmap actualizada con commit real de fase.
