namespace Velo.Logging.Renderers
{
    internal interface IRenderersCollection
    {
        ArrayRenderer GetArrayRenderer(string template, object[] args);

        TRenderer GetRenderer<TRenderer>(string template) where TRenderer : Renderer;
    }
}