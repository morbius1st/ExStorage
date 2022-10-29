using System.IO;
using System.Xml.Serialization;
using UtilityLibrary;


namespace RevitLibrary
{
	/// <summary>
	/// faux utility to get addinManifext / get vendorId
	/// </summary>
	public static class RevitAddinsUtil
	{
		/*
		public static RevitAddIns addinManifest { get; private set; }
		*/


		public static void ReadManifest(string appName)
		{
			// string path = CsUtilities.AssemblyDirectory;
			//
			// using (FileStream fs =
			// 		new FileStream(path + "\\" + appName + ".addin", FileMode.Open))
			// {
			// 	XmlSerializer xs = new XmlSerializer(typeof(RevitAddIns));
			// 	addinManifest = (RevitAddIns) xs.Deserialize(fs);
			// }
		}


		public static string GetVendorId()
		{
			return "pro.cyberstudio";
		}
	}

	/*

	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
	public partial class RevitAddIns
	{
		private RevitAddInsAddIn[] addInField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("AddIn")]
		public RevitAddInsAddIn[] AddIn
		{
			get { return this.addInField; }
			set { this.addInField = value; }
		}
	}



	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class RevitAddInsAddIn
	{
		private string nameField;

		private string textField;

		private string descriptionField;

		private string assemblyField;

		private string fullClassNameField;

		private string clientIdField;

		private string vendorIdField;

		private string vendorDescriptionField;

		private string typeField;

		/// <remarks/>
		public string Name
		{
			get { return this.nameField; }
			set { this.nameField = value; }
		}

		/// <remarks/>
		public string Text
		{
			get { return this.textField; }
			set { this.textField = value; }
		}

		/// <remarks/>
		public string Description
		{
			get { return this.descriptionField; }
			set { this.descriptionField = value; }
		}

		/// <remarks/>
		public string Assembly
		{
			get { return this.assemblyField; }
			set { this.assemblyField = value; }
		}

		/// <remarks/>
		public string FullClassName
		{
			get { return this.fullClassNameField; }
			set { this.fullClassNameField = value; }
		}

		/// <remarks/>
		public string ClientId
		{
			get { return this.clientIdField; }
			set { this.clientIdField = value; }
		}

		/// <remarks/>
		public string VendorId
		{
			get { return this.vendorIdField; }
			set { this.vendorIdField = value; }
		}

		/// <remarks/>
		public string VendorDescription
		{
			get { return this.vendorDescriptionField; }
			set { this.vendorDescriptionField = value; }
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Type
		{
			get { return this.typeField; }
			set { this.typeField = value; }
		}
	}

	*/
}