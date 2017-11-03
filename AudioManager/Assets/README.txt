# UnityAudioManager
The Unity Audio Management System based off of a Brackeys tutorial, but has been modified to have more features that weren't covered in the tutorial.

How To Use:
Simply import the asset package into your Unity project and make sure that the whole package is imported.

Within the prefabs folder specific to the package there is an Audio Manager. Drag this into your scene. Once in the scene click onto the manager and in the inspector view you will see 4 variables you can drop down.

The background music list and sfx list are the lists where you can add the specific audio clips to. The current music and prev music drop downs are there for debugging purposes within the inspector.

With both lists you can drop them down and increase the size to whatever you would like. From there you can add in the specific audio clips that you would like. Each element has a few variables that you can modify most similar to the audio source component within Unity itself. However, it also has a few new ones like the name, fade in/out speed and the fade bool.

The name can be used to play that specific element. The speed variables controls how fast the music fades in and out. The fade bool enables/disables the fade effect.

To intergrate with another script you can look at how the test input script works. In your script add the AudioManager namespace (using AudioManager). From this you should be able to access the singleton version of the audio manager (AudioManager.AudioManager.m_instance). From there you can call PlaySFX() or PlayMusic() each require either a string or int to be passed in. The string is the name that you entered in the element when setting up the audio manager and the int is the id value that the element has within the array.
