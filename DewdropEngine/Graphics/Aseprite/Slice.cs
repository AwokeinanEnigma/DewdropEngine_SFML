﻿using SFML.Graphics;
using System.Drawing;
// ReSharper disable MemberHidesStaticFromOuterClass

// Gist from:
// https://gist.github.com/NoelFB/778d190e5d17f1b86ebf39325346fcc5

// File Format:
// https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md

// Note: I didn't test with with Indexed or Grayscale colors
// Only implemented the stuff I needed / wanted, other stuff is ignored
namespace DewDrop.Graphics.Aseprite;


    public partial class AsepriteImporter
    {
        public class Slice : IUserData
        {
            public int Frame;
            public string Name = string.Empty;
            public int OriginX;
            public int OriginY;
            public int Width;
            public int Height;
            public Point? Pivot;
            public IntRect? NineSlice;

            public UserData UserData { get; set; }
        }

    }
