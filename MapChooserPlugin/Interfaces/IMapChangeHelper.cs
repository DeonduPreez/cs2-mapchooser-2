namespace MapChooserPlugin.Interfaces;

public interface IMapChangeHelper
{
    public string? NextMap { get; }

    void OnMatchEnd();
    public void SetNextMap(string? nextMap);
}