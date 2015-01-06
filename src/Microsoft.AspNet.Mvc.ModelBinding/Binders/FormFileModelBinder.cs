using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

namespace Microsoft.AspNet.Mvc.ModelBinding
{
    public class FormFileModelBinder : IModelBinder
    {
        public async Task<bool> BindModelAsync([NotNull] ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(IFormFile))
            {
                var postedFiles = await GetFormFiles(bindingContext);

                if (!postedFiles.Any())
                {
                    return false;
                }
                bindingContext.Model = postedFiles.First();
                return true;
            }
            else if (typeof(IEnumerable<IFormFile>).GetTypeInfo().IsAssignableFrom(
                    bindingContext.ModelType.GetTypeInfo()))
            {
                var postedFiles = await GetFormFiles(bindingContext);

                if (!postedFiles.Any())
                {
                    return false;
                }
                bindingContext.Model = ConvertValuesToCollectionType(bindingContext.ModelType, postedFiles);
                return true;
            }
            return false;
        }

        private async Task<List<IFormFile>> GetFormFiles(ModelBindingContext bindingContext)
        {
            var request = bindingContext.OperationBindingContext.HttpContext.Request;
            var postedFiles = new List<IFormFile>();
            if (request.HasFormContentType)
            {
                var form = await request.ReadFormAsync();

                foreach (var file in form.Files)
                {
                    if (string.IsNullOrEmpty(file.ContentDisposition))
                    {
                        continue;
                    }

                    var parsedContentDisposition = file.ParseContentDisposition();

                    var modelName = parsedContentDisposition.Key;

                    if (modelName.Equals(bindingContext.ModelName, StringComparison.OrdinalIgnoreCase))
                    {
                        postedFiles.Add(file);
                    }
                }
            }
            return postedFiles;
        }

        private object ConvertValuesToCollectionType(Type modelType, IList<IFormFile> values)
        {
            if (typeof(List<IFormFile>).IsAssignableFrom(modelType))
            {
                return new List<IFormFile>(values);
            }
            else if (typeof(IFormFile[]).IsAssignableFrom(modelType))
            {
                return values.ToArray();
            }
            else if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(modelType))
            {
                return values;
            }
            else
            {
                return null;
            }
        }
    }
}