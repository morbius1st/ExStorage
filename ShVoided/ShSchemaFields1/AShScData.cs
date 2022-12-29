#region + Using Directives

#endregion

// user name: jeffs
// created:   10/23/2022 9:05:41 PM

namespace ShSchemaFields
{
	public abstract class AShScData<TKey, TdataD, TdataF> : IShScFieldsBase<TKey,TdataD>  
		where TKey : Enum 
		where TdataD : IScData<TKey>, new() 
		where TdataF : IScField<TKey>, new() 
	{
		public abstract Dictionary<TKey, TdataD> Fields { get; }

		public Tfield
			Initialize<Tfield>(List<TdataF> fieldDefs)
			where Tfield : AShScFields<TKey, TdataF>, new()
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
			return $"this is {nameof(AShScData<TKey, TdataD, TdataF>)} | TKey| {typeof(TKey)} | Tdata| {typeof(TdataD)}";
		}
	}
}
