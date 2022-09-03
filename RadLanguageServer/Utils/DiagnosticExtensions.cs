using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RadDiagnostics;
using RadTypeChecker.TypeErrors;
using RadUtils;
using static RadLanguageServer.Utils.DiagnosticUtils;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace RadLanguageServer.Utils;

public static class DiagnosticExtensions {
  public const string TypeErrorSource = "Rad Type Checker";


  public static Diagnostic ToLSPDiagnostic(this IDiagnostic diagnostic) {
    return diagnostic switch {
      ITypeError typeError => new Diagnostic {
        Code     = typeError.ErrorCodeString,
        Severity = ToLSPSeverity(typeError.Severity),
        Message  = typeError.Message,
        Range = typeError.HasProperty("ForNode", out var dynTypeError)
                  ? new Range(
                      dynTypeError.ForNode.Line - 1,
                      dynTypeError.ForNode.Column,
                      dynTypeError.ForNode.EndLine - 1,
                      dynTypeError.ForNode.EndColumn
                    )
                  : default,
        Source = TypeErrorSource
      }
    };
  }
}
