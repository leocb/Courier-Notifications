using System;
using System.Collections.Generic;

namespace CN.Models.Channels;

public class Channel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Field> Fields { get; set; } = [];

}
