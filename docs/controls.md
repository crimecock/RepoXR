# RepoXR controls on PC (R.E.P.O. 0.5.0)

**English** · [Русский](controls.ru.md) · On Quest: [repo-quest-vr controls](https://github.com/crimecock/repo-quest-vr/blob/repo-0.5.0/docs/controls.md) (menu on ≡, X mutes)

RepoXR's default bindings (action map `VRInputs`) for Meta Quest / Oculus Touch controllers. Valve Index, HTC Vive and
Windows Mixed Reality have their own RepoXR profiles with the same actions on similar buttons. Everything can be
rebound in *Settings → Controls* (RepoXR lists its actions there). Left-handed: VR Settings → Gameplay → *Dominant Hand* →
*Left* swaps the hands.

> From RepoXR 1.2.3's code and action map and the 0.5.0 port's changes. Checked in VR on PC (SteamVR, Quest
> controllers): everything below works. Unlike
> the Quest build ([repo-quest-vr](https://github.com/crimecock/repo-quest-vr)), where the menu is on the ≡ button and X
> mutes, the PC build keeps RepoXR's bindings.

## Buttons

| Controller | Action |
|---|---|
| **Left stick** | Move |
| **Left stick, click** | Chat |
| **Left stick, hold (0.3 s)** | Expression wheel |
| **Left trigger** | Sprint |
| **Left grip** | Rotate the held object (with the right stick) |
| **X** | Menu (pause) |
| **Y** | Reset height |
| **Right stick left/right** | Turn (smooth by default; 45° snap turns: VR Settings → Input → *TurnProvider*) |
| **Right stick up/down** | Holding an object: push or pull it; in menus and lists: scroll |
| **Right stick, click** | Crouch |
| **Right trigger** | Grab (grab beam) |
| **Right grip** | Interact (doors, buttons, levers); fire a gun; horn on a scooter |
| **A** | Jump |
| **B** | Tumble |

## With your hands

- **Map:** bring a hand to where the map hangs on your belt (at the hip; it moves to the other hip while you hold
  something) or to your shoulder. When the controller buzzes, hold that hand's **grip**: the map is in your
  hand; let go and it goes away. Not while sprinting.
- **Flashlight on the head:** bring the other (non-dominant) hand to the side of your head and press **grip**; again to
  take it back.
- **Inventory (3 slots):** *store* — hold an item with the beam, the slots come forward, point the beam with the item
  at a slot; *take out* — put your dominant hand into a slot and press the **right trigger**.
- **Expressions:** hold the left stick to open the wheel, point a hand at an expression and press **that hand's
  trigger**. Holding the stick again closes the wheel.
- **Menus and UI:** the pointer from your hand and the **trigger**. RepoXR's keyboard appears when text is needed
  (lobby password, save name).

## Grabbing

Works on PC and Quest ([what was fixed](repo-0.5.0.md#grabbing-what-broke-and-how-it-was-fixed)). Point and hold the **right trigger**: the beam holds the object while the trigger is held. An item taken out of an
inventory slot: with the game's *Settings → Gameplay → Item Unequip Auto Hold* (new in 0.5.0, on by default) it stays
on the beam after you let go, until you press again.

## Tumble climbing

Needs the **Tumble Climb** upgrade from the shop and some stamina. Press **B** (tumble), point at a wall or floor of
the level (not at objects) within about 3 m and hold the **right trigger** to latch on. Right stick up/down pulls you in
or lets go; swing by moving. You are pulled where your hand points (checked on PC).

## Weapons

- **Guns:** grab one with the beam and press the **right grip**. The gun points along the grab beam from your hand: aim
  with your hand.
- **Melee** (bat, sword, frying pan...): hold it with the beam and swing.
- **Whip** (new in 0.5.0; alpha 3): hold it with the **right trigger** and use your hand like a real whip.
  - *Crack:* snap the controller back and then forward, from the side or from over the shoulder. The crack goes where
    your hand points.
  - *Fling:* once the whip has caught something (an item or an enemy), swing your hand to throw it.
  - Don't hold the **left grip** (rotate) meanwhile: the game turns the gestures off while you rotate.
  - Too easy or too hard? VR Settings → Input → *Whip Sideways Sensitivity* (default 0.5) and *Whip Crack Sensitivity*
    (default 1).

## Vehicles

Scooter, snow bike (Arctic), golf cart (Desert).

- **Get on / off:** point the beam at the seat or handlebar and press the **right trigger** once (a toggle, no need to
  hold; press again to get off).
- **Drive:** **left stick** forward to accelerate, back to brake and reverse, left/right to steer; far to a side to
  drift. The scooter and snow bike go where your hand points (or your head: VR Settings → Input → *Vehicle direction source*).
- **Horn:** **right grip** on a scooter.

## Rebinding and free actions

*Settings → Controls* lists RepoXR's actions: point at one, press the trigger, then press the new button. Some actions
have no button by default and can be given one there: **Inventory 1–3** (take an item out of a slot), **Expression
1–6** (play an expression directly), **Push To Talk**, **Chat Delete**. *Reset* in that page brings back the defaults.
Hold / toggle under some actions (crouch, sprint, grab...) chooses whether the button has to be held.

## When you are dead

**Right / left trigger:** next / previous player while spectating.

## Seated and voice

- **Seated:** sit the way you will play and press **Y**. Turn off **RoomscaleCrouch** in VR Settings → Gameplay, or
  looking down counts as crouching.
- **Voice:** the microphone follows the game's settings (Settings → Audio). Mute (ToggleMute) and push-to-talk have no
  default buttons on PC: bind them in *Settings → Controls*.
