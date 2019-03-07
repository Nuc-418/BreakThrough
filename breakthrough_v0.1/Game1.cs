using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.CSharp;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Diagnostics;

namespace breakthrough_v0._1
{
    public class Game1 : Game
    {

        #region Vars

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DirectoryInfo dir;
        SpriteFont[] randPercentage = new SpriteFont[4];




        #region           Lists   types(Map, ObjTextues)
        List<Map> maps = new List<Map>();
        List<ObjTextures> objects = new List<ObjTextures>();


        #endregion

        #region gameplay
        SpriteFont Score;

        #region    Colision && direction vars 
        int colisionArea = 200;
        float centerVecAngCos, centerVecAngSin, verticeDistance;
        Vector2[] vertices = new Vector2[4];
        Vector2 playerDirection = Vector2.Zero;
        Vector2 ballDirection = new Vector2(0.5f, 1);
        Vector2 ballCenter, objCenter, centerVec, auxVec;
        Vector2[] colisionPoint = new Vector2[5];

        #endregion

        #region player & ball vars
        int indexBall, indexPlayer, indexVertice, mouseLastPos, indexColision, indexPaddle;
        int playerBaseSpeed = 60, ballBaseSpeed = 800, maxBallSpeed = 1800;
        int[] bumpSpeed = new int[5];
        int score = 0;
        #endregion

        int lastMapIndex = -1;
        int lastHit = 0;
        int ballOut = 0, nextTexturePlayer = 0;
        int paddleHits = 0;
        bool startBall = false;
        bool addingMap = false;
        bool resetMaps = false;
        bool colides = false;
        int remainingObj = 0;
        int mapN = 0;
        #endregion

        #region map editor
        int randomSpaceY = 400;
        int objIndexAux = 0;
        int lastScroll = 0;
        bool addObj;
        float maxScale = 0.6f, minScale = 0.2f;

        #region rand menu
        int[] brickProb = new int[4];
        float xInicialPointer = 0;
        float positionXRandMin = 509;
        float positionXRandMax = 818;
        float[] positionXRandMaxDynamic = new float[4];
        float[] scaleWhiteBar = new float[4];
        #endregion

        #endregion

        #region Other vars
        int idObj;
        int indexCursor;
        int windowWidth, windowHeight;
        int menu = 0, pausa = 0, randMenu = 0;
        int pauseBackgroundIndex = 0;
        bool colorSwitch = false;

        float red = 44, green = 62, blue = 80;
        float colorFadeSpeed = 15;
        string menuAux, pausaAux;

        Random rand = new Random();
        MouseState mouse;
        GameObject lastObj;
        ObjTextures lastObjTextue = null;
        Vector2 mousePos, mouseClickPos;
        Stopwatch time = new Stopwatch();
        KeyboardState keyboard;

        #endregion

        #endregion

        public Game1()
        {

            graphics = new GraphicsDeviceManager(this);

            //resolution//1920//1080
            graphics.PreferredBackBufferWidth = 1366;//GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; //1366 // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 768;//GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;  //768 // set this value to the desired height of your window

            Window.IsBorderless = true;
            //graphics.ToggleFullScreen();

            //Get window size
            windowWidth = graphics.PreferredBackBufferWidth;
            windowHeight = graphics.PreferredBackBufferHeight;

            Content.RootDirectory = "Content";

        }


        protected override void Initialize()
        {
            dir = new DirectoryInfo(Content.RootDirectory);

            graphics.SynchronizeWithVerticalRetrace = false;

            IsFixedTimeStep = false;

            bumpSpeed[0] = 800;
            bumpSpeed[1] = ballBaseSpeed;
            bumpSpeed[2] = 900;
            bumpSpeed[3] = 1100;
            bumpSpeed[4] = 1500;

            for (int i = 0; i < 4; i++)
                positionXRandMaxDynamic[i] = positionXRandMax;

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //ADD Testures to List --> objects<ObjTextures>
            foreach (object obj in dir.GetFiles())
                if (obj.ToString().RemoveExt() != "Rand_TPercentage0" && obj.ToString().RemoveExt() != "Rand_TPercentage1" && obj.ToString().RemoveExt() != "Rand_TPercentage2" && obj.ToString().RemoveExt() != "Rand_TPercentage3" && obj.ToString().RemoveExt() != "Score")
                    objects.Add(new ObjTextures(obj.ToString().RemoveExt(), Content));


            //Get textBoxes
            for (int i = 0; i < 4; i++)
                randPercentage[i] = Content.Load<SpriteFont>("Rand_TPercentage" + Convert.ToString(i));
            Score = Content.Load<SpriteFont>("Score");



            #region Object Info

            #region Other
            //Cursor
            indexCursor = objects.NameToIndex("cursor");
            objects[indexCursor].Scale = 0.2f;
            objects[indexCursor].state = true;

            #endregion

            #region Menu

            //Butons_menu
            for (int i = 0; i < 3; i++)
                foreach (ObjTextures obj in objects)
                    if (obj.name == ("Button_menu" + Convert.ToString(i)))
                    {
                        obj.ButtonMenu = true;
                        obj.position = new Vector2((windowWidth / 2 - (obj.Width / 2)), (100 * i) + 350);
                    }

            //Title
            objects[objects.NameToIndex("Title_menu")].Scale = 1.2f;
            objects[objects.NameToIndex("Title_menu")].Refresh();
            objects[objects.NameToIndex("Title_menu")].position = new Vector2((windowWidth / 2 - objects[objects.NameToIndex("Title_menu")].Width / 2), 100);
            objects[objects.NameToIndex("Title_menu")].title = true;
            #endregion

            #region Gameplay
            //Player 
            for (int i = 0; i < 3; i++)
            {
                indexPlayer = objects.NameToIndex("Player" + Convert.ToString(i));
                objects[indexPlayer].Scale = 0.2f;
                objects[indexPlayer].state = false;
            }
            indexPlayer = objects.NameToIndex("Player0");

            //PlayerPaddle
            indexPaddle = objects.NameToIndex("Paddle");
            objects[indexPaddle].Scale = 0.2f;
            objects[indexPaddle].state = false;
            //Ball 
            for (int i = 0; i < 3; i++)
            {
                indexBall = objects.NameToIndex("Ball" + Convert.ToString(i));
                objects[indexBall].Scale = 0.2f;
                objects[indexBall].state = false;
            }
            indexBall = objects.NameToIndex("Ball0");

            #region Pause



            pauseBackgroundIndex = objects.NameToIndex("BackgroundSubmenu");
            objects[pauseBackgroundIndex].SubmenuBG = true;
            objects[pauseBackgroundIndex].Scale = 0.6f;
            objects[pauseBackgroundIndex].position = new Vector2((windowWidth / 2 - (objects[pauseBackgroundIndex].Width / 2)), (windowHeight / 2 - (objects[pauseBackgroundIndex].Height / 2)));


            for (int i = 0; i < 2; i++)
                foreach (ObjTextures obj in objects)
                    if (obj.name == ("Button_pause" + Convert.ToString(i)))
                    {
                        obj.ButtonPause = true;
                        obj.Scale = 0.5f;
                        obj.position = new Vector2((windowWidth / 2 - (obj.Width / 2)), (100 * i) + objects[pauseBackgroundIndex].position.Y + 100);
                    }

            #endregion

            #endregion

            #region Map Editor

            //Bricks
            objIndexAux = objects.NameToIndex("brick1");
            objects[objIndexAux].Scale = 0.2f;
            objects[objIndexAux].position = new Vector2(windowWidth - (objects[objIndexAux].Width * 1.5f), windowHeight - objects[objIndexAux].Height - 20);
            objects[objIndexAux].hp = 1;
            objects[objIndexAux].state = true;

            objIndexAux = objects.NameToIndex("brick2");
            objects[objIndexAux].Scale = 0.2f;
            objects[objIndexAux].position = new Vector2(windowWidth - (objects[objIndexAux].Width * 3f), windowHeight - objects[objIndexAux].Height - 20);
            objects[objIndexAux].hp = 2;
            objects[objIndexAux].state = true;

            objIndexAux = objects.NameToIndex("brick3");
            objects[objIndexAux].Scale = 0.2f;
            objects[objIndexAux].position = new Vector2(windowWidth - (objects[objIndexAux].Width * 4.5f), windowHeight - objects[objIndexAux].Height - 20);
            objects[objIndexAux].hp = 3;
            objects[objIndexAux].state = true;

            objIndexAux = objects.NameToIndex("brickI");
            objects[objIndexAux].Scale = 0.2f;
            objects[objIndexAux].position = new Vector2(windowWidth - (objects[objIndexAux].Width * 6), windowHeight - objects[objIndexAux].Height - 20);
            objects[objIndexAux].state = true;

            for (int i = 0; i < 3; i++)
                foreach (ObjTextures obj in objects)
                {
                    if (obj.name == ("Button_editor" + Convert.ToString(i)))
                    {
                        obj.ButtonEditor = true;
                        obj.Scale = 0.4f;
                        obj.position = new Vector2(220 * i + 50, windowHeight - obj.Height - 20);

                    }
                }

            #region Rand menu
            objects[pauseBackgroundIndex].Scale = 1f;
            objects[pauseBackgroundIndex].position = new Vector2((windowWidth / 2 - (objects[pauseBackgroundIndex].Width / 2)), (windowHeight / 2 - (objects[pauseBackgroundIndex].Height / 2) - 40));
            //submenu parts
            for (int i = 0; i < 4; i++)
                foreach (ObjTextures obj in objects)
                {
                    if (obj.name == ("Rand_Brick" + Convert.ToString(i)))
                    {
                        obj.SubmenuPart = true;
                        obj.Scale = 1f;
                        obj.position = new Vector2((0.7f * obj.Width) + objects[pauseBackgroundIndex].position.X, objects[pauseBackgroundIndex].position.Y + (120 * i) + 50);

                    }

                    if (obj.name == ("Rand_TPointer" + Convert.ToString(i)))
                    {
                        obj.SubmenuPart = true;
                        obj.Scale = 1f;
                        obj.position = new Vector2((8f * obj.Width) + objects[pauseBackgroundIndex].position.X, objects[pauseBackgroundIndex].position.Y + (120 * i) + 78);
                        positionXRandMin = obj.position.X;
                    }

                    if (obj.name == ("Rand_SliderBox" + Convert.ToString(i)))
                    {
                        obj.SubmenuPart = true;
                        obj.Scale = 1f;
                        obj.position = new Vector2((windowWidth / 2 - (obj.Width / 2)), objects[pauseBackgroundIndex].position.Y + (120 * i) + 60);
                    }

                    if (obj.name == ("Rand_Textbox" + Convert.ToString(i)))
                    {
                        obj.SubmenuPart = true;
                        obj.Scale = 1f;
                        obj.position = new Vector2((6.3f * obj.Width) + objects[pauseBackgroundIndex].position.X, objects[pauseBackgroundIndex].position.Y + (120 * i) + 62);
                    }

                    if (obj.name == ("Rand_AWhiteBar" + Convert.ToString(i)))
                    {
                        obj.SubmenuPart = true;
                        obj.Scale = 1f;
                        obj.position = new Vector2((6.5f * obj.Width) + objects[pauseBackgroundIndex].position.X, objects[pauseBackgroundIndex].position.Y + (120 * i) + 75);

                    }
                }


            //Butons

            for (int i = 0; i < 2; i++)
                foreach (ObjTextures obj in objects)
                    if (obj.name == ("Button_rand" + Convert.ToString(i)))
                    {
                        obj.ButtonRand = true;
                        obj.Scale = 1f;
                        obj.position = new Vector2((1f * obj.Width) + objects[pauseBackgroundIndex].position.X + (objects[pauseBackgroundIndex].Width - obj.Width - (2 * (1f * obj.Width))) * i, objects[pauseBackgroundIndex].position.Y + objects[pauseBackgroundIndex].Height - (1.3f * obj.Height));
                    }

            #endregion

            #endregion
            #endregion

            #region LoadMaps

            List<MapSave> mapSaves = new List<MapSave>();
            mapSaves = mapSaves.Load("Maps.dat");
            if (mapSaves != null)
            {

                foreach (MapSave mapSave in mapSaves)
                    maps.Add(new Map(mapSave.simpleGameObjs));

                foreach (Map map in maps)
                    foreach (GameObject obj in map.gameObjects)
                        foreach (ObjTextures element in objects)
                            if (obj.name == element.name)
                            {
                                obj.texture = element.texture;
                                obj.state = true;
                                obj.hit = 0;
                                obj.hp = element.hp;
                            }
                foreach (Map map in maps)
                    foreach (GameObject obj in map.gameObjects)
                    {
                        Debug.WriteLine(obj.name);
                        obj.Refresh();
                    }
            }
            #endregion

        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            //Menu
            switch (menu)
            {
                case 0://Menu

                    #region mapManagement

                    //Existe mapa?
                    if (maps != null)
                    {
                        lastMapIndex = maps.LastListIndex();
                        mapN = 0;
                    }

                    //Remover mapa nao guardado
                    if (lastMapIndex != -1 && addingMap == true)
                    {
                        if (maps[lastMapIndex].saved == false)
                        {
                            maps.RemoveAt(lastMapIndex);
                            lastMapIndex--;
                        }
                        else
                        {
                            maps[lastMapIndex].gameObjects[maps[lastMapIndex].gameObjects.NameToIndex("Player0")].position = new Vector2(10, windowHeight - maps[mapN].gameObjects[maps[mapN].gameObjects.NameToIndex("Ball0")].Height - 10);
                            maps[lastMapIndex].gameObjects[maps[lastMapIndex].gameObjects.NameToIndex("Paddle")].position = new Vector2(maps[lastMapIndex].gameObjects[maps[lastMapIndex].gameObjects.NameToIndex("Player0")].position.X + 55, maps[lastMapIndex].gameObjects[maps[lastMapIndex].gameObjects.NameToIndex("Player0")].position.Y);
                            maps[lastMapIndex].gameObjects[maps[lastMapIndex].gameObjects.NameToIndex("Ball0")].speed = ballBaseSpeed;
                            maps[lastMapIndex].gameObjects[maps[lastMapIndex].gameObjects.NameToIndex("Player0")].speed = playerBaseSpeed;
                            //save map ///////////////////////

                            List<MapSave> mapSaves = new List<MapSave>();
                            int n = -1;
                            foreach (Map map in maps)
                            {
                                mapSaves.Add(new MapSave());
                                n = mapSaves.LastListIndex();
                                foreach (GameObject obj in map.gameObjects)
                                {
                                    mapSaves[n].simpleGameObjs.Add(new SimpleGameObj(obj.name, obj.Scale, obj.position, obj.speed, obj.state, obj.staticState, obj.id));
                                }
                            }
                            mapSaves.Save("Maps.dat");

                        }

                    }
                    addingMap = false;

                    //Reset map gameObjects
                    if (resetMaps == true)
                        foreach (Map map in maps)
                            foreach (GameObject obj in map.gameObjects)
                                foreach (ObjTextures element in objects)
                                    if (obj.name == element.name)
                                    {
                                        obj.texture = element.texture;
                                        obj.state = true;
                                        obj.hit = 0;
                                        obj.hp = element.hp;
                                    }

                    #endregion

                    #region Menu

                    //get info
                    mouse = Mouse.GetState(Window);
                    mousePos = new Vector2(mouse.X, mouse.Y);

                    //Cursor
                    objects[indexCursor].position = mousePos;

                    //buttons
                    objects = AnimationFunctions.ButtonAnimation(objects, mousePos, windowWidth, "Button_menu");

                    foreach (ObjTextures obj in objects)
                        if (mouse.LeftButton == ButtonState.Pressed && obj.state == false && obj.ButtonMenu == true)
                            if (obj.ColidesParalelipiped(mousePos))
                                menuAux = obj.name;

                    if (menuAux != null && mouse.LeftButton == ButtonState.Released)
                    {
                        switch (menuAux)
                        {
                            case "Button_menu0":
                                if (lastMapIndex != -1)
                                {
                                    menu = 1;
                                    mapN = 0;
                                    score = 0;
                                }
                                break;
                            case "Button_menu1":
                                menu = 2;
                                break;
                            case "Button_menu2":
                                menu = 3;
                                break;
                            default:
                                menu = 0;
                                break;
                        }
                        menuAux = null;
                    }


                    #endregion

                    break;

                case 1://Gameplay
                    #region Gameplay
                    this.IsMouseVisible = false;
                    //Se existir mapas
                    if (maps[mapN] != null)
                    {

                        #region ball & player info

                        //Get Ball & Player info 
                        indexBall = maps[mapN].gameObjects.NameToIndex("Ball0");
                        indexPlayer = maps[mapN].gameObjects.NameToIndex("Player0");
                        indexPaddle = maps[mapN].gameObjects.NameToIndex("Paddle");
                        ballCenter = maps[mapN].gameObjects[indexBall].CenterPoint();

                        colisionPoint[0] = ballCenter + new Vector2(0, -maps[mapN].gameObjects[indexBall].radius);
                        colisionPoint[1] = ballCenter + new Vector2(maps[mapN].gameObjects[indexBall].radius, 0);
                        colisionPoint[2] = ballCenter + new Vector2(0, maps[mapN].gameObjects[indexBall].radius);
                        colisionPoint[3] = ballCenter + new Vector2(-maps[mapN].gameObjects[indexBall].radius, 0);

                        #endregion

                        if (pausa == 0)
                        {

                            #region Colisions

                            // ball vs walls 
                            if (colisionPoint[3].X < 0)
                            {
                                maps[mapN].gameObjects[indexBall].position.X = 0;
                                ballDirection = new Vector2(-ballDirection.X, ballDirection.Y);
                                lastObj = null;
                            }
                            else if (colisionPoint[2].Y > windowHeight + (2 * maps[mapN].gameObjects[indexBall].radius))
                            {
                                maps[mapN].gameObjects[indexBall].speed = 0;
                                startBall = false;
                                paddleHits = 0;
                                lastHit = 0;
                                lastObj = null;
                                ballOut++;
                            }
                            else if (colisionPoint[1].X > windowWidth)
                            {
                                maps[mapN].gameObjects[indexBall].position.X = windowWidth - maps[mapN].gameObjects[indexBall].Width;
                                ballDirection = new Vector2(-ballDirection.X, ballDirection.Y);
                                lastObj = null;
                            }
                            else if (colisionPoint[0].Y < 0)
                            {
                                maps[mapN].gameObjects[indexBall].position.Y = 0;
                                ballDirection = new Vector2(ballDirection.X, -ballDirection.Y);
                                lastObj = null;
                            }



                            // vs Obj

                            foreach (GameObject obj in maps[mapN].gameObjects)
                            {
                                if (obj.state == true && lastObj != obj)
                                {
                                    //Get object center point
                                    objCenter = obj.CenterPoint();

                                    // Detecting sprites in colisionArea ( / ballCenter)
                                    if ((Math.Sqrt(Math.Pow(objCenter.X - ballCenter.X, 2) + (Math.Pow(objCenter.Y - ballCenter.Y, 2))) <= colisionArea) && obj.name != maps[mapN].gameObjects[indexBall].name)
                                    {
                                        //get vec ( ball center -> obj center )
                                        centerVec = obj.CenterPoint() - ballCenter;
                                        centerVec.Normalize();

                                        //get ang
                                        centerVecAngCos = (float)(Math.Acos(centerVec.X));
                                        centerVecAngSin = (float)(Math.Asin(centerVec.Y));

                                        //get colision point index 4
                                        colisionPoint[4] = centerVec * maps[mapN].gameObjects[indexBall].radius + ballCenter;

                                        //Colisions
                                        #region ColisionAng


                                        if (obj.ColidesParalelipiped(colisionPoint[0]))
                                        {
                                            if (!(paddleHits == 4) || obj.staticState == true || obj.name == "brickI")
                                            {
                                                //Ang correction
                                                if (obj.staticState == false)
                                                    ballDirection *= obj.angModifiers[2];
                                                else
                                                    ballDirection = -centerVec;
                                                indexColision = 0;
                                            }
                                            //Other
                                            lastObj = obj;
                                            colides = true;
                                            obj.hit++;
                                            if (obj.staticState == false && obj.name != "brickI") score += (int)maps[mapN].gameObjects[indexBall].speed;
                                        }
                                        else
                                        if (obj.ColidesParalelipiped(colisionPoint[2]))
                                        {
                                            if (!(paddleHits == 4) || obj.staticState == true || obj.name == "brickI")
                                            {
                                                //Ang correction
                                                if (obj.staticState == false)
                                                    ballDirection *= obj.angModifiers[0];
                                                else
                                                    ballDirection = -centerVec;
                                                indexColision = 2;
                                            }
                                            //Other
                                            lastObj = obj;
                                            colides = true;
                                            obj.hit++;
                                            if (obj.staticState == false && obj.name != "brickI") score += (int)maps[mapN].gameObjects[indexBall].speed;
                                        }
                                        else
                                        if (obj.ColidesParalelipiped(colisionPoint[4]))
                                        {
                                            if (!(paddleHits == 4) || obj.staticState == true || obj.name == "brickI")
                                            {
                                                #region point colision 

                                                //get  obj vertices
                                                vertices = obj.Vertices();
                                                verticeDistance = (float)Math.Sqrt(Math.Pow(vertices[0].X - ballCenter.X, 2) + Math.Pow(vertices[0].Y - ballCenter.Y, 2));
                                                indexVertice = 0;

                                                // get closer  vertice
                                                for (int i = 0; i < 4; i++)
                                                    if (verticeDistance > (float)Math.Sqrt(Math.Pow(vertices[i].X - ballCenter.X, 2) + Math.Pow(vertices[i].Y - ballCenter.Y, 2)))
                                                    {
                                                        verticeDistance = (float)Math.Sqrt(Math.Pow(vertices[i].X - ballCenter.X, 2) + Math.Pow(vertices[i].Y - ballCenter.Y, 2));
                                                        indexVertice = i;
                                                    }

                                                //Get auxVec
                                                auxVec = vertices[indexVertice] - ballCenter;
                                                auxVec.Normalize();

                                                //Ball
                                                //0-top point , 1- right , 2- down , 3-left 
                                                //         0
                                                //       .   .
                                                //      .     .
                                                //     3       1
                                                //      .     .
                                                //       .   .
                                                //         2

                                                //Obj
                                                //    0----------1
                                                //    |          |
                                                //    |          |
                                                //    2----------3

                                                //Ang correction
                                                switch (indexVertice)
                                                {
                                                    case 0:
                                                        if (obj.staticState == false)
                                                        {
                                                            if (centerVecAngCos < (float)(Math.Acos(auxVec.X)) && (float)(Math.Asin(auxVec.Y)) > 0)
                                                            {
                                                                ballDirection *= obj.angModifiers[2];
                                                                indexColision = 2;
                                                            }
                                                            else
                                                            {
                                                                ballDirection *= obj.angModifiers[1];
                                                                indexColision = 1;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (centerVecAngCos < (float)(Math.Acos(auxVec.X)) && (float)(Math.Asin(auxVec.Y)) > 0)
                                                            {
                                                                ballDirection = -centerVec;
                                                                indexColision = 2;
                                                            }
                                                            else
                                                            {
                                                                ballDirection *= obj.angModifiers[1];
                                                                indexColision = 1;
                                                            }
                                                        }
                                                        break;
                                                    case 1:
                                                        if (obj.staticState == false)
                                                        {
                                                            if (centerVecAngCos > (float)(Math.Acos(auxVec.X)) && (float)(Math.Asin(auxVec.Y)) > 0)
                                                            {
                                                                ballDirection *= obj.angModifiers[2];
                                                                indexColision = 2;
                                                            }
                                                            else
                                                            {
                                                                ballDirection *= obj.angModifiers[3];
                                                                indexColision = 3;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (centerVecAngCos > (float)(Math.Acos(auxVec.X)) && (float)(Math.Asin(auxVec.Y)) > 0)
                                                            {
                                                                ballDirection = -centerVec;
                                                                indexColision = 2;
                                                            }
                                                            else
                                                            {
                                                                ballDirection *= obj.angModifiers[3];
                                                                indexColision = 3;
                                                            }
                                                        }
                                                        break;
                                                    case 2:
                                                        if (obj.staticState == false)
                                                        {
                                                            if (centerVecAngCos < (float)(Math.Acos(auxVec.X)) && (float)(Math.Asin(auxVec.Y)) < 0)
                                                            {
                                                                ballDirection *= obj.angModifiers[0];
                                                                indexColision = 0;
                                                            }
                                                            else
                                                            {
                                                                ballDirection *= obj.angModifiers[1];
                                                                indexColision = 1;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (centerVecAngCos < (float)(Math.Acos(auxVec.X)) && (float)(Math.Asin(auxVec.Y)) < 0)
                                                            {
                                                                ballDirection = -centerVec;
                                                                indexColision = 0;
                                                            }
                                                            else
                                                            {
                                                                ballDirection *= obj.angModifiers[1];
                                                                indexColision = 1;
                                                            }
                                                        }
                                                        break;
                                                    case 3:
                                                        if (obj.staticState == false)
                                                        {
                                                            if (centerVecAngCos > (float)(Math.Acos(auxVec.X)) && (float)(Math.Asin(auxVec.Y)) < 0)
                                                            {
                                                                ballDirection *= obj.angModifiers[0];
                                                                indexColision = 0;
                                                            }
                                                            else
                                                            {
                                                                ballDirection *= obj.angModifiers[3];
                                                                indexColision = 3;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (centerVecAngCos > (float)(Math.Acos(auxVec.X)) && (float)(Math.Asin(auxVec.Y)) < 0)
                                                            {
                                                                ballDirection = -centerVec;
                                                                indexColision = 0;
                                                            }
                                                            else
                                                            {
                                                                ballDirection *= obj.angModifiers[3];
                                                                indexColision = 3;
                                                            }
                                                        }
                                                        break;

                                                }
                                                #endregion
                                            }
                                            //hit ++
                                            for (int i = 0; i < paddleHits; i++)
                                                obj.hit++;

                                            if (obj.staticState == false && obj.name != "brickI") score += (int)maps[mapN].gameObjects[indexBall].speed;
                                            lastObj = obj;
                                            colides = true;//Colides with obj
                                        }

                                        if (!(paddleHits == 4) || obj.staticState == true || obj.name == "brickI")
                                            if (obj.ColidesParalelipiped(colisionPoint[4]))
                                            {
                                                if (indexColision == 0)
                                                    maps[mapN].gameObjects[indexBall].position = new Vector2((maps[mapN].gameObjects[indexBall].position.X), (obj.position.Y + obj.Height));
                                                else if (indexColision == 1)
                                                    maps[mapN].gameObjects[indexBall].position = new Vector2((obj.position.X - maps[mapN].gameObjects[indexBall].Width), (maps[mapN].gameObjects[indexBall].position.Y));
                                                else if (indexColision == 2)
                                                    maps[mapN].gameObjects[indexBall].position = new Vector2((maps[mapN].gameObjects[indexBall].position.X), (obj.position.Y - maps[mapN].gameObjects[indexBall].Height - 1));
                                                else if (indexColision == 3)
                                                    maps[mapN].gameObjects[indexBall].position = new Vector2((obj.position.X + obj.Width), (maps[mapN].gameObjects[indexBall].position.Y));
                                            }


                                        #region Other
                                        //Next ball
                                        if (ballOut > 2)
                                        {
                                            nextTexturePlayer = 0;
                                            paddleHits = 0;
                                            resetMaps = true;
                                            startBall = false;
                                            score = score / 3;
                                            lastHit = 0;
                                            ballOut = 0;
                                            menu = 0;
                                            mapN = 0;

                                        }

                                        //player textures
                                        if (ballOut < 4)
                                            maps[mapN].gameObjects[indexPlayer].texture = objects[objects.NameToIndex("Player" + Convert.ToString(nextTexturePlayer))].texture;

                                        //Player hp
                                        if (startBall == false && Mouse.GetState(Window).LeftButton == ButtonState.Pressed)
                                            if (nextTexturePlayer != (ballOut + 1))
                                                nextTexturePlayer++;

                                        //reset ball speed (colision player)
                                        if (colides == true && obj.name == "Player0")
                                        {
                                            maps[mapN].gameObjects[indexBall].speed = ballBaseSpeed;
                                            paddleHits = 1;
                                            lastHit = 0;
                                        }

                                        // Hit Paddle
                                        if (colides == true && obj.name == "Paddle" && Mouse.GetState(Window).LeftButton == ButtonState.Pressed)
                                        {
                                            if (maps[mapN].gameObjects[indexBall].speed < maxBallSpeed && time.ElapsedMilliseconds < 1000)
                                            {
                                                if (obj.hit != lastHit)
                                                {
                                                    paddleHits++;
                                                    lastHit = obj.hit;
                                                }

                                                if (paddleHits < 5)
                                                    maps[mapN].gameObjects[indexBall].speed = bumpSpeed[paddleHits];
                                                else
                                                    paddleHits = 4;

                                                if (maps[mapN].gameObjects[indexBall].speed > maxBallSpeed)
                                                    maps[mapN].gameObjects[indexBall].speed = maxBallSpeed;
                                            }
                                        }

                                        if (colides == true)
                                        {
                                            //Brick texture change
                                            if (obj.staticState == false && obj.name != "brickI")
                                            {
                                                for (int i = 0; i < paddleHits; i++)
                                                    obj.hp--;
                                                if (obj.hp <= 0)
                                                {
                                                    obj.hp = 0;
                                                    obj.state = false;
                                                }
                                                else
                                                    obj.texture = objects[objects.NameToIndex("brick" + obj.hp.ToString())].texture;
                                            }

                                            //Ball textures
                                            if (paddleHits > 0 && paddleHits < 6)
                                                maps[mapN].gameObjects[indexBall].texture = objects[objects.NameToIndex("Ball" + Convert.ToString(paddleHits - 1))].texture;

                                            //End Lvl
                                            if (obj.hit >= obj.hp && obj.staticState == false)
                                            {
                                                foreach (GameObject element in maps[mapN].gameObjects)
                                                {
                                                    if (element.staticState == false && element.state == true && element.name != "brickI")
                                                    {
                                                        remainingObj++;
                                                    }
                                                }
                                                if (remainingObj == 0)
                                                {
                                                    maps[mapN].gameObjects[indexBall].position = Vector2.Zero;
                                                    maps[mapN].gameObjects[indexBall].ballDirection = new Vector2(1, 1);
                                                    maps[mapN].gameObjects[indexBall].speed = ballBaseSpeed;
                                                    mapN++;

                                                    startBall = false;
                                                    paddleHits = 0;
                                                    lastHit = 0;
                                                    ballOut = 0;
                                                    nextTexturePlayer = 0;

                                                    if (maps.LastListIndex() < mapN)
                                                    {
                                                        resetMaps = true;
                                                        menu = 0;
                                                        mapN = 0;
                                                    }
                                                    lastObj = null;
                                                }
                                                remainingObj = 0;
                                            }
                                            colides = false;
                                        }



                                        #endregion

                                        #endregion



                                    }
                                }
                            }
                            #endregion

                            #region Moviment


                            //player

                            Mouse.SetPosition(windowWidth / 2, windowHeight / 2);
                            mouseLastPos = Mouse.GetState(Window).X;
                            maps[mapN].gameObjects[indexPlayer].position.Y = windowHeight - 40;
                            maps[mapN].gameObjects[indexPlayer].position += new Vector2((mouseLastPos - (windowWidth / 2)), 0) * playerBaseSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            // player vs walls
                            if (maps[mapN].gameObjects[indexPlayer].position.X < 0)
                                maps[mapN].gameObjects[indexPlayer].position.X = 0;
                            else if (maps[mapN].gameObjects[indexPlayer].position.X + maps[mapN].gameObjects[indexPlayer].Width > windowWidth)
                                maps[mapN].gameObjects[indexPlayer].position.X = windowWidth - maps[mapN].gameObjects[indexPlayer].Width;

                            //paddle
                            maps[mapN].gameObjects[indexPaddle].position.X = maps[mapN].gameObjects[indexPlayer].position.X + 55;



                            //ball
                            if (startBall == true)
                            {
                                maps[mapN].gameObjects[indexBall].position += ballDirection * maps[mapN].gameObjects[indexBall].speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            else
                            {
                                maps[mapN].gameObjects[indexBall].speed = ballBaseSpeed;
                                maps[mapN].gameObjects[indexBall].texture = objects[objects.NameToIndex("Ball0")].texture;
                                ballDirection = new Vector2(0, -1);
                                maps[mapN].gameObjects[indexBall].position = new Vector2(maps[mapN].gameObjects[indexPlayer].position.X - maps[mapN].gameObjects[indexBall].radius + (maps[mapN].gameObjects[indexPlayer].Width / 2), (maps[mapN].gameObjects[indexPlayer].position.Y - (maps[mapN].gameObjects[indexBall].radius * 2) - 5));
                            }

                            Debug.WriteLine("Ball speed : " + maps[mapN].gameObjects[indexBall].speed);
                            Debug.WriteLine("bassStart = " + startBall);

                            //bump
                            if (Mouse.GetState(Window).LeftButton == ButtonState.Pressed && time.ElapsedMilliseconds < 200)
                            {
                                startBall = true;
                                time.Start();
                                maps[mapN].gameObjects[indexPaddle].position.Y = maps[mapN].gameObjects[indexPlayer].position.Y - 15;

                            }
                            else
                                maps[mapN].gameObjects[indexPaddle].position.Y = maps[mapN].gameObjects[indexPlayer].position.Y;


                            //timer paddle
                            if (time.ElapsedMilliseconds > 400)
                            {
                                time.Stop();
                                time.Reset();
                            }
                            #endregion

                            //pause
                            keyboard = Keyboard.GetState();
                            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                                pausa = 1;
                        }
                        else
                        {
                            #region Pause

                            mouse = Mouse.GetState(Window);
                            mousePos = new Vector2(mouse.X, mouse.Y);

                            //Cursor
                            objects[indexCursor].position = mousePos;

                            //buton animation
                            objects = AnimationFunctions.ButtonAnimation(objects, mousePos, windowWidth, "Button_pause");

                            //Get clik
                            foreach (ObjTextures obj in objects)
                                if (mouse.LeftButton == ButtonState.Pressed && obj.state == false && (obj.ButtonPause == true))
                                    if (obj.ColidesParalelipiped(mousePos))
                                        pausaAux = obj.name;


                            if (pausaAux != null && mouse.LeftButton == ButtonState.Released)
                            {
                                switch (pausaAux)
                                {
                                    case "Button_pause0":
                                        pausa = 0;
                                        break;

                                    case "Button_pause1":
                                        nextTexturePlayer = 0;
                                        paddleHits = 0;
                                        resetMaps = true;
                                        startBall = false;
                                        lastHit = 0;
                                        ballOut = 0;
                                        menu = 0;
                                        mapN = 0;
                                        pausa = 0;
                                        break;
                                    default:
                                        break;
                                }
                                pausaAux = null;
                            }
                            #endregion
                        }
                    }

                    else menu = 0;
                    #endregion
                    break;

                case 2://map editor
                    #region Map Editor

                    if (randMenu == 0)
                    {
                        //Add new map
                        if (addingMap == false)
                        {
                            maps.Add(new Map());
                            lastMapIndex = maps.LastListIndex();
                            addingMap = true;
                        }

                        // Input info
                        keyboard = Keyboard.GetState();
                        mouse = Mouse.GetState(Window);
                        mousePos = new Vector2(mouse.X, mouse.Y);

                        //Cursor
                        objects[indexCursor].position = mousePos;

                        #region Edit Tools

                        //Add player and ball to map
                        if (maps[lastMapIndex].gameObjects.Count == 0)
                        {
                            foreach (ObjTextures obj in objects)
                                if (obj.state == false && obj != objects[indexCursor] && obj.button == false && obj.title == false && obj.submenu == false && obj.SubmenuPart == false && obj.name != "Ball1" && obj.name != "Ball2" && obj.name != "Ball3" && obj.name != "Player1" && obj.name != "Player2" && obj.name != "Player3")
                                    maps[lastMapIndex].gameObjects.Add(new GameObject(obj.name, Content, obj.position, true, obj.Scale, obj.hp, idObj));
                            idObj = maps[lastMapIndex].gameObjects[maps[lastMapIndex].gameObjects.Count - 1].id + 1;
                        }



                        //Add objects to map
                        foreach (ObjTextures obj in objects)
                            if (mouse.LeftButton == ButtonState.Pressed && obj.state == true && addObj == true && obj != objects[indexCursor] && obj.button == false && obj.title == false && obj.submenu == false && obj.SubmenuPart == false)
                                if (obj.ColidesParalelipiped(mousePos))
                                {

                                    maps[lastMapIndex].gameObjects.Add(new GameObject(obj.name, Content, obj.position, false, obj.Scale, obj.hp, idObj));
                                    idObj = maps[lastMapIndex].gameObjects[maps[lastMapIndex].gameObjects.Count - 1].id + 1;
                                    addObj = false;
                                }
                        if (mouse.LeftButton == ButtonState.Released)
                            addObj = true;


                        //Remove object from map
                        foreach (GameObject obj in maps[lastMapIndex].gameObjects)
                            if (mouse.RightButton == ButtonState.Pressed)
                                if (obj.ColidesParalelipiped(mousePos) && obj.staticState == false)
                                {
                                    maps[lastMapIndex].gameObjects.Remove(obj);
                                    break;
                                }


                        if (maps[lastMapIndex].gameObjects[0] != null)
                        {
                            #region Move Objects

                            if (mouse.LeftButton == ButtonState.Released)
                            {
                                if (lastObj != null)
                                {
                                    Mouse.SetPosition((int)lastObj.position.X, (int)lastObj.position.Y);
                                    objects[indexCursor].position = new Vector2((int)lastObj.position.X, (int)lastObj.position.Y);
                                    objects[indexCursor].state = true;
                                }
                                lastObj = null;
                            }

                            //Move obj whit mouse
                            if (mouse.LeftButton == ButtonState.Pressed)
                            {

                                mouseClickPos = new Vector2(windowWidth / 2, windowHeight / 2);


                                mouse = Mouse.GetState(Window);

                                foreach (GameObject obj in maps[lastMapIndex].gameObjects)
                                    if (obj.ColidesParalelipiped(mousePos) && lastObj == null && obj.staticState == false)
                                    {
                                        Mouse.SetPosition(windowWidth / 2, windowHeight / 2);
                                        objects[indexCursor].position = new Vector2(windowWidth / 2, windowHeight / 2);
                                        objects[indexCursor].state = false;
                                        lastObj = obj;
                                    }
                                    else if (lastObj == obj)
                                    {
                                        Mouse.SetPosition(windowWidth / 2, windowHeight / 2);
                                        objects[indexCursor].state = false;
                                        objects[indexCursor].position = new Vector2(windowWidth / 2, windowHeight / 2);
                                        obj.position += (mousePos - mouseClickPos);
                                    }
                            }

                            #endregion

                            #region Change object scale

                            foreach (GameObject obj in maps[lastMapIndex].gameObjects)
                            {
                                if (mouse.ScrollWheelValue > lastScroll)
                                    if (obj.ColidesParalelipiped(mousePos))
                                        if (obj.Scale < maxScale)
                                            obj.Scale += 0.075f;


                                if (mouse.ScrollWheelValue < lastScroll)
                                    if (obj.ColidesParalelipiped(mousePos))
                                        if (obj.Scale > minScale)
                                            obj.Scale -= 0.075f;

                                if (obj.Scale > maxScale)
                                    obj.Scale = maxScale;
                                if (obj.Scale < minScale)
                                    obj.Scale = minScale;

                            }
                            lastScroll = mouse.ScrollWheelValue;
                            #endregion
                        }
                        #endregion

                        #region buttons

                        foreach (ObjTextures obj in objects)
                            if (mouse.LeftButton == ButtonState.Pressed && obj.state == false && obj.ButtonEditor == true)
                                if (obj.ColidesParalelipiped(mousePos))
                                    menuAux = obj.name;

                        if (menuAux != null)
                            switch (menuAux)
                            {
                                //exit editor
                                case "Button_editor0":
                                    menu = 0;
                                    idObj = 0;
                                    break;

                                //Open rand menu
                                case "Button_editor1":
                                    randMenu = 1;
                                    break;

                                //save map 
                                case "Button_editor2":
                                    foreach (GameObject obj in maps[lastMapIndex].gameObjects)
                                        if (obj.staticState == false)
                                        {
                                            maps[lastMapIndex].saved = true;
                                            menu = 0;
                                        }
                                    idObj = 0;
                                    break;
                            }
                        #endregion
                    }
                    else
                    {
                        //Rand Menu
                        mouse = Mouse.GetState(Window);
                        mousePos = new Vector2(mouse.X, mouse.Y);

                        //Cursor
                        objects[indexCursor].position = mousePos;


                        #region Sliders

                        //white bar
                        foreach (ObjTextures obj in objects)
                        {
                            for (int i = 0; i < 4; i++)
                                scaleWhiteBar[i] = (float)((0.0205f) * (objects[objects.NameToIndex(("Rand_TPointer" + Convert.ToString(i)))].position.X - 509) + 1.05f);
                        }

                        if (mouse.LeftButton == ButtonState.Released)
                        {
                            if (lastObjTextue != null)
                            {
                                Mouse.SetPosition((int)lastObjTextue.position.X, (int)lastObjTextue.position.Y);
                                objects[indexCursor].position = new Vector2((int)lastObjTextue.position.X, (int)lastObjTextue.position.Y);
                                objects[indexCursor].state = true;

                                for (int i = 0; i < 4; i++)
                                {
                                    if (("Rand_TPointer" + Convert.ToString(i)) != lastObjTextue.name)
                                    {
                                        positionXRandMaxDynamic[i] -= lastObjTextue.position.X - xInicialPointer;
                                        if (positionXRandMaxDynamic[i] < positionXRandMin)
                                            positionXRandMaxDynamic[i] = positionXRandMin;
                                        if (positionXRandMaxDynamic[i] > positionXRandMax)
                                            positionXRandMaxDynamic[i] = positionXRandMax;
                                    }


                                }
                            }
                            lastObjTextue = null;
                        }

                        //Move obj whit mouse
                        if (mouse.LeftButton == ButtonState.Pressed)
                        {

                            mouseClickPos = new Vector2(windowWidth / 2, windowHeight / 2);
                            mouse = Mouse.GetState(Window);

                            foreach (ObjTextures obj in objects)
                                if (obj.ColidesParalelipiped(mousePos) && lastObjTextue == null && (obj.name == "Rand_TPointer0" || obj.name == "Rand_TPointer1" || obj.name == "Rand_TPointer2" || obj.name == "Rand_TPointer3"))
                                {

                                    Mouse.SetPosition(windowWidth / 2, windowHeight / 2);
                                    objects[indexCursor].position = new Vector2(windowWidth / 2, windowHeight / 2);
                                    objects[indexCursor].state = false;
                                    xInicialPointer = obj.position.X;
                                    lastObjTextue = obj;
                                }
                                else if (lastObjTextue == obj)
                                {
                                    Mouse.SetPosition(windowWidth / 2, windowHeight / 2);
                                    objects[indexCursor].state = false;
                                    objects[indexCursor].position = new Vector2(windowWidth / 2, windowHeight / 2);
                                    obj.position.X += (mousePos - mouseClickPos).X;
                                    lastObjTextue.position.X = ((lastObjTextue.position.X < positionXRandMin) ? positionXRandMin : ((lastObjTextue.position.X > positionXRandMax) ? positionXRandMax : (lastObjTextue.position.X > positionXRandMaxDynamic[Convert.ToInt32(lastObjTextue.name.RemoveStr("Rand_TPointer"))]) ? positionXRandMaxDynamic[Convert.ToInt32(lastObjTextue.name.RemoveStr("Rand_TPointer"))] : lastObjTextue.position.X));
                                }
                        }

                        for (int i = 0; i < 4; i++)
                            objects[objects.NameToIndex("Rand_TPointer" + Convert.ToString(i))].position.X = ((objects[objects.NameToIndex("Rand_TPointer" + Convert.ToString(i))].position.X < positionXRandMin) ? positionXRandMin : ((objects[objects.NameToIndex("Rand_TPointer" + Convert.ToString(i))].position.X > positionXRandMax) ? positionXRandMax : (objects[objects.NameToIndex("Rand_TPointer" + Convert.ToString(i))].position.X > positionXRandMaxDynamic[Convert.ToInt32(objects[objects.NameToIndex("Rand_TPointer" + Convert.ToString(i))].name.RemoveStr("Rand_TPointer"))]) ? positionXRandMaxDynamic[Convert.ToInt32(objects[objects.NameToIndex("Rand_TPointer" + Convert.ToString(i))].name.RemoveStr("Rand_TPointer"))] : objects[objects.NameToIndex("Rand_TPointer" + Convert.ToString(i))].position.X));



                        //brick %
                        brickProb[0] = (int)(((objects[objects.NameToIndex("Rand_TPointer0")].position.X - positionXRandMin) / 309) * 100);
                        brickProb[1] = (int)(((objects[objects.NameToIndex("Rand_TPointer1")].position.X - positionXRandMin) / 309) * 100);
                        brickProb[2] = (int)(((objects[objects.NameToIndex("Rand_TPointer2")].position.X - positionXRandMin) / 309) * 100);
                        brickProb[3] = (int)(((objects[objects.NameToIndex("Rand_TPointer3")].position.X - positionXRandMin) / 309) * 100);

                        #endregion


                        #region buttons
                        menuAux = null;
                        foreach (ObjTextures obj in objects)
                            if (mouse.LeftButton == ButtonState.Pressed && obj.state == false && obj.ButtonRand == true)
                                if (obj.ColidesParalelipiped(mousePos))
                                    menuAux = obj.name;


                        if (menuAux != null)
                            switch (menuAux)
                            {
                                //exit editor
                                case "Button_rand0":
                                    randMenu = 0;
                                    break;

                                //save map 
                                case "Button_rand1":

                                    float nBricksX = windowWidth / objects[objects.NameToIndex("brickI")].Width;
                                    float nBricksY = (windowHeight - randomSpaceY) / objects[objects.NameToIndex("brickI")].Height;

                                    if ((nBricksX + objects[objects.NameToIndex("brickI")].Width) > windowWidth)
                                        nBricksX -= objects[objects.NameToIndex("brickI")].Width;

                                    if ((nBricksY + objects[objects.NameToIndex("brickI")].Height) > windowHeight - randomSpaceY)
                                        nBricksY -= objects[objects.NameToIndex("brickI")].Height;


                                    float spaceX = (windowWidth - (objects[objects.NameToIndex("brickI")].Width * nBricksX)) / 2;
                                    //float spaceY = (windowHeight - randomSpaceY - (objects[objects.NameToIndex("brickI")].Height * nBricksY)) / ;


                                    string objName = null;
                                    int randN = 0;

                                    //remove last Obj
                                    if (maps[lastMapIndex].gameObjects.Count != 3)
                                    {
                                        maps[lastMapIndex].gameObjects.Clear();
                                        idObj = 0;
                                        //Add player and ball to map
                                        if (maps[lastMapIndex].gameObjects.Count == 0)
                                        {
                                            foreach (ObjTextures obj in objects)
                                                if (obj.state == false && obj != objects[indexCursor] && obj.button == false && obj.SubmenuPart == false && obj.title == false && obj.SubmenuBG == false && obj.name != "Ball1" && obj.name != "Ball2" && obj.name != "Ball3" && obj.name != "Player1" && obj.name != "Player2" && obj.name != "Player3")
                                                {
                                                    maps[lastMapIndex].gameObjects.Add(new GameObject(obj.name, Content, obj.position, true, obj.Scale, obj.hp, idObj));
                                                    idObj = maps[lastMapIndex].gameObjects[maps[lastMapIndex].gameObjects.Count - 1].id + 1;
                                                }
                                        }

                                    }
                                    Random getRandom = new Random();
                                    List<BrickSlot> brickSlots = new List<BrickSlot>();
                                    for (int y = 0; y < (int)nBricksY; y++)
                                        for (int x = 0; x < (int)nBricksX; x++)
                                        {
                                            randN = getRandom.Next(1, 100);

                                            objName = (randN <= brickProb[0]) ? ("brick1") : (randN <= (brickProb[0] + brickProb[1])) ? ("brick2") : (randN <= (brickProb[0] + brickProb[1] + brickProb[2])) ? ("brick3") : (randN <= (brickProb[0] + brickProb[1] + brickProb[2] + brickProb[3])) ? ("brickI") : null;
                                            if (objName != null)
                                                brickSlots.Add(new BrickSlot(objName, new Vector2((objects[objects.NameToIndex(objName)].Width * x) + spaceX, (objects[objects.NameToIndex(objName)].Height * y))));
                                        }

                                    //Add to maps->gameObjects
                                    foreach (BrickSlot slot in brickSlots)
                                    {
                                        maps[lastMapIndex].gameObjects.Add(new GameObject(objects[objects.NameToIndex(slot.name)].name, Content, slot.position, false, objects[objects.NameToIndex(slot.name)].Scale, objects[objects.NameToIndex(slot.name)].hp, idObj));
                                        idObj = maps[lastMapIndex].gameObjects[maps[lastMapIndex].gameObjects.Count - 1].id + 1;
                                    }
                                    idObj = 0;
                                    randMenu = 0;

                                    break;
                            }
                        #endregion

                    }

                    #endregion
                    break;

                case 3://quit
                    Exit();
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            #region Background Color

            Color color = Color.White;
            switch (menu)
            {
                case 0: // menu
                    #region Color Management

                    if (red >= 44)
                        colorSwitch = true;

                    if (red <= 1)
                        colorSwitch = false;

                    if (colorSwitch == false)
                    {
                        red += (44 / 34) * (float)gameTime.ElapsedGameTime.TotalSeconds * colorFadeSpeed;
                        green += (62 / 49) * (float)gameTime.ElapsedGameTime.TotalSeconds * colorFadeSpeed;
                        blue += (80 / 63) * (float)gameTime.ElapsedGameTime.TotalSeconds * colorFadeSpeed;
                    }
                    else
                    {
                        red -= (44 / 34) * (float)gameTime.ElapsedGameTime.TotalSeconds * colorFadeSpeed;
                        green -= (62 / 49) * (float)gameTime.ElapsedGameTime.TotalSeconds * colorFadeSpeed;
                        blue -= (80 / 63) * (float)gameTime.ElapsedGameTime.TotalSeconds * colorFadeSpeed;
                    }

                    #endregion
                    color = new Color((int)red, (int)green, (int)blue);
                    break;
                case 1://gameplay
                    color = new Color(33, 33, 90);
                    break;
                case 2://map editor
                    color = Color.White;
                    break;
                case 3://quit
                    color = new Color(33, 33, 90);
                    break;
            }
            #endregion

            GraphicsDevice.Clear(color);

            spriteBatch.Begin();
            switch (menu)
            {
                case 0://Menu
                    #region Menu

                    //buttons
                    foreach (ObjTextures obj in objects)
                        if (obj.ButtonMenu == true || obj.title == true)
                            obj.Draw(spriteBatch);
                    //Score
                    spriteBatch.DrawString(Score, "Score : " + Convert.ToString(score), new Vector2(5, windowHeight - 40), new Color(0, 10, 40));

                    //cursor
                    if (objects[indexCursor].state == true)
                        objects[indexCursor].Draw(spriteBatch);
                    #endregion
                    break;
                case 1://Gameplay
                    #region Gameplay Draw
                    //Score
                    spriteBatch.DrawString(Score, "Score : " + Convert.ToString(score), new Vector2(3, windowHeight - 40), new Color(0, 10, 40));

                    foreach (GameObject obj in maps[mapN].gameObjects)
                        if (obj.state == true)
                            obj.Draw(spriteBatch);

                    if (pausa == 1)
                    {
                        objects[pauseBackgroundIndex].Scale = 0.6f;
                        objects[pauseBackgroundIndex].position = new Vector2((windowWidth / 2 - (objects[pauseBackgroundIndex].Width / 2)), (windowHeight / 2 - (objects[pauseBackgroundIndex].Height / 2)));
                        //Pause menu
                        foreach (ObjTextures obj in objects)
                            if (obj.SubmenuBG || obj.ButtonPause)
                                obj.Draw(spriteBatch);

                        //Draw cursor
                        if (objects[indexCursor].state == true)
                            objects[indexCursor].Draw(spriteBatch);
                    }
                    #endregion
                    break;


                case 2://Map editor
                    #region Map Editor Draw
                    //Draw objects
                    foreach (ObjTextures obj in objects)
                        if (obj.state == true && obj.name != "cursor" || obj.ButtonEditor == true)
                            obj.Draw(spriteBatch);

                    //Draw Map
                    if (lastMapIndex > -1)
                        foreach (GameObject obj in maps[lastMapIndex].gameObjects)
                            if (obj.staticState == false)
                                obj.Draw(spriteBatch);

                    //Draw cursor
                    if (objects[indexCursor].state == true)
                        objects[indexCursor].Draw(spriteBatch);

                    //Rand menu
                    if (randMenu == 1)
                    {
                        //Pause menu
                        objects[pauseBackgroundIndex].Scale = 1f;
                        objects[pauseBackgroundIndex].position = new Vector2((windowWidth / 2 - (objects[pauseBackgroundIndex].Width / 2)), (windowHeight / 2 - (objects[pauseBackgroundIndex].Height / 2) - 40));
                        foreach (ObjTextures obj in objects)
                            if (obj.SubmenuBG)
                                obj.Draw(spriteBatch);

                        foreach (ObjTextures obj in objects)
                            if (obj.name == "Rand_AWhiteBar0" || obj.name == "Rand_AWhiteBar1" || obj.name == "Rand_AWhiteBar2" || obj.name == "Rand_AWhiteBar3")
                            {
                                switch (obj.name)
                                {
                                    case "Rand_AWhiteBar0":
                                        obj.DrawDynamicScale(spriteBatch, new Vector2((scaleWhiteBar[0]), obj.Scale));
                                        break;
                                    case "Rand_AWhiteBar1":
                                        obj.DrawDynamicScale(spriteBatch, new Vector2((scaleWhiteBar[1]), obj.Scale));
                                        break;
                                    case "Rand_AWhiteBar2":
                                        obj.DrawDynamicScale(spriteBatch, new Vector2((scaleWhiteBar[2]), obj.Scale));
                                        break;
                                    case "Rand_AWhiteBar3":
                                        obj.DrawDynamicScale(spriteBatch, new Vector2((scaleWhiteBar[3]), obj.Scale));
                                        break;
                                }
                            }

                        //obj rand
                        foreach (ObjTextures obj in objects)
                            if (obj.name.RemoveAPartir('_') == "Rand" && obj.name != "Rand_AWhiteBar0" && obj.name != "Rand_AWhiteBar1" && obj.name != "Rand_AWhiteBar2" && obj.name != "Rand_AWhiteBar3")
                                obj.Draw(spriteBatch);


                        //Button Rand 
                        foreach (ObjTextures obj in objects)
                            if (obj.ButtonRand)
                                obj.Draw(spriteBatch);

                        for (int i = 0; i < 4; i++)
                            spriteBatch.DrawString(randPercentage[i], Convert.ToString(brickProb[i]) + "%", new Vector2(952, objects[pauseBackgroundIndex].position.Y + (120 * i) + 82), Color.Black);

                        //Draw cursor
                        if (objects[indexCursor].state == true)
                            objects[indexCursor].Draw(spriteBatch);


                    }
                    #endregion
                    break;
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}