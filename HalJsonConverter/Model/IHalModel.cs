using System.Collections.Generic;

namespace Hal.Json.Model
{
    public interface IHalModel
    {
        List<Link> Links { get; set; } 
    }
}