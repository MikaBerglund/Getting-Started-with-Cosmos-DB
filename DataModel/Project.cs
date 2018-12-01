using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class Project : DocumentBase
    {

        private string _CompanyId;
        /// <summary>
        /// The ID of the company the project is associated with.
        /// </summary>
        public string CompanyId
        {
            get => _CompanyId;
            set
            {
                _CompanyId = value;
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
            get { return _Company; }
            set
            {
                _Company = value;
                this.CompanyId = null != this.Company ? $"{this.Company.Partition}|{this.Company.Id}" : null;
            }
        }

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string Name { get; set; }

    }
}
