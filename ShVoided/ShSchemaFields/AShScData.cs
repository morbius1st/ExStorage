#region + Using Directives

#endregion

// user name: jeffs
// created:   10/23/2022 9:05:41 PM

namespace ShSchemaFields
{
	public abstract class AShScData<Tkey, TdataD, TdataF> : IShScFieldsBase<Tkey,TdataD>  
		where Tkey : Enum 
		where TdataD : IScData<Tkey>, new() 
		where TdataF : IScField<Tkey>, new() 
	{
		public abstract Dictionary<Tkey, TdataD> Fields { get; }

		public Tfield
			Initialize<Tfield>(List<TdataF> fieldDefs)
			where Tfield : AShScFields<Tkey, TdataF>, new()
		{
			Tfield fields = new Tfield();

			foreach (TdataF fd in fieldDefs)
			{
				TdataD dd = new TdataD();
				dd = (TdataD) dd.Create(fd);
				fields.Fields.Add(fd.Key, fd);
				Fields.Add(dd.Key, dd);
			}

			return fields;
		}

		public override string ToString()
		{
			return $"this is {nameof(AShScData<Tkey, TdataD, TdataF>)} | Tkey| {typeof(Tkey)} | Tdata| {typeof(TdataD)}";
		}
	}
}
