using DosinisSDK.Core;
using NUnit.Framework;
using UnityEngine;

public class AppTests
{
    [Test]
    public void ModuleManifestBaseTest()
    {
        var manifest = Resources.Load<ModuleManifestBase>(App.MANIFEST_PATH);
        
        Assert.IsNotNull(manifest);
    }

    [Test]
    public void CoreModulesTest()
    {
        var app = App.Core;
        
        Assert.IsNotNull(app.SceneManager);
        Assert.IsNotNull(app.Clock);
        Assert.IsNotNull(app.Timer);
        Assert.IsNotNull(app.Coroutine);
        Assert.IsNotNull(app.EventsManager);
        Assert.IsNotNull(app.DataManager);
    }
}
