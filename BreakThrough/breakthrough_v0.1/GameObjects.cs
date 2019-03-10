using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.CSharp;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System;

namespace breakthrough_v0._1
{

    public class GameObject : Game1
    {
        public GameObject() { }
        public Texture2D texture;
        public Vector2 position = Vector2.Zero;
        public Vector2[] angModifiers = new Vector2[4];//0- up ,1- right, 2- down, 3- left
        public Color gameObjectColor = Color.White;
        public string name;
        public int id = 0;
        private float scale = 0.2f;
        public float rotation = 0;
        public float speed = 0f;
        public float gravity = 0f;
        public int Width = 0, Height = 0, radius = 0;
        public int hit = 0, hp = 3;
        public bool state = true;
        public bool staticState = false;

        public float Scale
        {
            set { scale = value; this.Refresh(); }
            get { return scale; }
        }


        public GameObject(string texture, ContentManager c, Vector2 position, bool staticState, float scale, int hp, int id)
        {

            this.name = texture;
            this.texture = c.Load<Texture2D>(texture);
            this.Width = (int)(this.texture.Width * scale);
            this.Height = (int)(this.texture.Height * scale);
            this.radius = Width / 2;
            this.Scale = scale;
            this.position = position;
            this.angModifiers[0] = new Vector2(1, -1);
            this.angModifiers[1] = new Vector2(-1, 1);
            this.angModifiers[2] = new Vector2(1, -1);
            this.angModifiers[3] = new Vector2(-1, 1);
            this.hp = hp;
            this.id = id;
            this.staticState = staticState;

        }
        public GameObject(string name, float x, float y, float speed, float scale, bool state, bool staticState)
        {
            this.name = name;
            this.scale = scale;
            this.position = new Vector2(x, y);
            this.speed = speed;
            this.state = state;
            this.staticState = staticState;
            this.angModifiers[0] = new Vector2(1, -1);
            this.angModifiers[1] = new Vector2(-1, 1);
            this.angModifiers[2] = new Vector2(1, -1);
            this.angModifiers[3] = new Vector2(-1, 1);
        }

        public void Refresh()
        {
            this.Width = (int)(this.texture.Width * scale);
            this.Height = (int)(this.texture.Height * scale);
            this.radius = Width / 2;
        }



        public Vector2 CenterPoint()
        {
            Vector2 vec;

            vec = new Vector2((Width / 2) + position.X, (Height / 2) + position.Y);
            return vec;
        }

        public Vector2[] Vertices()
        {
            Vector2[] vertices = new Vector2[4];

            vertices[0] = position;
            vertices[1] = new Vector2(position.X + Width, position.Y);
            vertices[2] = new Vector2(position.X, position.Y + Height);
            vertices[3] = new Vector2(position.X + Width, position.Y + Height);

            return vertices;
        }

        public void SetPostion(Vector2 position)
        {
            this.position = position;
            this.position -= new Vector2(texture.Width, texture.Height) / 2;
        }

        public void LoadTexture(string name, ContentManager c)
        {
            texture = c.Load<Texture2D>(name);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, gameObjectColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        //Detects if a point is incide of the circle
        public bool ColidesCircle(Vector2 point)
        {
            bool colides = false;
            colides = (Math.Sqrt(Math.Pow((point.X - (Width / 2) + position.X), 2) + Math.Pow((point.Y - (Height / 2) + position.Y), 2)) <= radius) ? true : false;
            return colides;
        }

        public bool ColidesParalelipiped(Vector2 point)
        {
            bool colides = false;
            colides = ((point.X >= position.X && point.X <= (position.X + Width)) && (point.Y >= position.Y && point.Y <= (position.Y + Height))) ? true : false;
            return colides;
        }


    }


    public class ObjTextures : Game1
    {
        public ObjTextures() { }
        public string name;
        private float scale = 0.1f;
        private Vector2 specialScale;
        public int hp = 0;
        public int Width = 0, Height = 0;
        public bool state = false;
        public bool button = false;
        public bool submenu = false;


        //Buttons
        private bool buttonMenu = false;
        private bool buttonEditor = false;
        private bool buttonPause = false;
        private bool buttonRand = false;
        private bool buttonOptions = false;

        //Submenu
        private bool submenuPart = false;

        public bool title = false;
        public Texture2D texture;
        public Vector2 position = Vector2.Zero;

        //Buttons
        public bool ButtonMenu
        {
            set
            {
                buttonMenu = value;
                if (value == true)
                    this.button = true;
                else
                    this.button = false;
            }

            get { return buttonMenu; }
        }
        public bool ButtonEditor
        {
            set
            {
                buttonEditor = value;
                if (value == true)
                    this.button = true;
                else
                    this.button = false;
            }
            get { return buttonEditor; }
        }
        public bool ButtonPause
        {
            set
            {
                buttonPause = value;
                if (value == true)
                    this.button = true;
                else
                    this.button = false;
            }

            get { return buttonPause; }
        }
        public bool ButtonRand
        {
            set
            {
                buttonRand = value;
                if (value == true)
                    this.button = true;
                else
                    this.button = false;
            }

            get { return buttonRand; }
        }
        public bool ButtonOptions
        {
            set
            {
                buttonOptions = value;
                if (value == true)
                    this.button = true;
                else
                    this.button = false;
            }

            get { return buttonOptions; }
        }

        //Submenus
        public bool SubmenuBG
        {
            set
            {
                this.submenu = value;
            }

            get { return submenu; }
        }
        public bool SubmenuPart
        {
            set
            {
                this.submenuPart = value;
            }

            get { return submenuPart; }
        }

        public Vector2 SpecialScale
        {
            set { specialScale = value; }
            get { return specialScale; }
        }
        public float Scale
        {
            set { scale = value; this.Refresh(); }
            get { return scale; }
        }

        public ObjTextures(string name, ContentManager c)
        {
            this.name = name;
            this.texture = c.Load<Texture2D>(name);
            this.Width = (int)(this.texture.Width * scale);
            this.Height = (int)(this.texture.Height * scale);
        }
        public void Refresh()
        {

            this.Width = (int)(this.texture.Width * scale);
            this.Height = (int)(this.texture.Height * scale);

        }
        public bool ColidesParalelipiped(Vector2 point)
        {
            bool colides = false;
            colides = ((point.X >= position.X && point.X <= (position.X + Width)) && (point.Y >= position.Y && point.Y <= (position.Y + Height))) ? true : false;
            return colides;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
        public void DrawDynamicScale(SpriteBatch spriteBatch,Vector2 dinamicScale)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, dinamicScale, SpriteEffects.None, 0f);
        }
        public void SpecialDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, specialScale, SpriteEffects.None, 0f);
        }
    }



    public class Map : Game1
    {
        public Map() { }
        public List<GameObject> gameObjects = new List<GameObject>();
        public bool saved = false;

        public Map(GameObject obj)
        {
            this.gameObjects.Add(obj);
        }

        public Map(List<SimpleGameObj> simpleObj)
        {
            foreach (SimpleGameObj obj in simpleObj)
            {
                this.gameObjects.Add(new GameObject(obj.name, obj.x, obj.y, obj.speed, obj.scale, obj.state, obj.staticState));
            }
            this.saved = true;
        }

    }

    [Serializable]
    public class SimpleGameObj
    {
        public SimpleGameObj() { }
        public string name;
        public float x = 0, y = 0;
        public float speed = 0;
        public float scale = 0;
        public bool state = true;
        public bool staticState = false;
        public int id = 0;

        public SimpleGameObj(string name, float scale, Vector2 position, float speed, bool state, bool staticState, int id)
        {
            this.name = name;
            this.scale = scale;
            this.x = position.X;
            this.y = position.Y;
            this.speed = speed;
            this.state = state;
            this.staticState = staticState;
            this.id = id;
        }
    }
    
    [Serializable]
    public class MapSave
    {
        public MapSave() { }
        public List<SimpleGameObj> simpleGameObjs = new List<SimpleGameObj>();
        public bool saved = true;
    }
}