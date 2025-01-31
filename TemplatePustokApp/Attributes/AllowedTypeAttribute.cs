using System.ComponentModel.DataAnnotations;

namespace TemplatePustokApp.Attributes
{
    public class AllowedTypeAttribute:ValidationAttribute
    {
        private string[] _allowedtypes;
        public AllowedTypeAttribute(params string[] types)
        {
            _allowedtypes = types;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<IFormFile> files = new List<IFormFile>();
            if (value is List<IFormFile> fileList) files = fileList;
            if (value is IFormFile file) files.Add(file);
            foreach (var item in files)
            {
                if (!_allowedtypes.Contains(item.ContentType))
                {
                    string message = "File Content Type Is Invalid... ";
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }
    }
}
