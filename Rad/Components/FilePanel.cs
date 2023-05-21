using RadUtils;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Rad.Components;

public class FilePanel : Renderable {
  private readonly Panel panel;


  public FilePanel(string file) {
    var path = new TextPath(Emoji.Replace(":page_facing_up: ") + file)
      .LeafColor(Color.Blue)
      .StemStyle(new Style(null, null, Decoration.Dim))
      .RootStyle(new Style(null, null, Decoration.Dim))
      .SeparatorStyle(new Style(null, null, Decoration.Dim));

    panel = new Panel(path) {
      Header = new PanelHeader("Compiling File:", Justify.Left),
      Border = BoxBorder.Rounded,
      Expand = false
    };
    panel.BorderColor(Color.Blue);
  }


  protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth) {
    return panel.Call<IEnumerable<Segment>>("Render", options, maxWidth);
  }
}
