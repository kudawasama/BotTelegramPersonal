# 🌟 Sistema de Clases Ocultas y Tracking de Acciones

## 📋 Resumen

Sistema completo implementado que trackea todas las acciones del jugador y desbloquea automáticamente:
- ✅ **Pasivas permanentes** (30+ pasivas)
- ✅ **Clases ocultas** (6 clases con requisitos complejos)
- ✅ **Nuevas habilidades** desbloqueables por progreso

---

## 🎯 Clases Ocultas Implementadas

### 1. **🐺 Domador de Bestias** (Beast Tamer)
**Requisitos:**
- Acariciar bestias: 50 veces
- Calmar bestias: 30 veces
- Domar bestias: 100 veces
- Meditar: 100 veces
- Matar bestias: 200 veces

**Otorga:**
- Pasivas: Beast Whisperer, Beast Companion (+20% daño), Beast Empathy
- Skills: Tame Beast, Beast Fury, Beast Heal
- Stats: +5 STR, +10 DEX, +15 WIS, +10 CHA

---

### 2. **👤 Caminante de las Sombras** (Shadow Walker)
**Requisitos:**
- Matar desde el sigilo: 100 veces
- Golpes críticos: 500 veces
- Esquivar ataques: 300 veces
- Backstabs: 150 veces
- Usar Vanish: 50 veces

**Otorga:**
- Pasivas: Shadow Step (+50% crítico inicial), Night Vision, Silent Movement
- Skills: Shadow Strike (200% daño), Vanish, Shadow Clone
- Stats: +20 DEX, +5 INT, +5 CHA

---

### 3. **⛪ Profeta Divino** (Divine Prophet)
**Requisitos:**
- Curar: 500 veces
- Revivir aliados: 20 veces
- Bendecir: 100 veces
- Meditar: 300 veces
- Matar no-muertos: 200 veces

**Otorga:**
- Pasivas: Divine Blessing (+50% heals), Holy Aura (regen 5% HP), Resurrection
- Skills: Divine Intervention, Mass Heal, Holy Smite (300% vs undead)
- Stats: +10 INT, +20 WIS, +15 CHA

---

### 4. **💀 Señor Nigromante** (Necromancer Lord)
**Requisitos:**
- Lanzar magia oscura: 400 veces
- Invocar no-muertos: 200 veces
- Drenar vida: 300 veces
- Profanar: 100 veces
- Sacrificar HP: 50 veces

**Otorga:**
- Pasivas: Necrotic Touch (+20 daño), Lichdom (-50% daño físico), Soul Harvest (+20% XP)
- Skills: Raise Undead, Death Coil, Dark Pact
- Stats: +25 INT, -5 CON, +10 WIS

---

### 5. **🌊 Sabio Elemental** (Elemental Sage)
**Requisitos:**
- Hechizos de fuego: 200 veces
- Hechizos de agua: 200 veces
- Hechizos de tierra: 200 veces
- Hechizos de aire: 200 veces
- Combinar elementos: 100 veces

**Otorga:**
- Pasivas: Elemental Affinity (+30% resist), Elemental Mastery (-20% mana cost), Primal Force (+15% daño mágico)
- Skills: Elemental Blast, Elemental Shield, Meteor Storm
- Stats: +30 INT, +15 WIS

---

### 6. **⚔️ Danzante de Espadas** (Blade Dancer)
**Requisitos:**
- Combos de 10+ hits: 100 veces
- Combos de 20+ hits: 50 veces
- Parrys perfectos: 200 veces
- Esquivar: 500 veces
- Combates sin daño: 100 veces

**Otorga:**
- Pasivas: Blade Dancer (combo nunca se resetea), Flow State (+5% daño/hit), Graceful Fighter (+20% evasión)
- Skills: Blade Storm (5 hits), Perfect Counter, Dance of Death
- Stats: +15 STR, +25 DEX, +5 CON

---

## ✨ Sistema de Pasivas (30+ implementadas)

### Pasivas de Combate:
- **Critical Mastery** (+10% chance crítico) - 100 críticos
- **Lethal Strikes** (+25% daño crítico) - 500 críticos
- **Berserker Rage** (+15 daño físico) - Automático
- **Arcane Power** (+20 daño mágico) - Automático

### Pasivas de Supervivencia:
- **Iron Skin** (+50 MaxHP) - Recibir 1000 daño
- **Mana Font** (+30 MaxMana) - Automático
- **Tireless** (+20 MaxStamina) - Automático
- **Second Wind** (Auto-revive 30% HP) - Automático

### Pasivas Avanzadas:
- **Bloodlust** (+2% daño por 10% HP perdido, max 20%) - 20 victorias con <30% HP
- **Counter Master** (30% chance contraatacar al defender) - 100 contraataques
- **Thorns** (Devuelve 20% daño) - Automático
- **Life Steal** (Roba 15% daño físico) - 200 kills
- **Spell Vamp** (Roba 20% daño mágico) - Automático

### Pasivas de Recursos:
- **Treasure Hunter** (+25% loot drop) - 50 loots encontrados
- **Gold Magnate** (+30% oro) - 10000 oro acumulado
- **Fast Learner** (+20% XP) - Subir 10 niveles
- **Merchant Friend** (-15% precio tiendas) - Automático

---

## 🎮 Acciones Trackeadas

### Combate:
- `physical_attack` - Ataques físicos
- `magic_attack` - Ataques mágicos
- `critical_hit` - Golpes críticos
- `dodge_success` - Esquivar exitoso
- `defend` - Defender
- `counter_attack` - Contraatacar

### Progreso:
- `level_up` - Subir de nivel
- `enemy_kill` - Matar enemigo
- `boss_kill` - Matar jefe
- `beast_kills` - Matar bestias
- `undead_kills` - Matar no-muertos

### Exploración:
- `meditation` - Meditar
- `rest` - Descansar
- `explore` - Explorar
- `treasure_found` - Encontrar tesoro
- `loot_found` - Recoger loot

### Interacción con Bestias:
- `pet_beast` - Acariciar bestia
- `calm_beast` - Calmar bestia
- `tame_beast` - Domar bestia

### Combos y Skills:
- `combo_5plus` - Combo de 5+ hits
- `combo_10plus` - Combo de 10+ hits
- `combo_20plus` - Combo de 20+ hits
- `skill_used` - Usar habilidad
- `skill_{skillId}` - Usar habilidad específica

### Combate Avanzado:
- `stealth_kill` - Matar desde sigilo
- `backstab` - Ataque por la espalda
- `perfect_parry` - Parry perfecto
- `no_damage_combat` - Combate sin recibir daño
- `low_hp_victory` - Victoria con <30% HP

### Magia:
- `fire_spell_cast` - Lanzar hechizo de fuego
- `water_spell_cast` - Lanzar hechizo de agua
- `earth_spell_cast` - Lanzar hechizo de tierra
- `air_spell_cast` - Lanzar hechizo de aire
- `combo_spell` - Combinar elementos
- `dark_magic_cast` - Lanzar magia oscura
- `heal_cast` - Curar

### Recursos:
- `gold_earned` - Oro ganado
- `damage_taken` - Daño recibido

---

## 📊 Sistema de Verificación

El sistema verifica automáticamente después de cada acción:

```csharp
// Ejemplo de uso
var tracker = new ActionTrackerService(rpgService);

// Durante combate
tracker.TrackAction(player, "critical_hit");
// → Verifica si desbloquea "Critical Mastery"

// Después de matar 200 bestias
tracker.TrackAction(player, "beast_kills", 1);
// → Si completa los otros requisitos, desbloquea "Beast Tamer"

// Al meditar
tracker.TrackAction(player, "meditation");
// → Progreso hacia "Beast Tamer", "Divine Prophet", etc.
```

---

## 🎯 Flujo de Desbloqueo

```
1. Jugador realiza acción
   ↓
2. Se incrementa contador en ActionCounters
   ↓
3. Se verifica automáticamente todos los requisitos
   ↓
4. Si se cumplen requisitos de pasiva → Se desbloquea
   ↓
5. Si se cumplen TODOS los requisitos de clase oculta → Se desbloquea
   ↓
6. Se otorgan automáticamente:
   - Pasivas de la clase
   - Habilidades de la clase
   - Stats bonus (al activar la clase)
```

---

## 💡 Cómo Activar una Clase Oculta

```csharp
// Ver progreso hacia una clase
var progress = tracker.GetClassProgress(player, "beast_tamer");
// progress.CurrentProgress["pet_beast"] → 35/50
// progress.CurrentProgress["meditation"] → 100/100
// progress.RequirementsMet["meditation"] → true

// Ver porcentaje de completitud
var percentage = tracker.GetClassProgressPercentage(player, "beast_tamer");
// → 60.0% (3 de 5 requisitos cumplidos)

// Activar clase (aplica bonuses de stats)
tracker.ActivateHiddenClass(player, "beast_tamer");
// → +5 STR, +10 DEX, +15 WIS, +10 CHA
// → Recalcula MaxHP/Mana/Stamina

// Desactivar clase
tracker.DeactivateHiddenClass(player);
// → Remueve bonuses de stats
```

---

## 🔧 Integración en el Sistema Existente

### En RpgCombatService:
```csharp
// Ya está implementado el tracking básico
private void TrackAction(RpgPlayer player, string actionId)
{
    if (!player.ActionCounters.ContainsKey(actionId))
        player.ActionCounters[actionId] = 0;
    
    player.ActionCounters[actionId]++;
}

// Ejemplo de uso en combate
if (result.PlayerCritical)
{
    TrackAction(player, "critical_hit");
}
```

### Nuevas Acciones a Implementar en UI:
1. **Meditar** (recupera mana, trackea para múltiples clases)
2. **Acariciar Bestia** (en exploración con enemigos bestia)
3. **Calmar Bestia** (evita combate con bestias)
4. **Intentar Domar** (captura bestia si tienes la habilidad)

---

## 📱 UI a Agregar en CallbackQueryHandler

### Menú de Progreso de Clases:
```
🌟 CLASES OCULTAS

🐺 Domador de Bestias [60%]
→ Acariciar bestias: 35/50 ✅
→ Calmar bestias: 30/30 ✅
→ Domar bestias: 70/100
→ Meditar: 100/100 ✅
→ Matar bestias: 150/200

[Ver Detalles] [Más Clases]
```

### Menú de Pasivas:
```
✨ PASIVAS ACTIVAS

⚔️ Blade Dancer
   Combo nunca se resetea

💥 Critical Mastery
   +10% chance crítico

❤️ Regeneration
  Regeneras 5% HP por turno

[Ver Todas]
```

---

## ✅ Estado de Implementación

- [x] Modelo de Passive
- [x] Modelo de HiddenClass
- [x] Modelo de ClassUnlockProgress
- [x] ActionTrackerService completo
- [x] HiddenClassDatabase con 6 clases
- [x] PassiveDatabase con 30+ pasivas
- [x] Sistema de verificación automática
- [x] Sistema de activación de clases
- [x] Tracking básico en RpgCombatService
- [x] Campos en RpgPlayer (UnlockedPassives, UnlockedHiddenClasses, ActiveHiddenClass)
- [ ] Nuevas acciones de exploración (pet_beast, calm_beast, etc.)
- [ ] UI en CallbackQueryHandler para progreso
- [ ] UI para activar/desactivar clases ocultas
- [ ] Aplicación de pasivas en cálculos de daño/defensa

---

## 🚀 Próximos Pasos

1. **Integrar UI** - Agregar menús en CallbackQueryHandler
2. **Implementar acciones de exploración** - Meditar, interactuar con bestias
3. **Aplicar pasivas en combate** - Life Steal, Thorns, Bloodlust en cálculos
4. **Testing** - Probar desbloqueos en Telegram
5. **Balance** - Ajustar requisitos si son muy altos/bajos

---

## 📝 Ejemplo de Jugador Desbloqueando Beast Tamer

```
Día 1: Jugador empieza a jugar
→ Encuentra zona con lobos
→ Después de vencer, aparece botón "🐾 Acariciar Lobo"
→ Click → pet_beast: 1/50

Día 5: Sigue progresando
→ pet_beast: 50/50 ✅
→ calm_beast: 30/30 ✅
→ meditation: 100/100 ✅
→ Desbloquea pasiva "Beast Whisperer"
→ Ahora puede usar "Domar Bestia" en combate

Día 10: Usa Domar Bestia frecuentemente
→ tame_beast: 100/100 ✅
→ beast_kills: 200/200 ✅

🎉 ¡CLASE OCULTA DESBLOQUEADA!
"🐺 Domador de Bestias"

→ Obtiene pasivas: Beast Companion, Beast Empathy
→ Obtiene skills: Beast Fury, Beast Heal
→ Puede activar clase para +5 STR, +10 DEX, +15 WIS, +10 CHA
→ Su bestia domada ahora pelea a su lado (+20% daño)
```

---

¡Sistema completo listo para implementar en UI! 🎮
