using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using FashionReport.GUI;
using ImGuiScene;

namespace FashionReport;
public sealed class FashionReportPlugin : IDalamudPlugin
{
    public string Name => "Fashion Report Viewer";
    private const string CommandName = "/fr";

    public TextureWrap? Image = null;
    public string? Error = null;
    private HttpClient Client;

    private DalamudPluginInterface pi { get; init; }
    private CommandManager cm { get; init; }
    public WindowSystem windowSystem = new("FashionReport");
    private MainWindow mainWindow { get; init; }

    public FashionReportPlugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] CommandManager commandManager)
    {
        pi = pluginInterface;
        cm = commandManager;
        Client = new HttpClient();

        this.UpdateImage();

        mainWindow = new MainWindow(this);
        windowSystem.AddWindow(mainWindow);

        cm.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Displays the main Fashion Report details image."
        });

        pi.UiBuilder.Draw += DrawUI;
    }

    public void Dispose()
    {
        pi.UiBuilder.Draw -= DrawUI;
        windowSystem.RemoveAllWindows();
        if (Image != null)
            Image.Dispose();
        cm.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        mainWindow.IsOpen = true;
    }

    private void DrawUI()
    {
        windowSystem.Draw();
    }

    public void UpdateImage()
    {
        UpdateImageInternal().ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                Exception? exc = t.Exception;
                // while (ex is AggregateException && ex.InnerException != null)
                //     ex = ex.InnerException;

                if (exc != null)
                    Error = exc.Message;
            }
        });
    }

    public async Task UpdateImageInternal()
    {
        Image = null;
        Error = null;
        try
        {
            byte[] imageBuffer = await Client.GetByteArrayAsync("https://api.arti3.dev/xiv/kaiyoko?im=0");
            Image = await this.pi.UiBuilder.LoadImageAsync(imageBuffer);
        }
        catch (System.Exception exc)
        {
            Error = exc.Message;
        }
    }
}
