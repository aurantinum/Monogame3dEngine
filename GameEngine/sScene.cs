

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CPI311.GameEngine
{
    internal class sScene
    {
         delegate void CallMethod();
         CallMethod Update;
         CallMethod Draw;
         sScene(CallMethod update, CallMethod draw)
        { Update = update; Draw = draw; }



        //public List<GameObject> GameObjects;
        //public Camera Camera;
        //public Scene()
        //{
        //    GameObjects = new List<GameObject>();
        //}
        //public void Add(GameObject gameObject)
        //{

        //    GameObjects.Add(gameObject);
        //}
        //public void Destroy(GameObject gameObject)
        //{
        //    GameObjects.Remove(gameObject);
        //}
        //public void Remove(GameObject gameObject)
        //{
        //    GameObjects.Remove(gameObject);
        //}
        //public void _Update()
        //{
        //    foreach (GameObject gameObject in GameObjects)
        //    {
        //        gameObject.Update();
        //    }
        //}
        //public void _Draw()
        //{
        //    foreach (GameObject gameObject in GameObjects)
        //    {
        //        gameObject.Draw();
        //    }
        //}
        //public void _Draw(SpriteBatch batch)
        //{
        //    foreach (GameObject gameObject in GameObjects)
        //    {
        //        gameObject.Draw(batch);
        //    }
        //}


        //private static Scene activeScene = null;
        //public static Camera ActiveSceneCamera { get { return activeScene.Camera; } 
        //    set { 
        //        activeScene.Camera = value; 
        //        foreach (GameObject go in activeScene.GameObjects)
        //        {
        //           if(go.Get<Renderer>() != null)
        //            {
        //                go.Get<Renderer>().Camera = value;
        //            }
        //        }
        //    } }
        //private static List<Scene> scenes = new List<Scene>();
        //private static int activeSceneIndex = 0;
        //private static int sceneCount = 0;
        //public static void AddToActiveScene(GameObject toAdd)
        //{
        //    activeScene.Add(toAdd);
        //}
        //public static void AddScene(Scene scene)
        //{
        //    scenes.Add(scene);
        //    sceneCount++;
        //    if (activeScene == null)
        //    {
        //        activeScene = scene;
        //    }
        //}
        //public static void NextScene()
        //{
        //    activeSceneIndex++;
        //    if(activeSceneIndex >= sceneCount)
        //    {
        //        activeSceneIndex = 0;
        //    }
        //    activeScene = scenes[activeSceneIndex];
        //}
        //public static void PreviousScene()
        //{
        //    activeSceneIndex--;
        //    if(activeSceneIndex < 0)
        //    {
        //        activeSceneIndex = sceneCount - 1;
        //    }
        //    activeScene = scenes[activeSceneIndex];
        //}
        //public static void SetScene(int index)
        //{
        //    activeScene = scenes[index];
        //}
        //public static void Update(GameTime gameTime)
        //{
        //    Time.Update(gameTime);
        //    InputManager.Update();
        //    activeScene._Update();
        //}
        //public static void Draw()
        //{
        //    activeScene._Draw();
        //}
        //public static void Draw(SpriteBatch batch)
        //{
        //    activeScene._Draw(batch);
        //}
    }
}