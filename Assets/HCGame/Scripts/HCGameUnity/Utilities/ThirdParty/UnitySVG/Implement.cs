using UnityEngine;
using UnityEngine.Profiling;

public class Implement {
  // private TextAsset _SVGFile;
  
  private string _SVGFileString;
  private Texture2D _texture;
  private readonly SVGGraphics _graphics;
  private SVGDocument _svgDocument;

  public Implement(TextAsset svgFile, ISVGDevice device) {
    // _SVGFile = svgFile;
    _SVGFileString = svgFile.text;
    _graphics = new SVGGraphics(device);
  }
  
  public Implement(string svgText, ISVGDevice device) {
    // _SVGFile = svgFile;
    _SVGFileString = svgText;
    _graphics = new SVGGraphics(device);
  }

  private void CreateEmptySVGDocument() {
    _svgDocument = new SVGDocument(_SVGFileString, _graphics);
  }

  public void StartProcess() {
    Profiler.BeginSample("SVG.Implement.StartProcess[CreateEmptySVGDocument]");
    CreateEmptySVGDocument();
    Profiler.EndSample();
    Profiler.BeginSample("SVG.Implement.StartProcess[GetRootElement]");
    SVGSVGElement _rootSVGElement = _svgDocument.RootElement;
    Profiler.EndSample();
    Profiler.BeginSample("SVG.Implement.StartProcess[ClearCanvas]");
    _graphics.SetColor(Color.white);
    Profiler.EndSample();
    Profiler.BeginSample("SVG.Implement.StartProcess[RenderSVGElement]");
    _rootSVGElement.Render();
    Profiler.EndSample();
    Profiler.BeginSample("SVG.Implement.StartProcess[RenderGraphics]");
    _texture = _graphics.Render();
    Profiler.EndSample();
  }

  // public void NewSVGFile(TextAsset svgFile) {
  //   _SVGFile = svgFile;
  // }

  public Texture2D GetTexture() {
    if(_texture == null)
      return new Texture2D(0, 0, TextureFormat.ARGB32, false);
    return _texture;
  }
}
