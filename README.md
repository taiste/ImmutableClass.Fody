#ImmutableClass.Fody

##A Fody add-in that generates boilerplate code for a usable immutable class from a stub class.

####How to use it:
Use NuGet to add package ImmutableClass.Fody in each project where you want to use it. ImmutableClass depends on Fody, so it should be added too.

There will be a FodyWeavers.XML file in the root folder of each project. Add an`<ImmutableClass />` tag to the file, so that it looks something like the snippet below. If you have several weavers added, they will appear here too:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <ImmutableClass />
</Weavers>
```
That's it for the setup part! Now you can start using it in your classes. For example, if you want an immutable class with a bool, a string and an int, you can simply write it like this:

```c#
using System;
using ImmutableClass;

namespace Example
{
    [ImmutableClass]
    public class ImmutableClassTest
    {
        public int IntField { get; set; }
        public string StringField { get; set; }
        public bool BoolField { get; set; }
    }
}
```

When you build your project, ImmutableClass Fody weaver will then generate boilerplate code for the class in the following ways:
    * All properties will have their Setter method made as `private`. This is done in order to prevent the properties from being set from outside the class.
    * A constructor having all properties as parameters is added. This is the normal way to create a new instance of this class.
    * 'With' instance methods related with each property will be added. This method will create a copy of the current instance, except that the given parameter will replace the value of the property in question.
5. Given the stub above, the generated class will then become as follows:
```c#
using System;

namespace Example
{
	public class ImmutableClassTest
	{
		//
		// Properties
		//
		public bool BoolField
		{
			get;
			private set;
		}

		public int IntField
		{
			get;
			private set;
		}

		public string StringField
		{
			get;
			private set;
		}

		//
		// Constructors
		//
		public ImmutableClassTest(int intField, string stringField, bool boolField)
		{
			this.IntField = intField;
			this.StringField = stringField;
			this.BoolField = boolField;
		}

		//
		// Methods
		//
		public ImmutableClassTest WithBoolField(bool boolField)
		{
			return new ImmutableClassTest(this.IntField, this.StringField, boolField);
		}

		public ImmutableClassTest WithIntField(int intField)
		{
			return new ImmutableClassTest(intField, this.StringField, this.BoolField);
		}

		public ImmutableClassTest WithStringField(string stringField)
		{
			return new ImmutableClassTest(this.IntField, stringField, this.BoolField);
		}
	}
}
```

You can have extra attributes on the properties (such as `[JsonProperty("int")]`), they will be passed on to the generated class as they are. Likewise you can have fields in your class, they are also retained on without modification.
