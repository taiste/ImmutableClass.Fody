#ImmutableClass.Fody

##A Fody add-in that generates boilerplate code for an immutable class from a minimal class definition.

####How to use it:
Add ImmutableClass.Fody in each project where you want to use it:
* If you're using Visual Studio, right click on the project name in the Solution Explorer and select "Manage NuGet packages..." from the pop-up menu. Or use the NuGet Package Manager Console if you prefer.
* If you're using Xamarin Studio, right click on the Packages folder under the project. In the pop-up menu, select "Add packages...".

ImmutableClass depends on Fody, so it should be added automatically too, if you don't have it already.

There will be a FodyWeavers.XML file in the root folder of each project. Add an`<ImmutableClass />` tag to the file, so that it looks something like the XML file below. If you have several weavers added, they will appear here too:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <ImmutableClass />
</Weavers>
```
That's it for the setup part! Now you can start using ImmutableClass.Fody in your project. For example, if you want an immutable class with an integer, a string and a boolean, you can simply write the class like this:

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

There's two things here to note: (1) mark all classes that you want to generate the boilerplate for with the `[ImmutableClass]` attribute; and (2) add the `using ImmutableClass;` line at the top of the file whenever you use this attribute. The class itself is very simple, with just the name and a list of properties.

When you build your project, ImmutableClass Fody weaver will generate boilerplate code for the class in the following ways:
    * All properties will have their Setter method made as `private`. This is done in order to prevent the properties from being set from outside the class.
    * A constructor having all its properties as parameters is added. This kind of constructor is typically used to create a new instance of an immutable class: since the class can't be changed anymore after it is created, all class properties need to be given in the constructor.
    * 'With' instance methods related with each property will be added. If we used mutable classes, we could just give a new value to a property -- but for immutable classes we need to create a copy of the class, with everything the same except replace the property with something else. These methods are created exactly for this purpose. The method has the name of the property to change, and one single argument to be used as the value of that property. Values for all other properties will be copied over from the instance where this method is called.

Given the stub above, the generated class will then become as follows:

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

You can have extra attributes on the properties (such as `[JsonProperty("int")] if you need to use the class in JSON serialization`), they will be passed on to the generated class as they are. Likewise you can have fields in your class, they are also retained on without modification.
