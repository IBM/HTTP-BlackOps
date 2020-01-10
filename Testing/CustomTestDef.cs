using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testing
{
	/// <summary>
	/// Custom test definition
	/// </summary>
	public class CustomTestDef
	{
		private string _name;
        /// <summary>
        /// The test name
        /// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		private string _type;

		public string Type
		{
			get { return _type; }
			set { _type = value; }
		}
		private string _mutation;
        /// <summary>
        /// Required expression that indicates 
        /// </summary>
		public string Mutation
		{
			get { return _mutation; }
			set { _mutation = value; }
		}
		private string _validation;
        /// <summary>
        /// Required expression which if matches indicates a vulnerability
        /// </summary>
		public string Validation
		{
			get { return _validation; }
			set { _validation = value; }
		}

        private string _exclusion;
        /// <summary>
        /// Regular expression which if set indicates the issue is not vulnerable in spite of the validation matching
        /// </summary>
        public string Exclusion
        {
            get { return _exclusion; }
            set { _exclusion = value; }
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="mutation"></param>
		/// <param name="validation"></param>
        /// <param name="exclusion"></param>
		public CustomTestDef(string name, string type, string mutation, string validation, string exclusion)
		{
			// TODO: Complete member initialization
			this._name = name;
			this._type = type;
			this._mutation = mutation;
			this._validation = validation;
            this._exclusion = exclusion;
		}

		/// <summary>
		/// Returns the contents of the definition separated by \t
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0}\t{1}\t{2}\t{3}\t{4}",_name,_type,_mutation,_validation,_exclusion);
		}

	}
}
