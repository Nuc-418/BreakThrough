using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.IO;
namespace breakthrough_v0._1
{
    public class BrickSlot
    {

        public string name;
        public Vector2 position;
        public BrickSlot() { }

        public BrickSlot(string name, Vector2 pos)
        {
            this.name = name;
            this.position = pos;
        }

    }
    public static class AnimationFunctions 
    {
        public static List<ObjTextures> ButtonAnimation(List<ObjTextures> objects, Vector2 mousePos, int windowWidth, string buttonName)
        {


            float posAux;
            foreach (ObjTextures obj in objects)
                if (obj.name == buttonName + "0" || obj.name == buttonName + "1" || obj.name == buttonName + "2" || obj.name == buttonName + "3")
                {
                    posAux = obj.position.Y;
                    if (obj.ColidesParalelipiped(mousePos))
                    {
                        if (obj.Scale != 0.7f)
                        {
                            obj.Scale = 0.7f;
                            obj.position.Y = posAux - 7.5f;
                        }
                    }
                    else
                    {
                        if (obj.Scale != 0.6f)
                        {
                            obj.Scale = 0.6f;
                            obj.position.Y = posAux + 7.5f;
                        }
                    }
                    obj.position.X = (windowWidth / 2 - (obj.Width / 2));
                }

            return objects;
        }

    }
    

}
