Thank you for purchasing Aim Assist Pro, I hope it helps you take your shooter game to the next level. 

To understand the integration better, please refer to the documentation and the example scenes inside Scenes folder. 
There are examples for Character Controller and Rigidbody based control, with one scene using the old Input Manager and the others using the new Input System. 

If you wish to use the new Input System, accept the Unity Editor's offer to upgrade to that when it is prompted during asset import. You may have to reimport the asset after the restart.
Example scenes are available in Scenes > Input System. 

If you wish to use the old Input Manager, decline the editor's offer, in which case the only example scene available is in Scenes > Input Manager.

To pick which aim assists to use in game, hit Esc / controller Start them click on the aim assist buttons to the right.

As the code only does calculations, it should work with any third party input system that is available for Unity. 
The project doesn't use any artwork apart from the demos so it works with any rendering pipeline. The demo scenes are using Unity's default surface shader.

Note that there are example scenes only for a controller. That is because input handling is outside of the scope of the asset. It just helps you aim better. 
However the asset should work on any platform that runs Unity and you manage to put a shooter game together.

If your project doesn't use the Input System yet, enable that by either accepting it from Unity's dialogue, or going to 
File > Build Settings > Player Settings > (scroll down) configuration > Active Input Handling > Input System Package (new). 
If you then want to try out the scenes with Input Manager, enable the Input manager here. The option "Both" will just enable the input system (this is how the example script is written).  

If you have any questions or concerns, don't hesitate to contact me at amsiamun.dev@gmail.com.