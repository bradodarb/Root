using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Util.Root;


namespace BLL.Root.Validation
{
    [Serializable]
    [DataContract(Name = "validationInfo")]
    public class ValidationInfo
    {
        [DataMember(Name = "validationInfoType")]
        public virtual ValidationInfoType ValidationInfoType { get; set; }
        [DataMember(Name = "name")]
        public virtual string Name { get; set; }
        [DataMember(Name = "errorMessage")]
        public virtual string ErrorMessage { get; set; }
        [DataMember(Name = "errorSource")]
        public virtual string ErrorSource { get; set; }
        [DataMember(Name = "faulted")]
        public virtual bool Faulted { get; set; }


        public ValidationInfo(ValidationInfoType type, string name, string errormessage, bool faulted)
        {

            ValidationInfoType = type;
            Name = name;
            ErrorMessage = errormessage;
            Faulted = faulted;

        }

        public ValidationInfo(ValidationInfoType type, string name, string errormessage, string errorsource, bool faulted)
        {

            ValidationInfoType = type;
            Name = name;
            ErrorMessage = errormessage;
            ErrorSource = errorsource;
            Faulted = faulted;

        }

        public override string ToString()
        {
            return String.Format("Type = {0} :: Name = {1} :: Error Message = {2} :: Faulted = {3}",
                this.ValidationInfoType, this.Name, this.ErrorMessage, this.Faulted);
        }
    }
}
