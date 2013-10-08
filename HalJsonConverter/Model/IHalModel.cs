using System.Collections.Generic;

namespace HalJsonConverter.Model
{
    public interface IHalModel
    {
        List<Link> Links { get; set; } 
    }
}