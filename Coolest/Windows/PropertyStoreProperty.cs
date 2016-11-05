using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolest.Windows {
	/// <summary>
	/// Property Store Property
	/// </summary>
	class PropertyStoreProperty {
		private readonly PropertyKey propertyKey;
		private PropertyVariant propertyValue;

		internal PropertyStoreProperty(PropertyKey key, PropertyVariant value) {
			propertyKey = key;
			propertyValue = value;
		}

		/// <summary>
		/// Property Key
		/// </summary>
		public PropertyKey Key {
			get { return propertyKey; }
		}

		/// <summary>
		/// Property Value
		/// </summary>
		public object Value {
			get { return propertyValue.Value; }
		}
	}
}
