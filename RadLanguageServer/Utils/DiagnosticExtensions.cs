using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RadDiagnostics;
using RadTypeChecker.TypeErrors;
using static RadLanguageServer.Utils.DiagnosticUtils;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace RadLanguageServer.Utils;

public static class DiagnosticExtensions {
  public const string TypeErrorSource = "Rad Type Checker";
  public const string SyntaxErrorSource = "Rad Parser";


  public static Diagnostic ToLSPDiagnostic(this IDiagnostic diagnostic) {
    return diagnostic switch {
      ITypeError typeError => new Diagnostic {
        Code     = typeError.ErrorCodeString,
        Severity = ToLSPSeverity(typeError.Severity),
        Message  = typeError.Message,
        Range = new Range(
            typeError.Location.Line - 1,
            typeError.Location.Column,
            typeError.Location.EndLine - 1,
            typeError.Location.EndColumn
          ),
        Source = TypeErrorSource
      },
      {} => new Diagnostic {
        Severity = ToLSPSeverity(diagnostic.Severity),
        Message  = diagnostic.Message,
        Range = new Range(
            diagnostic.Location.Line - 1,
            diagnostic.Location.Column,
            diagnostic.Location.EndLine - 1,
            diagnostic.Location.EndColumn
          ),
        Source = SyntaxErrorSource
      }
    };
  }
}
