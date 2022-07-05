# Unity-3D-SketchFab-Viewer

Is a Plugin where you can use to add the viewing functionality for you application as a camera orbit just like SketchFab and you can easly switch bettwen a normal FPS or thered person view with a smooth lerp effect.

![image](https://user-images.githubusercontent.com/12146382/177429268-b02acfe7-b2b8-4cdf-b122-923a7b6df298.png)


# How To Implement

After downloading the package from the realeses section and importing it:

1- you just need to add the prefab "OrbitCenter" to your scene.

2- you can make the either use the defualt settings with no other camera or the scribt will fetch the default MainCamera to switch with or just assign the Camera you want to switch with in the "Player Camera" Variable in the Inspector.

3- At runtime the OrbitCenter would be disabled by defualt but to prevent any interferance bettwen the OrbitCamera and the MainCamera but setting the game object as active whill switch to the Orbit Camera with (falling back/zoom out) animation.

4- You can use the main functionalites as Sketch fab like (Rotating/Zooming/Panning).

5- To allow foucse area on double clicking functionality you need to tag the Model or the gameobject of the area you want to double click on with "Model" Tag.

6- One more feature is that when allowing the "Orbit Camera" a Dropdown menu will show up that will allow you to fast switch bettwen the model axeis and views (Top,Bottom,Left,Right,Front,Back) and you can sit any style of your on to the Dropdown.

7- You are ready to play the viewer you want and to edit it or customize it for your needs Good Luck.


This Plugin is free to use either privetily or commertial.

Thank you.

Peace Out :"D .
