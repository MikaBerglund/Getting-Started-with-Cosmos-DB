using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class Company : DocumentBase
    {

        public Company()
        {
            this.GlobalId = Guid.NewGuid().ToString();
        }

        public string GlobalId { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public override string Partition
        {
            get => $"location:{this.Country}/{this.City}";
        }
    }
}
