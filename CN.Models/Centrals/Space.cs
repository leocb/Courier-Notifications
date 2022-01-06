using System;
using System.Collections.Generic;
using System.Text;

namespace CN.Models.Centrals
{
    public class Space
    {
        public Guid Id { get; set; }
        public List<Guid> AllowedDevices { get; set; } = new List<Guid>();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public List<Field>? Fields { get; set; } = new List<Field> { };


    }
}
