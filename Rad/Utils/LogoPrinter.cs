using System.Text;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace Rad.Utils;

public static class LogoPrinter {
  private static readonly string logo = @"
                                i(\lj](:                                            
                       i((cRg#&D@@@@@@@%wkv                                         
                    {/|WM%@@@@@@@@@@@@@@@@@E:                                       
               :{-:/vNe@@@@@%d6pkUn3NPkO8@@@I                                       
               :luV6@@8dS1=t!          ;q9@@D!               *[^FUUXAvj:            
             :i ]cqQ1K: ))p8O}        {g@@@@0;  YIpaF     !tZ9m@@@@@@@@mOz-         
               |Q>~:   *oh@@@<];     fm@@@@mL lW@@@gY[   ;ZGF@@@@@@@@@@@@@0v        
                       ]M@@@0[;    Kg@@@@@dj*3%@@@btv7   ;X@@@@@9UNcXsq@@@@@v       
                     ]K%@@@@%?   <p@@@@@%3!T0@@@@@@@@4     Lk@@@@@+    ]R@@@&       
                    KMM@@@@8u;;zb@@@@@@n}Lb@@@@@@@@@@7     !o@@@@W:     :9@@@~      
                   [6@@@@@%p/X9@@@@@Dnt;k@@@@@O^@@@@%*;i] ~b@@@@a~      \%@@D:      
                 icm@@@@@@O&@@@@@@9P+i?q@@@@@6[I@@@@BkJ>c(o@@@@$\      ]B@@@n       
                jk@@@@@@@%@@@@@@w^tvh@@@@@@@@@@D@@@@@@@hNw@@@@$]     icD@@@m}       
               \d@@@@@@@@@@@@aP<:[X@@@@@@@@@@@@@@@@aJ=]-a@@@@$]    (20@@@@DY        
              /0@@@@@@@@@@9q?    {&@@@@@@&CTI@@@@@@f  -E@@@@$\  ivO%@@@@@w+         
             r8@@@@@@@@8UY:      Y%@@@D@ni  F@@@@@@{ -W@@@@8j->#D@@@@@@MI-          
            Km@@@@@@@@@%U)      V%@@@@gJ    w@@@@@M :R@@@@@SEM@@@@@@@9F)            
           KM@@@@@@@D@@@@8J     ^@@0Gpj    j@@@@@@a s@@@@@@@@@@@@@%Wv;              
          Lm@@@@@@@q!T$@@@@ql   :2L t{     c@@@@@@gJ@@@@@@@@@@@DRJ)                 
         /8@@@@@@@6!  iC%@@@%F-            q@@@@@@&0@@@@@@@@bX?-                    
        {0@@@@@@@9{     lS@@@@wK           m@@@@@@w$@@@@h3y!                        
       *6@@@@@@@@?        HD@@@@3;         %D@@@m%@0ss^~                            
       O@@@@@@@@I          j6@@@@hl        w7D@m!(];                                
      A@@@@@@@@O            -s@@@@MI       :!L>)                                    
     i8@@@@@@@bi              7M@@@@e~                                              
     (w@@@@DCx*                /a@@@@Bj                                             
     :q@@@@F|;                  !p@@@@Df                                            
    l N#xfc[                     :U@@@@@2                                           
          *                        x%@@@@2                                          
                                    [m@@@@^;                                        
                                     jd@@@@T                                        
                                      *Fs88k-                                       
                                        |*YI{                                       
                                           i                                        
";


  public static void Print() {
    var colorTop    = (r: 227f, g: 28f, b: 66f);
    var colorBottom = (r: 28f, g: 227f, b: 206f);
    var colorRight  = (r: 161f, g: 48f, b: 249f);

    var split                   = logo.Split('\n');
    var leftPaddingForCentering = Console.BufferWidth / 2 - split[1].Length / 2;

    // Determine the Y increment to for the gradient based on the different in color divided by
    // the numbers of lines.
    var rYIncrement = (colorTop.r - colorBottom.r) / split.Length;
    var gYIncrement = (colorTop.g - colorBottom.g) / split.Length;
    var bYIncrement = (colorTop.b - colorBottom.b) / split.Length;

    var currentYColor = colorTop;
    var logoString    = new StringBuilder();

    for (var y = 0; y < split.Length; y++) {
      var line = split[y];

      // Increment the "Y" color.
      currentYColor.r = (currentYColor.r - rYIncrement + 255) % 255;
      currentYColor.g = (currentYColor.g - gYIncrement + 255) % 255;
      currentYColor.b = (currentYColor.b - bYIncrement + 255) % 255;

      // Determine the X increment to for the gradient based on the different in color divided by
      // the numbers of lines.
      var rXIncrement = (currentYColor.r - colorRight.r) / line.Length;
      var gXIncrement = (currentYColor.g - colorRight.g) / line.Length;
      var bXIncrement = (currentYColor.b - colorRight.b) / line.Length;

      var currentXColor = currentYColor;

      logoString.Append(new string(' ', leftPaddingForCentering));

      foreach (var character in line) {
        // Increment the "X" color.
        currentXColor.r = (currentXColor.r - rXIncrement + 255) % 255;
        currentXColor.g = (currentXColor.g - gXIncrement + 255) % 255;
        currentXColor.b = (currentXColor.b - bXIncrement + 255) % 255;

        logoString.Append(
            character == ' '
              ? character
              : $"[rgb{Regex.Replace(currentXColor.ToString(), @"\s|(\.\d+)", "")}]{Markup.Escape($"{character}")}[/]"
          );
      }

      logoString.Append('\n');
    }

    AnsiConsole.MarkupLine(logoString.ToString());
  }
}
