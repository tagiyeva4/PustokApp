using System.ComponentModel.DataAnnotations;

namespace TemplatePustokApp.Attributes
{
    public class MaxSizeAttribute:ValidationAttribute
    {
        private int _size;
        public MaxSizeAttribute(int size)
        {
            _size = size;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<IFormFile> files = new List<IFormFile>();
            if (value is List<IFormFile> fileList) files=fileList;
            if (value is IFormFile file) files.Add(file);
            foreach (var item in files)
            {
                if (item.Length > _size)
                {
                    string message = $"File must be less than {_size}...";
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }
    }
}
