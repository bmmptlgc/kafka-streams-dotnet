namespace Sample.Kafka.Supplier.DI.UnitTests.TopologyDriverTests;

[global::System.CodeDom.Compiler.GeneratedCodeAttribute("avrogen", "1.11.3")]
public partial class User : global::Avro.Specific.ISpecificRecord
{
	public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse(@"{""type"":""record"",""name"":""User"",""namespace"":""Sample.Kafka.Supplier.DI.UnitTests.TopologyDriverTests"",""fields"":[{""name"":""UserId"",""type"":""string""},{""name"":""DisplayName"",""type"":""string""},{""name"":""RoleNames"",""type"":{""type"":""array"",""items"":""string""}},{""name"":""IdpId"",""type"":""string""}]}");
	private string _UserId;
	private string _DisplayName;
	private IList<System.String> _RoleNames;
	private string _IdpId;
	public virtual global::Avro.Schema Schema
	{
		get
		{
			return User._SCHEMA;
		}
	}
	public string UserId
	{
		get
		{
			return this._UserId;
		}
		set
		{
			this._UserId = value;
		}
	}
	public string DisplayName
	{
		get
		{
			return this._DisplayName;
		}
		set
		{
			this._DisplayName = value;
		}
	}
	public IList<System.String> RoleNames
	{
		get
		{
			return this._RoleNames;
		}
		set
		{
			this._RoleNames = value;
		}
	}
	public string IdpId
	{
		get
		{
			return this._IdpId;
		}
		set
		{
			this._IdpId = value;
		}
	}
	public virtual object Get(int fieldPos)
	{
		switch (fieldPos)
		{
			case 0: return this.UserId;
			case 1: return this.DisplayName;
			case 2: return this.RoleNames;
			case 3: return this.IdpId;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Get()");
		};
	}
	public virtual void Put(int fieldPos, object fieldValue)
	{
		switch (fieldPos)
		{
			case 0: this.UserId = (System.String)fieldValue; break;
			case 1: this.DisplayName = (System.String)fieldValue; break;
			case 2: this.RoleNames = (IList<System.String>)fieldValue; break;
			case 3: this.IdpId = (System.String)fieldValue; break;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Put()");
		};
	}
}