namespace RadCompiler;

/// <summary>
///   The <c> CompilerEventHandler </c> delegate is used to handle the <c> Status </c> event for
///   when the compiler wishes to signal a status update to subscribers.
/// </summary>
public delegate void CompilerEventHandler(object sender, CompilerEventArgs e);
