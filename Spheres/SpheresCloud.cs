using OpenTK.Graphics.OpenGL4;

namespace PolygonalLightShading;

public class SpheresCloud
{
    private const int Count = 1_000_000;
    private Mesh mesh;

    public SpheresCloud()
    {
        var random = new Random();
        var positions = new List<float>();
        var radius = new List<float>();
        var indices = new List<int>();
        for (int i = 0; i < Count; i++)
        {
            positions.AddRange(new[]{ 100 * (random.NextSingle() * 2 - 1), 100 * (random.NextSingle() * 2 - 1), 100 * (random.NextSingle() * 2 - 1) });
            radius.Add(random.NextSingle());
            indices.Add(i);
        }

        mesh = new Mesh(positions.ToArray(), radius.ToArray(), indices.ToArray(), PrimitiveType.Points);
    }

    public void Render()
    {
        mesh.Render();
    }

    public void Dispose()
    {
        mesh.Dispose();
        GC.SuppressFinalize(this);
    }
}