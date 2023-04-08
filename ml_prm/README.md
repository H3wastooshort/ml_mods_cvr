# Player Ragdoll Mod
This mod turns player's avatar into ragdoll puppet.

# Installation
* Install [latest MelonLoader](https://github.com/LavaGang/MelonLoader)
* Get [latest release DLL](../../../releases/latest):
  * Put `ml_prm.dll` in `Mods` folder of game
  
# Usage
* Press `R` to turn into ragdoll and back.

Optional mod's settings with [BTKUILib](https://github.com/BTK-Development/BTKUILib):
* **Switch ragdoll:** turns into ragdoll state and back, made for VR usage primarily.
* **Use hotkey:** enables/disables ragdoll state switch with `R` key; `true` by default.
* **Restore position:** returns to position of ragdoll state activation upon ragdoll state exit; `false` by default.
* **Use gravity:** enables/disables gravity for ragdoll; `true` by default.
* **Velocity multiplier:** velocity force multiplier based on player's movement direction; `2.0` by default.
* **Movement drag:** movement resistance; `1.0` by default.
* **Angular movement drag:** angular movement resistance; `0.5` by default.
* **Reset settings:** resets mod settings to default.

# Notes
* Incompatible with `Follow hips on IK override` option in AvatarMotionTweaker.
* Not suggested to activate fly mode with enabled ragdoll state.
* Can't be activated in worlds that don't allow flying and spawnables.