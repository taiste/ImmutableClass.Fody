# ImmutableClass.Fody

##A Fody add-in that generates a fully usable immutable class from a stub.

###Turns this:

```
using System;
using Newtonsoft.Json;

namespace TimeSloth.Core
{
    [ImmutableClass]
    public class ImmutableClassTest
    {
        [JsonProperty("int")]
        public int IntField { get; set; }
        public string StringField { get; set; }
        public bool BoolField { get; set; }
    }
}
```

###Into this:

```
using Newtonsoft.Json;
using System;

namespace TimeSloth.Core
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

		[JsonProperty("int")]
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
