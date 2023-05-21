namespace RadUtils;

/// <summary>
///   A collection of utilities for working with files more easily.
/// </summary>
public static class FileUtils {
  /// <summary>
  ///   Copies a stream to another stream asynchronously and reports progress.
  /// </summary>
  /// <param name="source"> The source stream to copy from. </param>
  /// <param name="destination"> The destination stream to copy to. </param>
  /// <param name="progress">
  ///   An optional progress reporter. Progress is reported as a percentage.
  /// </param>
  /// <param name="sourceLength">
  ///   The length of the source stream. If set to <c> 0 </c>, the length will be determined
  ///   automatically.
  /// </param>
  public static async Task CopyStreamAsync(
    Stream source,
    Stream destination,
    IProgress<int>? progress = null,
    int sourceLength = 0
  ) {
    var  buffer = new byte[81920]; // or any other buffer size you prefer
    int  bytesRead;
    long totalBytesRead = 0;
    while ((bytesRead = await source.ReadAsync(buffer)) > 0) {
      await destination.WriteAsync(buffer.AsMemory(0, bytesRead));
      totalBytesRead += bytesRead;
      progress?.Report(
          (int)(100 * totalBytesRead / (sourceLength == 0 ? source.Length : sourceLength))
        ); // report progress as a percentage
    }
  }


  /// <summary>
  ///   Downloads a file from a URL to a local file asynchronously and reports progress.
  /// </summary>
  /// <param name="url"> The URL in string form to download from. </param>
  /// <param name="filePath"> The path to the file to download to. </param>
  /// <param name="progress">
  ///   An optional progress reporter. Progress is reported as a percentage.
  /// </param>
  public static async void DownloadFileAsync(
    string url,
    string filePath,
    IProgress<int>? progress = null
  ) {
    using var client = new HttpClient();
    client.MaxResponseContentBufferSize = 81920;
    using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
    var       length   = int.Parse(response.Content.Headers.GetValues("content-length").First());
    response.EnsureSuccessStatusCode();
    await using var stream     = await response.Content.ReadAsStreamAsync();
    await using var fileStream = File.Create(filePath);
    await CopyStreamAsync(stream, fileStream, progress, length);
  }
}
