import AdmZip from 'adm-zip';
import * as fs from 'fs';
import { IncomingMessage } from 'http';
import * as https from 'https';
import path from 'path';
import { pipeline, Readable } from 'stream';
import { promisify } from 'util';

const pipelineAsync = promisify(pipeline);

/**
 * Downloads a file from the given URL to the given destination file path.\
 * @param url - The URL to download the file from.
 * @param destFilePath - The destination file path to save the file to.
 * @param [onProgress] - A callback function to be called when the download progress changes.
 * @returns A promise that resolves when the download is complete.
 */
export const downloadFile = async (
  url: string,
  destFilePath: string,
  onProgress?: (downloadedBytes: number, totalBytes: number) => void
): Promise<void> => {
  // Create the destination file stream.
  const fileStream = fs.createWriteStream(destFilePath);

  // Download the file. This is a promise that resolves when the request get an initial
  // response. The response object is an IncomingMessage object that emits events when
  // data is received. We can use the 'data' event to write the received data to the
  // destination file stream. We can also use the 'end' event to resolve the promise
  // when the download is complete.
  const response = await new Promise(
    (resolve: (res: IncomingMessage) => void, reject) => {
      https.get(url, resolve).on('error', reject);
    }
  );

  // If the response is a redirect, follow it and download the file from the new URL.
  if (
    (response.statusCode === 301 || response.statusCode === 302) &&
    response.headers.location
  ) {
    return downloadFile(response.headers.location, destFilePath, onProgress);
  }

  // Check the response status code. If it's not 200, throw an error.
  if (response.statusCode !== 200) {
    throw new Error(`Failed to download file: ${response.statusCode}`);
  }

  // Get the total bytes of the file from the 'content-length' header. This is a string
  // so we need to parse it to an integer. If the header is not present, default to 0.
  const totalBytes = parseInt(response.headers['content-length'] ?? '0', 10);
  let downloadedBytes = 0;

  // Listen to the 'data' event to write the received data to the destination file stream.
  response.on('data', (chunk) => {
    downloadedBytes += chunk.length;
    // Write the chunk to the file stream.
    fileStream.write(chunk);

    // Call the onProgress callback if it's provided.
    if (onProgress) {
      onProgress(downloadedBytes, totalBytes);
    }
  });

  // Wait for the 'end' event to resolve the promise. This event is emitted when the
  // response is complete. We can also listen to the 'error' event to reject the promise
  // if an error occurred.
  await new Promise((resolve, reject) => {
    response.on('end', resolve);
    response.on('error', reject);
  });

  // Finally, close the file stream.
  fileStream.end();
};

type ProgressCallback = (fileName: string, progress: number) => void;

/**
 * Extracts a zip file to the given destination path.
 * @param zipFilePath - The path to the zip file to extract.
 * @param extractToPath - The path to extract the zip file to.
 * @param [onProgress] - A callback function to be called when the extraction progress changes.
 */
export const extractZipFile = async (
  zipFilePath: string,
  extractToPath: string,
  onProgress?: ProgressCallback
): Promise<void> => {
  const zip = new AdmZip(zipFilePath);
  const zipEntries = zip.getEntries();
  const totalEntries = zipEntries.length;

  for (let i = 0; i < totalEntries; i++) {
    const zipEntry = zipEntries[i]!;

    if (zipEntry.isDirectory) {
      continue;
    }

    const entryPath = path.resolve(extractToPath, './' + zipEntry.entryName);
    const entryStream = Readable.from(zipEntry.getData());

    // Create the directory for the entry if it doesn't already exist.
    await fs.promises.mkdir(path.dirname(entryPath), { recursive: true });

    const fileWriteStream = fs.createWriteStream(entryPath);
    let progress = 0;

    entryStream.on('data', (buffer: Buffer) => {
      progress += buffer.length;
      const progressPercent = ((progress / zipEntry.header.size) * 100).toFixed(
        2
      );
      if (onProgress) {
        onProgress(zipEntry.entryName, Number(progressPercent));
      }
    });

    await pipelineAsync(entryStream, fileWriteStream);
  }

  if (onProgress) {
    onProgress('Extraction completed', 100);
  }
};

/**
 * Gives permissions to the given file. This is required for the file to be executable.
 * @param filePath - The path to the file to give permissions to.
 * @returns A promise that resolves when the permissions have been given. *
 */
export const givePermissionsToFile = async (
  filePath: string
): Promise<void> => {
  await new Promise<void>((resolve, reject) => {
    fs.chmod(filePath, 0o755, (err) => {
      if (err) {
        reject(err);
      } else {
        resolve();
      }
    });
  });
};

/**
 * Makes a directory at the given path if it doesn't already exist. If the directory
 * already exists, this function does nothing.
 * @param dirPath - The path to the directory to make.
 */
export const makeDirectory = (dirPath: string): void => {
  // OR
  if (!fs.existsSync(dirPath)) {
    fs.mkdirSync(dirPath, {
      mode: 0o744, // Not supported on Windows. Default: 0o777
      recursive: true
    });
  }
};
