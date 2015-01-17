Quark
====
Quark is a game development framework which utilizes high level modularity for building mainly role playing games using the base components it provides.

Quark mainly provides the following systems by default:

+ Casted Activities (`Spells`)
+ Time or Event Driven Activities (`Buffs`)
+ Targeting
+ Casting
+ Object Mutation (`Effects`)
+ Quark Controlled Objects (`Characters` and `Targetables`)

We take maximum advantage of object orientation, therefore the components in quark are very modular and reusable.

Let us begin with an example, which demonstrates the key concepts of Quark framework.
## **BOOM**
> **Note:** the following example and every asset used is included in the [Tutorial package][1]. 
> Tutorials about the `NearestCharacter` and `DamageEffect` will be included.

In this example, we will write a **Spell** and inspect it row by row to get your grasp on some of the terminology in Quark. To do so, first we must form the semantic model of our spell, which is what your spell does in your native language, say English.

The semantic model we will be focusing in this example is as follows:
> When the player hits Q key, the nearest object to the players character recieves 100 damage.

Your syntactic model is the representation of this semantic model on the platform you are building your game. 

Even though you are not familiar with anything below, this is the Quark representation of our **BOOM** model:

```csharp
public class BoomSpell : Spell
{
	public override string Name {
		get {
			return "BOOM";
		}
	}

	public override TargetMacro TargetMacro {
		get {
			return new NearestCharacter();
		}
	}

	protected override EffectCollection TargetingDoneEffects {
		get {
			return new EffectCollection {
				new DamageEffect (100)
			};
		}
	}
}
```
It may not mean anything to you right now, but when this spell is cast, it will do whatever we wanted the **BOOM** spell to do.

If you are lost, of course this spell does not cast itself, for that, we have a very simple `KeyBinding` class. We want our spell to be casted when the player hits `Q` key. To accomplish that, we invoke the following code at the beginning of our scene:
```csharp
new KeyBinding<BoomSpell>(KeyCode.Q).Register();
```
This is it. Our **BOOM** spell will damage the nearest character for 100 when the Q key is hit by the player.

####But How Does This Work?
**Magic.** No not really, let us start examining the code starting from the last piece, KeyBinding system is not the only way to let the player cast a spell but it is certainly one of the easiest ones. Every frame, Quark checks for user input and if the `Q` key was hit, it instantiates a new `BoomSpell` instance, and begins the cast procedure for it.

>Details about Casting procedure: **[Casting][2]**

Now, moving on to the actual part of the **BOOM**. We have defined 3 properties: ***Name***, ***TargetMacro*** and ***TargetingDoneEffects***. We will examine each of them now.

+ `Name`
Name of a `Spell` has nothing to do with the Quark, it is simply an identifier for your players, by default Quark returns the name of the `Spell` instances type. So if we haven't had overriden it, by default it would have `BoomSpell` as its value.

+ `TargetMacro`
Targeting plays a crucial role in a role playing game, as a `Spell` will usually require a target to be able to actually do something. Quark has a very neat solution for targeting: `TargetMacro`s. We simply implement our generic targeting logic in another class and when a `Spell` is being casted, simply an instance of this `TargetMacro` class will handle targeting. In a `Spell`, we just provide the appropriate macro object. For our example, our `Spell` needs the nearest object as its target, therefore we use the `NearestCharacter` macro.
> Details about targeting can be found in its own article
> **Note** that we do not have to know how the `NearestCharacter` macro is implemented, it simply finds the nearest character to the caster and adds it to the target list of its caller.

+ `TargetingDoneEffects`
Event effects are the actual places where we define the behavior of the `Spell` we are writing. Quark had 8 default `Spell Events` at the time this document was being written. 
The one we will be using is the `TargetingDone` event, as our `Spell` needs a target and this event is handled right after the `TargetingMacro` returns.
Note that, if our spell had a projectile, say a fireball, we could not implement all of our logic in `TargetingDone` event, but for simplicity, **BOOM** spell can be ran right after a target is selected. So note here that you should be careful when you are choosing which event handlers to implement for your spells.
Back to **BOOM**, our semantic model says that *the nearest object to the players character recieves 100 damage after it is casted*. 
The nearest object part is handled by the `TargetMacro`, after that, our `Spell` must deal 100 damage to that target. 
To achieve this, we can use the `DamageEffect`. 
This effect decreases the health stat of the given target when it is applied, it perfectly fits, we simply return an array containing a new `DamageEffect` which applies a constant damage of 100 when applied.
>Again note that the implementation of `DamageEffect` is abstracted away, it is a simple generic effect which decreases the health stat of the target `Character` and broadcasts this incident.

Simple as that! 

###Summary
In this article, we have taken a semantic model of a game action called **BOOM**, and represented that model by using an `Effect` and a `TargetMacro` in the form of a `Spell`.
Therefore we have touched upon the basic logic of the `Spell` and how to construct one using other very basic elements of Quark.
An important thing to notice that we haven't even written any code about **how** our spell should do something, we have simply written code for **what** should it do.
 
[1]: http://quark.fatihbakir.net/Packages/Tutorial
[2]: Casting.md