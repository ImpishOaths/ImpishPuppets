using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class TextureList: Texture2DArray
{
    [Export]
    public Array<Texture2D> Textures;
    public Array<Image> Images;
    public Array<ImageTexture> ImageTextures;

    public void Initialize()
    {
        Images = [];
        ImageTextures = [];
        foreach(var texture in Textures)
        {
            var image = texture.GetImage();
            image.Decompress();
            Images.Add(image);
            ImageTextures.Add(ImageTexture.CreateFromImage(image));
        }
        CreateFromImages(Images);
    }
}
