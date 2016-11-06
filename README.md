#Quark Framework

[![Build Status](https://api.travis-ci.org/FatihBAKIR/Quark.svg)](https://api.travis-ci.org/FatihBAKIR/Quark.svg)

![](http://quarkup.io/res/quark.md.png)

Quark is both a design pattern for developing in game interactions in a *behavioral* fashion for action based games and a framework built on top of Unity that incorporates the pattern.

An example *action* that deals 10 damage to the nearest object:

```csharp
public class BoomSpell : Spell
{
	public override TargetMacro TargetMacro {
		get {
			return new NearestCharacter(5);
		}
	}

	protected override EffectCollection<ICastContext> TargetingDoneEffects {
		get {
			return new EffectCollection<ICastContext> {
				new DamageEffect (10)
			};
		}
	}
}
```

List of features
----
Currently, the following features of Quark are present in the framework:

+ [Quark Entities][read_Objects]
+ [Scene Manipulation][read_Effects]
+ [Targeting][read_Targeting]
+ [Time or Event Driven Activities][read_Buffs]
+ [Projectiles][read_Projectiles]
+ [Actions][read_Actions]
+ Late evaluated Expressions


Installation / Using
----
There is no actual installation, you can simply build the project and include `Quark.dll` as an asset in your Quark game, or just run the to-be-provided `Quark.unityasset` file.

For usage part, if you used the Unity Asset way, it is easy. Simply drag the `Quark.Main` prefab to your hierarchy and your scene is ready to use Quark. 

If you included the dll, you should create a `Quark.Main` object. For details, please refer to [this article][1]

Contribute
----
Source and issues of Quark are held in [this GitHub repository][2].

Support
----
If you need help, you can use the GitHub issues or [send me an email][3]

License
----
Quark is licensed under [Apache V2][4] License.

[1]: http://read.quarkup.io/content/quark_framework/using_quark_framework.html
[2]: https://github.com/FatihBAKIR/Quark
[3]: mailto:fatih@quarkup.io
[4]: http://www.apache.org/licenses/LICENSE-2.0
[5]: http://quarkup.io/Doc/Introduction.md

[read_Objects]: http://read.quarkup.io/content/quark_pattern/objects.html
[read_Effects]: http://read.quarkup.io/content/quark_pattern/scene_manipulation.html
[read_Targeting]: http://read.quarkup.io/content/quark_pattern/targeting.html
[read_Buffs]: http://read.quarkup.io/content/quark_pattern/buffs.html
[read_Projectiles]: http://read.quarkup.io/content/quark_pattern/projectile_controlling.html
[read_Actions]: http://read.quarkup.io/content/quark_pattern/actions.html

