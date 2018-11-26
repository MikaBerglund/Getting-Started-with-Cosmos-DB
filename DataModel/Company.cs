using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class Company : DocumentBase
    {
        public string Name { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public override string Partition
        {
            get => $"location:{this.Country}/{this.City}";
            set => { /** Deliberately empty */ }
        }
    }
}
