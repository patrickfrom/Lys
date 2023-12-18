using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Lys;

public class Texture2D
{
    public int Id;

    public Texture2D(string imagePath)
    {
        var imageBuffer = File.ReadAllBytes(imagePath);
        var image = ImageResult.FromMemory(imageBuffer);

        Id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, Id);
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        var format = image.Comp switch
        {
            ColorComponents.Default => (int)PixelFormat.Red,
            ColorComponents.RedGreenBlue => (int)PixelFormat.Rgb,
            ColorComponents.RedGreenBlueAlpha => (int)PixelFormat.Rgba,
            _ => 0
        };
        
        GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)format, image.Width, image.Height, 0,
            (PixelFormat)format, PixelType.UnsignedByte, image.Data);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Use(int textureUnit = 0)
    {
        GL.ActiveTexture(TextureUnit.Texture0+textureUnit);
        GL.BindTexture(TextureTarget.Texture2D, Id);
    }

    public void Dispose()
    {
        GL.DeleteTexture(Id);
    }
}