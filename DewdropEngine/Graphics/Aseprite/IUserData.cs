using SFML.Graphics;

// Gist from:
// https://gist.github.com/NoelFB/778d190e5d17f1b86ebf39325346fcc5

// File Format:
// https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md

// Note: I didn't test with with Indexed or Grayscale colors
// Only implemented the stuff I needed / wanted, other stuff is ignored

namespace DewDrop.Graphics.Aseprite;

public struct UserData
{
    public string Text = String.Empty;
    public Color Color = Color.Transparent;

    public UserData()
    {
    }
}

public partial class AsepriteImporter
{
    public interface IUserData
    {
        public UserData UserData { get; set; }
    }

}