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

                // We use the company ID as partition key. To distribute company
                // related data in their own partition.
                this.Partition = value;
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
                this.CompanyId = this.Company?.Id;
            }
        }

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string Name { get; set; }

    }
}
