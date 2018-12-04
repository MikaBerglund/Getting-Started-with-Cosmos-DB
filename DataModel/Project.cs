using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class Project : DocumentBase
    {

        private string _CompanyGlobalId;
        /// <summary>
        /// The <see cref="Company.GlobalId"/> of the company that the project refers to.
        /// </summary>
        public string CompanyGlobalId
        {
            get => _CompanyGlobalId;
            set
            {
                _CompanyGlobalId = value;
                this.Partition = $"company:{value}";
            }
        }

        private Company _Company;
        /// <summary>
        /// The company associated with the project.
        /// </summary>
        [JsonIgnore]
        public Company Company
        {
            get => _Company;
            set
            {
                _Company = value;
                this.CompanyGlobalId = value?.GlobalId;
            }
        }

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string Name { get; set; }

    }
}
