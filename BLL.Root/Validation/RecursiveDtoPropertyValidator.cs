using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BLL.Root.Dto;

namespace BLL.Root.Validation
{
    public class RecursiveDtoPropertyValidator
    {
        public List<ValidationInfo> ValidationInfo { get; private set; }
        public bool IsValid { get { return ValidationInfo.Count < 1; } }

        public RecursiveDtoPropertyValidator()
        {
        }

        public RecursiveDtoPropertyValidator(DtoBase dto)
        {
            throw new NotImplementedException();
        }

        public void Validate(DtoBase dto, string root = "")
        {
            ValidationInfo = new List<Validation.ValidationInfo>();
             Type type = dto.GetType();
             var properties = type.GetProperties();

             foreach (PropertyInfo property in properties)
             {

                 ValidationManager.ValidateProperty(dto, property, ValidationInfo, root);

             }

        }
    }
}
