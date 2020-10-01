# AudioManager
An AudioManager and pooler for Unity Engine.

### HOW TO USE
1. Install the .unitypackage or clone/download the repo into your assets.

1. Add the AudioManager script onto a GameObject of your choice in your scene.

1. Adjusts the pooling settings according to your needs.

1. From within your other scripts, just call AudioManager.PlaySound2D or AudioManager.PlaySound3D for 2D (UI) or 3D (Game) sounds respectively3 and pass the audio clip you wish to play as a parameter.

1. The AudioManager will automatically return finished audio files back into its own queue.

1. The AudioManager's looped methods return an Audio Source so you can control them if needed.
