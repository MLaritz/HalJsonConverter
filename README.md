HalJsonConverter
================

Hal JSON Converter using JSON.Net

For more information on the HAL specification, head to:
http://stateless.co/hal_specification.html

### How to Use

The Model used for serializing must implement `IHalModel`: 

    public Interface IHalModel
    {
        List<Link> Links { get; set; }
    }

To add objects to the `_embedded` property in JSON, add the `Embedded` attribute on the property in your model.

#### Example

For this person model:

    public class Person : IHalModel {
        public int Id { get;set; }
        public string Name { get; set; }

        public List<Link> Links { get; set; }

        [Embedded]
        public Address Address { get;set; }
    }

The outputted JSON would look like:

    {
        "_links" : {
            "self" : "/api/persons/1"
        },
		"id" : 1,
		"name" : "John Doe",
		"_embedded" : {
			"address" : {
				"city" : "Louisville",
				"state" : "KY"
			}
		}
	}
   

