using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Coolest.Windows {
	/// <summary>
	/// from Propidl.h.
	/// http://msdn.microsoft.com/en-us/library/aa380072(VS.85).aspx
	/// contains a union so we have to do an explicit layout
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct PropertyVariant {
		[FieldOffset(0)]
		private short vt;
		[FieldOffset(2)]
		private short wReserved1;
		[FieldOffset(4)]
		private short wReserved2;
		[FieldOffset(6)]
		private short wReserved3;
		[FieldOffset(8)]
		private sbyte cVal;
		[FieldOffset(8)]
		private byte bVal;
		[FieldOffset(8)]
		private short iVal;
		[FieldOffset(8)]
		private ushort uiVal;
		[FieldOffset(8)]
		private int lVal;
		[FieldOffset(8)]
		private uint ulVal;
		[FieldOffset(8)]
		private int intVal;
		[FieldOffset(8)]
		private uint uintVal;
		[FieldOffset(8)]
		private long hVal;
		[FieldOffset(8)]
		private long uhVal;
		[FieldOffset(8)]
		private float fltVal;
		[FieldOffset(8)]
		private double dblVal;
		[FieldOffset(8)]
		private short boolVal;
		[FieldOffset(8)]
		private int scode;
		[FieldOffset(8)]
		private DateTime date;
		[FieldOffset(8)]
		private System.Runtime.InteropServices.ComTypes.FILETIME filetime;
		[FieldOffset(8)]
		private Blob blobVal;
		[FieldOffset(8)]
		private IntPtr pointerValue;

		/// <summary>
		/// Creates a new PropVariant containing a long value
		/// </summary>
		public static PropertyVariant FromLong(long value) {
			return new PropertyVariant() { vt = (short) VarEnum.VT_I8, hVal = value };
		}

		/// <summary>
		/// Helper method to gets blob data
		/// </summary>
		private byte[] GetBlob() {
			var blob = new byte[blobVal.Length];
			Marshal.Copy(blobVal.Data, blob, 0, blob.Length);
			return blob;
		}

		/// <summary>
		/// Interprets a blob as an array of structs
		/// </summary>
		public T[] GetBlobAsArrayOf<T>() {
			var blobByteLength = blobVal.Length;
			var singleInstance = (T) Activator.CreateInstance(typeof(T));
			var structSize = Marshal.SizeOf(singleInstance);
			if (blobByteLength % structSize != 0) {
				throw new InvalidDataException(string.Format("Blob size {0} not a multiple of struct size {1}", blobByteLength, structSize));
			}
			var items = blobByteLength / structSize;
			var array = new T[items];
			for (int n = 0; n < items; n++) {
				array[n] = (T) Activator.CreateInstance(typeof(T));
				Marshal.PtrToStructure(new IntPtr((long) blobVal.Data + n * structSize), array[n]);
			}
			return array;
		}

		/// <summary>
		/// Gets the type of data in this PropVariant
		/// </summary>
		public VarEnum DataType {
			get { return (VarEnum) vt; }
		}

		/// <summary>
		/// Property value
		/// </summary>
		public object Value {
			get {
				VarEnum ve = DataType;
				switch (ve) {
					case VarEnum.VT_I1:
						return bVal;
					case VarEnum.VT_I2:
						return iVal;
					case VarEnum.VT_I4:
						return lVal;
					case VarEnum.VT_I8:
						return hVal;
					case VarEnum.VT_INT:
						return iVal;
					case VarEnum.VT_UI4:
						return ulVal;
					case VarEnum.VT_UI8:
						return uhVal;
					case VarEnum.VT_LPWSTR:
						return Marshal.PtrToStringUni(pointerValue);
					case VarEnum.VT_BLOB:
					case VarEnum.VT_VECTOR | VarEnum.VT_UI1:
						return GetBlob();
					case VarEnum.VT_CLSID:
						return Marshal.PtrToStructure<Guid>(pointerValue);
					case VarEnum.VT_BOOL:
						switch (boolVal) {
							case -1:
								return true;
							case 0:
								return false;
							default:
								throw new NotSupportedException("PropVariant VT_BOOL must be either -1 or 0");
						}
				}
				throw new NotImplementedException("PropVariant " + ve);
			}
		}

		/// <summary>
		/// Clears with a known pointer
		/// </summary>
		public static void Clear(IntPtr ptr) {
			PropVariantNative.PropVariantClear(ptr);
		}
	}
}