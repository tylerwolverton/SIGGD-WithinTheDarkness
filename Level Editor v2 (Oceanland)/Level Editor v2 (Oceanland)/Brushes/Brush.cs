using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Level_Editor_v2__Oceanland_.Brushes
{
    // Class for a particular brush
    public class Brush
    {
        // The brush's name
        public readonly string name;
        
        // The brushes tile values (by x, then y)
        public int[,] values;

        // Read a brush from a file
        public Brush(string name, FileStream f)
        {
            this.name = name;

            // Read the brush's size
            StreamReader r = new StreamReader(f);
            int w, h;
            string l = r.ReadLine();
            string[] ls = l.Split(' ');
            w = int.Parse(ls[0]); h = int.Parse(ls[1]);

            values = new int[w,h];

            // Read the brush's data
            for (int j = 0; j < w; j++) {
                l = r.ReadLine();
                ls = l.Split(' ');
                for (int i = 0; i < h; i++) {
                    values[i, j] = int.Parse(ls[i]);
                }
            }
        }


        public static List<Brush> brushes;
        public const string brushPath = "Brushes";

        public static void discoverBrushes()
        {
            brushes = new List<Brush>();

            IEnumerable<string> brushNames;
            string cwd = Directory.GetCurrentDirectory();
            try
            {
                brushNames = Directory.EnumerateFiles(brushPath);
            }
            catch (Exception e) { return; }

            foreach (string s in brushNames)
            {
                string name = Path.GetFileNameWithoutExtension(s);
                brushes.Add(new Brush(name, new FileStream(s, FileMode.Open)));
            }
        }

        // Generate an image for this brush
        public Texture2D genImage()
        {
            return null;
        }
    }
}
