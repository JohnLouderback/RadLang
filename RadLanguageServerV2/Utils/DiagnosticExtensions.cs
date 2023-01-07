using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadDiagnostics;
using RadTypeChecker.TypeErrors;
using static RadLanguageServerV2.Utils.DiagnosticUtils;
using Range = Microsoft.VisualStudio.LanguageServer.Protocol.Range;

namespace RadLanguageServerV2.Utils;

public static class DiagnosticExtensions {
  public const string TypeErrorSource = "Rad Type Checker";
  public const string SyntaxErrorSource = "Rad Parser";


  public static Diagnostic ToLSPDiagnostic(this IDiagnostic diagnostic) {
    return diagnostic switch {
      ITypeError typeError => new Diagnostic {
        Code     = typeError.ErrorCodeString,
        Severity = ToLSPSeverity(typeError.Severity),
        Message  = typeError.Message,
        Range = new Range {
          Start = new Position {
            Line      = typeError.Location.Line - 1,
            Character = typeError.Location.Column
          },
          End = new Position {
            Line      = typeError.Location.EndLine - 1,
            Character = typeError.Location.EndColumn
          }
        },
        Source = TypeErrorSource
      },
      {} => new Diagnostic {
        Severity = ToLSPSeverity(diagnostic.Severity),
        Message  = diagnostic.Message,
        Range = new Range {
          Start = new Position {
            Line      = diagnostic.Location.Line - 1,
            Character = diagnostic.Location.Column
          },
          End = new Position {
            Line      = diagnostic.Location.EndLine - 1,
            Character = diagnostic.Location.EndColumn
          }
        },
        Source = SyntaxErrorSource
      }
    };
  }
}
