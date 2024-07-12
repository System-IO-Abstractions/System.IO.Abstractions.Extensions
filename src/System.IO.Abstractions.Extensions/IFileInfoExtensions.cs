using System.Collections.Generic;
using System.Text;

namespace System.IO.Abstractions
{
    public static class IFileInfoExtensions
    {
        /// <summary>
        /// Throws an exception if the <paramref name="file"/> doesn't exists
        /// </summary>
        /// <param name="file">File that will be checked for existance</param>
        /// <exception cref="FileNotFoundException">Exception thrown if the file is not found</exception>
        public static void ThrowIfNotFound(this IFileInfo file)
        {
            if (!file.Exists)
                throw new FileNotFoundException(StringResources.Format("COULD_NOT_FIND_FILE_EXCEPTION", file.FullName));
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{String}"/> that can enumerate the lines of text in the <paramref name="file"/>
        /// </summary>
        /// <param name="file">File to enumerate content</param>
        /// <returns>Returns an <see cref="IEnumerable{String}"/> to enumerate the content of the file</returns>
        public static IEnumerable<string> EnumerateLines(this IFileInfo file)
        {
            return new LineEnumerable(file, null);
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{String}"/> that can enumerate the lines of text in the specified <paramref name="file"/>
        /// using the specified <paramref name="encoding"/>
        /// </summary>
        /// <param name="file">File to enumerate content</param>
        /// <param name="encoding">Encoding to use to read the file</param>
        /// <returns>Returns an <see cref="IEnumerable{String}"/> to enumerate the content of the file</returns>
        public static IEnumerable<string> EnumerateLines(this IFileInfo file, Encoding encoding)
        {
            return new LineEnumerable(file, encoding);
        }

        /// <summary>
        /// Opens a <see cref="FileSystemStream"/> for the <paramref name="file"/> in the specified <paramref name="mode"/>
        /// </summary>
        /// <param name="file">File to open stream on</param>
        /// <param name="mode">Mode to use when opening the file</param>
        /// <returns>A <see cref="FileSystemStream"/> that can read or write data to the specified <paramref name="file"/></returns>
        public static FileSystemStream OpenFileStream(this IFileInfo file, FileMode mode)
        {
            return file.FileSystem.FileStream.New(file.FullName, mode);
        }

        /// <summary>
        /// Creates a new empty <paramref name="file"/>.
        /// If the file already exists, the file is truncated.
        /// </summary>
        /// <param name="file">File to create</param>
        /// <returns>The original <see cref="IFileInfo"/> so that methods calls can be chained</returns>
        public static IFileInfo Truncate(this IFileInfo file)
        {
            using(var stream = file.OpenFileStream(FileMode.Create))
            {
                stream.Dispose();
            }

            return file;
        }

        /// <summary>
        /// Writes the specified <paramref name="lines"/> to the specified <paramref name="file"/> using the UTF-8 encoding.
        /// If the file already exists and the <paramref name="overwrite"/> flag is set to true, the file will be truncated.
        /// </summary>
        /// <param name="file">File to write to</param>
        /// <param name="lines">Lines to write to file as text</param>
        /// <param name="overwrite">Flag that specifies if the file can be overwritten if it exists</param>
        /// <exception cref="IOException">Exception thrown if the file already exists and the <paramref name="overwrite"/> flag is set to <see cref="false"/></exception>
        public static void WriteLines(this IFileInfo file, IEnumerable<string> lines, bool overwrite = false)
        {
            using (var stream = file.OpenFileStream(GetWriteFileMode(file, overwrite)))
            using (var writer = new StreamWriter(stream))
            foreach(var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        /// <summary>
        /// Writes the specified <paramref name="lines"/> to the specified <paramref name="file"/>
        /// using the specified <paramref name="encoding"/>.
        /// If the file already exists and the <paramref name="overwrite"/> flag is set to true, the file will be truncated.
        /// </summary>
        /// <param name="file">File to write to</param>
        /// <param name="lines">Lines to write to file as text</param>
        /// <param name="encoding">Encoding to use when writing the <paramref name="lines"/> to the text file</param>
        /// <param name="overwrite">Flag that specifies if the file can be overwritten if it exists</param>
        /// <exception cref="IOException">Exception thrown if the file already exists and the <paramref name="overwrite"/> flag is set to <see cref="false"/></exception>
        public static void WriteLines(this IFileInfo file, IEnumerable<string> lines, Encoding encoding, bool overwrite = false)
        {
            using (var stream = file.OpenFileStream(GetWriteFileMode(file, overwrite)))
            using (var writer = new StreamWriter(stream, encoding))
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        /// <summary>
        /// Appends the specified <paramref name="lines"/> to the specified <paramref name="file"/>
        /// </summary>
        /// <param name="file">File to append to</param>
        /// <param name="lines">Lines to append to file as text</param>
        public static void AppendLines(this IFileInfo file, IEnumerable<string> lines)
        {
            using (var writer = file.AppendText())
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        /// <inheritdoc cref="IFile.AppendAllLines(string, IEnumerable{string})"/>
        public static void AppendAllLines(this IFileInfo file, IEnumerable<string> contents)
        {
            file.FileSystem.File.AppendAllLines(file.FullName, contents);
        }

        /// <inheritdoc cref="IFile.AppendAllText(string, string)"/>
        public static void AppendAllText(this IFileInfo file, string contents)
        {
            file.FileSystem.File.AppendAllText(file.FullName, contents);
        }

        /// <inheritdoc cref="IFile.ReadAllBytes(string)"/>
        public static byte[] ReadAllBytes(this IFileInfo file)
        {
            return file.FileSystem.File.ReadAllBytes(file.FullName);
        }

        /// <inheritdoc cref="IFile.ReadAllLines(string)"/>
        public static string[] ReadAllLines(this IFileInfo file)
        {
            return file.FileSystem.File.ReadAllLines(file.FullName);
        }

        /// <inheritdoc cref="IFile.ReadAllText(string)"/>
        public static string ReadAllText(this IFileInfo file)
        {
            return file.FileSystem.File.ReadAllText(file.FullName);
        }

        /// <inheritdoc cref="IFile.WriteAllBytes(string, byte[])"/>
        public static void WriteAllBytes(this IFileInfo file, byte[] bytes)
        {
            file.FileSystem.File.WriteAllBytes(file.FullName, bytes);
        }

        /// <inheritdoc cref="IFile.WriteAllLines(string, IEnumerable{string})"/>
        public static void WriteAllLines(this IFileInfo file, IEnumerable<string> contents)
        {
            file.FileSystem.File.WriteAllLines(file.FullName, contents);
        }

        /// <inheritdoc cref="IFile.WriteAllText(string, string)"/>
        public static void WriteAllText(this IFileInfo file, string contents)
        {
            file.FileSystem.File.WriteAllText(file.FullName, contents);
        }

        /// <inheritdoc cref="IPath.GetFileNameWithoutExtension(string?)"/>
        public static string GetFileNameWithoutExtension(this IFileInfo file)
        {
            return file.FileSystem.Path.GetFileNameWithoutExtension(file.Name);
        }

        private static FileMode GetWriteFileMode(IFileInfo info, bool overwrite)
        {
            if (!overwrite && info.Exists)
            {
                throw new IOException(StringResources.Format("CANNOT_OVERWRITE", info.FullName));
            }

            //if the file already exists it will be truncated
            return FileMode.Create;
        }
    }
}
