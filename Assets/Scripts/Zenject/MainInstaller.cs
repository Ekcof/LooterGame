using Installers;

public class MainInstaller : BaseInstaller
{
    public override void InstallBindings()
    {
        var windows = FindObjectsOfType<UIWindow>();

        foreach (var window in windows)
        {
            Container.Bind(window.GetType()).FromInstance(window).AsSingle();
        }
    }
}
