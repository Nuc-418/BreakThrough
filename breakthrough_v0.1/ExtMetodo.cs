using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.CSharp;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System;
using System.IO;

namespace breakthrough_v0._1
{
    public static class ExtMetodo
    {
        //Remove the file type // Ex: "Player.png"-->"Player"
        public static string RemoveExt(this string name)
        {
            string result = null;
            foreach (char c in name)
            {
                if (c == '.')
                    break;
                result += c;
            }
            return result;
        }
        public static string RemoveAPartir(this string name, char charCorte)
        {
            string result = null;
            foreach (char c in name)
            {
                if (c == charCorte)
                    break;
                result += c;
            }
            return result;
        }
        public static string RemoveStr(this string name, string aux)
        {
            int index = 0;
            string result = null;
            foreach (char c in name)
            {
                if (c != aux[index])
                {
                    result += c;
                }
                else
                    if (index < aux.Length - 1)
                    index++;
            }
            return result;
        }

        //Returns the index of the sprite whit the "name"(input)
        public static int NameToIndex(this List<ObjTextures> Objects, string name)
        {
            int index = 0, i = 0;
            foreach (ObjTextures obj in Objects)
            {
                if (name == obj.name)
                {
                    index = i;
                    return index;
                }
                i++;
            }
            return -1;
        }
        public static int NameToIndex(this List<GameObject> Objects, string name)
        {
            int index = 0, i = 0;
            foreach (GameObject obj in Objects)
            {
                if (name == obj.name)
                {
                    index = i;
                    return index;
                }
                i++;
            }
            return -1;
        }

        public static int LastListIndex(this List<Map> mapList)
        {
            int index = -1;
            foreach (Map element in mapList)
            {
                index++;
            }
            return index;
        }


        public static int LastListIndex(this List<MapSave> mapList)
        {
            int index = -1;
            foreach (MapSave element in mapList)
            {
                index++;
            }
            return index;
        }

        //Criar e guardar informação em ficheiro XML
        public static void Save<T>(this T obj, string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName,
                         FileMode.Create,
                         FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, obj);
            stream.Close();
        }

        //Ler informação de ficheiros XML
        public static T Load<T>(this T obj, string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);
            obj = (T)formatter.Deserialize(stream);
            stream.Close();
            return obj;

        }

        //Clone
        public static T DeepClone<T>(this T obj)
        {
            T objResult;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Position = 0;
                objResult = (T)bf.Deserialize(ms);
            }
            return objResult;
        }

        //Last element


    }
}


