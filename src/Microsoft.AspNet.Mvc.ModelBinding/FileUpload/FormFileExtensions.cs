using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Extension methods for <see cref="IFormFile"/>.
    /// </summary>
    public static class FormFileExtensions
    {
        /// <summary>
        /// Saves the contents of an uploaded file.
        /// </summary>
        /// <param name="formFile">The <see cref="IFormFile"/>.</param>
        /// <param name="filename">The name of the file to create.</param>
        public static void SaveAs([NotNull] this IFormFile formFile, string filename)
        {
            FileStream f = new FileStream(filename, FileMode.Create);

            try
            {
                var inputStream = formFile.OpenReadStream();
                byte[] streamBytes = new byte[inputStream.Length];
                inputStream.Read(streamBytes, 0, streamBytes.Length);
                f.Write(streamBytes, 0, streamBytes.Length);
                f.Flush();
            }
            finally
            {
                f.Dispose();
            }
        }

        /// <summary>
        /// Parses the content disposition header.
        /// </summary>
        /// TODO: Remove this when strongly typed header are in place.
        /// <param name="file">The <see cref="IFormFile"/>.</param>
        /// <returns>A <see cref="KeyValuePair{TKey, TValue}" /> of Model Name and File Name.</returns>
        public static KeyValuePair<string, string> ParseContentDisposition(this IFormFile file)
        {
            var modelName = string.Empty;
            var filename = string.Empty;

            var toFind = "name=";

            int index = file.ContentDisposition.IndexOf(toFind, StringComparison.CurrentCultureIgnoreCase);
            if (index >= 0)
            {
                var temp = file.ContentDisposition.Substring(index + toFind.Length);
                modelName = temp.Substring(0, temp.IndexOf(";")).Replace("\"", "");
            }

            toFind = "filename=";
            index = file.ContentDisposition.IndexOf(toFind, StringComparison.CurrentCultureIgnoreCase);
            if (index >= 0)
            {
                filename = file.ContentDisposition.Substring(index + toFind.Length).Replace("\"", "");
            }

            return new KeyValuePair<string, string>(modelName, filename);
        }
    }
}