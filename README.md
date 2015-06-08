#Quark Framework

![](https://api.travis-ci.org/FatihBAKIR/Quark.svg)

![](http://quarkup.io/res/quark.md.png)
[http://quarkup.io][6]

>This page contains a summary of the project, an extended version of this can be found [here][5]

Quark is a game development framework which utilizes high level modularity for building mainly role playing games using the base components it provides.

For example, a `Spell` that deals 10 damage to nearest character is defined like this:
```csharp
public class BoomSpell : Spell
{
	public override TargetMacro TargetMacro {
		get {
			return new NearestCharacter(5);
		}
	}

	protected override EffectCollection TargetingDoneEffects {
		get {
			return new EffectCollection {
				new DamageEffect (10)
			};
		}
	}
}
```

Features
----
Currently, the following features of Quark are present in the framework:

+ [Quark Controlled Objects][read_Objects]
+ [Scene Manipulation][read_Effects]
+ [Targeting][read_Targeting]
+ [Time or Event Driven Activities][read_Buffs]
+ [Projectiles][read_Projectiles]
+ [Actions][read_Actions]

Also there are more features on the way.

Installation / Using
----
There is no actual installation, you can simply build the project and include `Quark.dll` as an asset in your Quark game, or just run the to-be-provided `Quark.unityasset` file.

For usage part, if you used the Unity Asset way, it is easy. Simply drag the `Quark.Head` prefab to your hierarchy and your scene is ready to use Quark. 

If you included the dll, you should create a `Quark.Head` object. For details, please refer to [this article][1]

Contribute
----
Source and issues of Quark are held in [this GitHub repository][2].

Support
----
If you need help, you can use the GitHub issues or [send me an email][3]

License
----
Quark is licensed under [Apache V2][4] License.

[1]: http://quarkup.io/Doc/Usage.md
[2]: https://github.com/FatihBAKIR/Quark
[3]: mailto:fatih@quarkup.io
[4]: http://www.apache.org/licenses/LICENSE-2.0
[5]: http://quarkup.io/Doc/Introduction.md
[6]: http://quarkup.io/

[read_Objects]: http://read.quarkup.io/content/quark_pattern/objects.html
[read_Effects]: http://read.quarkup.io/content/quark_pattern/scene_manipulation.html
[read_Targeting]: http://read.quarkup.io/content/quark_pattern/targeting.html
[read_Buffs]: http://read.quarkup.io/content/quark_pattern/buffs.html
[read_Projectiles]: http://read.quarkup.io/content/quark_pattern/projectile_controlling.html
[read_Actions]: http://read.quarkup.io/content/quark_pattern/actions.html
