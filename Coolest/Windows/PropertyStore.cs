using Coolest.Windows.ComInterface;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	/// <summary>
	/// Property Store class, only supports reading properties at the moment.
	/// </summary>
	class PropertyStore {
		private readonly IPropertyStore storeInterface;

		/// <summary>
		/// Property Count
		/// </summary>
		public int Count {
			get {
				int result;
				Marshal.ThrowExceptionForHR(storeInterface.GetCount(out result));
				return result;
			}
		}

		/// <summary>
		/// Gets property by index
		/// </summary>
		/// <param name="index">Property index</param>
		/// <returns>The property</returns>
		public PropertyStoreProperty this[int index] {
			get {
				PropertyVariant result;
				PropertyKey key = Get(index);
				Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out result));
				return new PropertyStoreProperty(key, result);
			}
		}

		/// <summary>
		/// Contains property guid
		/// </summary>
		/// <param name="key">Looks for a specific key</param>
		/// <returns>True if found</returns>
		public bool Contains(PropertyKey key) {
			for (int i = 0; i < Count; i++) {
				PropertyKey ikey = Get(i);
				if ((ikey.ID == key.ID) && (ikey.PropertyID == key.PropertyID)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Indexer by guid
		/// </summary>
		/// <param name="key">Property Key</param>
		/// <returns>Property or null if not found</returns>
		public PropertyStoreProperty this[PropertyKey key] {
			get {
				PropertyVariant result;
				for (int i = 0; i < Count; i++) {
					PropertyKey ikey = Get(i);
					if ((ikey.ID == key.ID) && (ikey.PropertyID == key.PropertyID)) {
						Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref ikey, out result));
						return new PropertyStoreProperty(ikey, result);
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Gets property key at sepecified index
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>Property key</returns>
		public PropertyKey Get(int index) {
			PropertyKey key;
			Marshal.ThrowExceptionForHR(storeInterface.GetAt(index, out key));
			return key;
		}

		/// <summary>
		/// Gets property value at specified index
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>Property value</returns>
		public PropertyVariant GetValue(int index) {
			PropertyVariant result;
			PropertyKey key = Get(index);
			Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out result));
			return result;
		}

		/// <summary>
		/// Creates a new property store
		/// </summary>
		/// <param name="store">IPropertyStore COM interface</param>
		internal PropertyStore(IPropertyStore store) {
			this.storeInterface = store;
		}
	}
}