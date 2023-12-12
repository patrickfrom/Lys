using Assimp;
using OpenTK.Mathematics;

namespace Lys;

public class Model
{
    private List<Mesh> _meshes = [];
    
    public Model(string path)
    {
        LoadModel(path);
    }

    public void Draw(Shader shader)
    {
        foreach (var mesh in _meshes)
        {
            mesh.Draw(shader);
        }
    }

    private void LoadModel(string path)
    {
        var importer = new AssimpContext();
        var scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

        if (scene == null || scene.SceneFlags == SceneFlags.Incomplete || scene.RootNode == null)
        {
            Console.WriteLine("ERROR::ASSIMP::");
            return;
        }
        
        ProcessNode(scene.RootNode, scene);
    }

    private void ProcessNode(Node node, Scene scene)
    {
        for (var i = 0; i < node.MeshCount; i++)
        {
            var mesh = scene.Meshes[node.MeshIndices[i]];
            _meshes.Add(ProcessMesh(mesh, scene));
        }

        for (var i = 0; i < node.ChildCount; i++)
        {
            ProcessNode(node.Children[i], scene);
        }
    }

    private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
    {
        var vertices = new List<Vertex>();
        var indices = new List<int>();
        var textures = new List<Texture>();

        for (var i = 0; i < mesh.VertexCount; i++)
        {
            Vertex vertex;

            vertex.Position = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
            vertex.Normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);

            if (mesh.HasTextureCoords(0))
            {
                vertex.TexCoords = new Vector2(mesh.TextureCoordinateChannels[0][i].X,
                    mesh.TextureCoordinateChannels[0][i].Y);
            }
            else
            {
                vertex.TexCoords = new Vector2(0.0f, 0.0f);
            }
            
            vertices.Add(vertex);
        }

        for (var i = 0; i < mesh.FaceCount; i++)
        {
            var face = mesh.Faces[i];
            for (var j = 0; j < face.IndexCount; j++)
            {
                indices.Add(face.Indices[j]);
            }
        }

        if (mesh.MaterialIndex >= 0)
        {
            
        }

        return new Mesh(vertices, indices, textures);
    }

    private List<Texture> LoadMaterialTextures(Material material, TextureType type, string typeName)
    {
        return new List<Texture>();
    }
}