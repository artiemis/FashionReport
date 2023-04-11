using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace FashionReport.GUI;

public class MainWindow : Window
{
    private FashionReportPlugin Plugin;

    public MainWindow(FashionReportPlugin plugin) : base(
        "Fashion Report")
    {
        // this.SizeConstraints = new WindowSizeConstraints
        // {
        //     MinimumSize = new Vector2(375, 330),
        //     MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        // };

        // ImGui.SetWindowPos(new Vector2(ImGui.GetIO().DisplaySize.X / 2 * 0.5f, ImGui.GetIO().DisplaySize.Y / 2 * 0.5f), ImGuiCond.Always);
        Plugin = plugin;
    }

    // TODO: resizing, panning and zooming
    public override void Draw()
    {
        if (Plugin.Error != null)
        {
            ImGui.TextColored(new Vector4(255, 0, 0, 255), Plugin.Error);
        }
        else
        {
            if (Plugin.Image != null)
            {
                ImGui.Image(Plugin.Image.ImGuiHandle, new Vector2(Plugin.Image.Width, Plugin.Image.Height));
                ImGui.Spacing();
                if (ImGui.Button("Refresh"))
                    Plugin.UpdateImage();
            }
            else
            {
                ImGui.Text("Loading...");
            }
        }
    }
}
