#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Framework.Configuration
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Xml;
	using System.Text;

	public sealed class XmlAdapter : IDataAdapter, IDisposable
	{
		private sealed class XmlReaderContext
		{
			private readonly XmlDocument _document;
			private readonly Stack<XmlNode> _node;

			public XmlReaderContext(XmlDocument document)
			{
				Verify.Argument.IsNotNull(document, "document");
				
				_document = document;
				_node = new Stack<XmlNode>();
				_node.Push(_document.DocumentElement);
			}

			public XmlNode Node
			{
				get { return _node.Peek(); }
			}

			public string CurrentName
			{
				get { return Node.Name; }
			}

			public bool TryPush(string name)
			{
				var node = Node[name];
				if(node == null) return false;
				_node.Push(node);
				return true;
			}

			public XmlNode Push(string name)
			{
				var node = Node[name];
				Verify.Argument.IsTrue(node != null, "name", "Node not found.");
				_node.Push(node);
				return node;
			}

			public XmlNode Push()
			{
				var node = Node.ChildNodes[0];
				_node.Push(node);
				return node;
			}

			public bool Next()
			{
				var node = _node.Pop();
				node = node.NextSibling;
				if(node == null) return false;
				_node.Push(node);
				return true;
			}

			public XmlNode Pop()
			{
				return _node.Pop();
			}

			public bool HasAttribute(string name)
			{
				return Node.Attributes[name] != null;
			}

			public string LoadFromAttribute(string name)
			{
				var attr = Node.Attributes[XmlConvert.EncodeName(name)];
				if(attr == null) return string.Empty;
				return attr.Value;
			}

			public string LoadFromAttribute()
			{
				return LoadFromAttribute("Value");
			}

			public string LoadFromChildNode(string name)
			{
				var node = Node[XmlConvert.EncodeName(name)];
				if(node == null) return string.Empty;
				var attr = node.Attributes["Value"];
				if(attr == null) return string.Empty;
				return attr.Value;
			}
		}

		private sealed class XmlWriterContext
		{
			private readonly XmlDocument _document;
			private readonly Stack<XmlNode> _node;

			public XmlWriterContext(XmlDocument document)
			{
				Verify.Argument.IsNotNull(document, "document");
				
				_document = document;
				_node = new Stack<XmlNode>();
				_node.Push(document);
			}

			public XmlDocument Document
			{
				get { return _document; }
			}

			public XmlNode Node
			{
				get { return _node.Peek(); }
			}

			public XmlNode Push(string name)
			{
				var node = _document.CreateElement(XmlConvert.EncodeName(name));
				_node.Push(node);
				return node;
			}

			public XmlNode Pop()
			{
				var node = _node.Pop();
				Node.AppendChild(node);
				return node;
			}

			public void StoreAttribute(string name)
			{
				var attr = _document.CreateAttribute(XmlConvert.EncodeName(name));
				Node.Attributes.Append(attr);
			}

			public void StoreInAttribute(string value)
			{
				StoreInAttribute("Value", value);
			}

			public void StoreInAttribute(string name, string value)
			{
				var attr = _document.CreateAttribute(XmlConvert.EncodeName(name));
				attr.Value = value;
				Node.Attributes.Append(attr);
			}

			public void StoreInChild(string name, string value)
			{
				var node = _document.CreateElement(XmlConvert.EncodeName(name));
				var attr = _document.CreateAttribute("Value");
				attr.Value = value;
				node.Attributes.Append(attr);
				Node.AppendChild(node);
			}
		}


		private static string EncodeTypeName(Type type)
		{
			string shortName;
			var aqn = type.AssemblyQualifiedName;
			if(ReverseLookupTable.TryGetValue(aqn, out shortName))
			{
				return shortName;
			}
			else
			{
				var assembly = type.Assembly;
				if(!assembly.GlobalAssemblyCache)
				{
					aqn = type.FullName + ", " + assembly.GetName().Name;
				}
				return aqn;
			}
		}

		private static Type DecodeTypeName(string name)
		{
			string fullName;
			if(TypeLookupTable.TryGetValue(name, out fullName))
			{
				name = fullName;
			}
			return Type.GetType(name, false, false);
		}

		private interface IXmlPersister
		{
			Type Type { get; }

			void Store(XmlWriterContext context, Parameter parameter);

			object LoadValue(XmlReaderContext context);
		}

		private abstract class XmlPersister<T> : IXmlPersister
		{
			protected abstract void StoreCore(XmlWriterContext context, T value);

			protected abstract T LoadCore(XmlReaderContext context);

			public Type Type
			{
				get { return typeof(T); }
			}

			public void Store(XmlWriterContext context, Parameter parameter)
			{
				context.Push(parameter.Name);
				context.StoreInAttribute("Type", EncodeTypeName(parameter.Type));
				try
				{
					StoreCore(context, (T)parameter.Value);
				}
				finally
				{
					context.Pop();
				}
			}

			public object LoadValue(XmlReaderContext context)
			{
				return LoadCore(context);
			}
		}

		private sealed class BooleanPersister : XmlPersister<bool>
		{
			protected override void StoreCore(XmlWriterContext context, bool value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override bool LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToBoolean(context.LoadFromAttribute());
			}
		}

		private sealed class CharPersister : XmlPersister<char>
		{
			protected override void StoreCore(XmlWriterContext context, char value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override char LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToChar(context.LoadFromAttribute());
			}
		}

		private sealed class BytePersister : XmlPersister<byte>
		{
			protected override void StoreCore(XmlWriterContext context, byte value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override byte LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToByte(context.LoadFromAttribute());
			}
		}

		private sealed class SBytePersister : XmlPersister<sbyte>
		{
			protected override void StoreCore(XmlWriterContext context, sbyte value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override sbyte LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToSByte(context.LoadFromAttribute());
			}
		}

		private sealed class Int16Persister : XmlPersister<short>
		{
			protected override void StoreCore(XmlWriterContext context, short value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override short LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToInt16(context.LoadFromAttribute());
			}
		}

		private sealed class UInt16Persister : XmlPersister<ushort>
		{
			protected override void StoreCore(XmlWriterContext context, ushort value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override ushort LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToUInt16(context.LoadFromAttribute());
			}
		}

		private sealed class Int32Persister : XmlPersister<int>
		{
			protected override void StoreCore(XmlWriterContext context, int value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override int LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToInt32(context.LoadFromAttribute());
			}
		}

		private sealed class UInt32Persister : XmlPersister<uint>
		{
			protected override void StoreCore(XmlWriterContext context, uint value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override uint LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToUInt32(context.LoadFromAttribute());
			}
		}

		private sealed class Int64Persister : XmlPersister<long>
		{
			protected override void StoreCore(XmlWriterContext context, long value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override long LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToInt64(context.LoadFromAttribute());
			}
		}

		private sealed class UInt64Persister : XmlPersister<ulong>
		{
			protected override void StoreCore(XmlWriterContext context, ulong value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override ulong LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToUInt64(context.LoadFromAttribute());
			}
		}

		private sealed class SinglePersister : XmlPersister<float>
		{
			protected override void StoreCore(XmlWriterContext context, float value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override float LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToSingle(context.LoadFromAttribute());
			}
		}

		private sealed class DoublePersister : XmlPersister<double>
		{
			protected override void StoreCore(XmlWriterContext context, double value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override double LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToDouble(context.LoadFromAttribute());
			}
		}

		private sealed class DecimalPersister : XmlPersister<decimal>
		{
			protected override void StoreCore(XmlWriterContext context, decimal value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override decimal LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToDecimal(context.LoadFromAttribute());
			}
		}

		private sealed class DateTimePersister : XmlPersister<DateTime>
		{
			protected override void StoreCore(XmlWriterContext context, DateTime value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc));
			}

			protected override DateTime LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToDateTime(context.LoadFromAttribute(), XmlDateTimeSerializationMode.Utc);
			}
		}

		private sealed class DateTimeOffsetPersister : XmlPersister<DateTimeOffset>
		{
			protected override void StoreCore(XmlWriterContext context, DateTimeOffset value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override DateTimeOffset LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToDateTimeOffset(context.LoadFromAttribute());
			}
		}

		private sealed class TimeSpanPersister : XmlPersister<TimeSpan>
		{
			protected override void StoreCore(XmlWriterContext context, TimeSpan value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override TimeSpan LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToTimeSpan(context.LoadFromAttribute());
			}
		}

		private sealed class GuidPersister : XmlPersister<Guid>
		{
			protected override void StoreCore(XmlWriterContext context, Guid value)
			{
				context.StoreInAttribute(XmlConvert.ToString(value));
			}

			protected override Guid LoadCore(XmlReaderContext context)
			{
				return XmlConvert.ToGuid(context.LoadFromAttribute());
			}
		}

		private sealed class StringPersister : XmlPersister<string>
		{
			protected override void StoreCore(XmlWriterContext context, string value)
			{
				context.StoreInAttribute(value);
			}

			protected override string LoadCore(XmlReaderContext context)
			{
				return context.LoadFromAttribute();
			}
		}

		private sealed class PointPersister : XmlPersister<Point>
		{
			protected override void StoreCore(XmlWriterContext context, Point value)
			{
				context.StoreInAttribute("X", XmlConvert.ToString(value.X));
				context.StoreInAttribute("Y", XmlConvert.ToString(value.Y));
			}

			protected override Point LoadCore(XmlReaderContext context)
			{
				var x = XmlConvert.ToInt32(context.LoadFromAttribute("X"));
				var y = XmlConvert.ToInt32(context.LoadFromAttribute("Y"));
				return new Point(x, y);
			}
		}

		private sealed class SizePersister : XmlPersister<Size>
		{
			protected override void StoreCore(XmlWriterContext context, Size value)
			{
				context.StoreInAttribute("Width", XmlConvert.ToString(value.Width));
				context.StoreInAttribute("Height", XmlConvert.ToString(value.Height));
			}

			protected override Size LoadCore(XmlReaderContext context)
			{
				var w = XmlConvert.ToInt32(context.LoadFromAttribute("Width"));
				var h = XmlConvert.ToInt32(context.LoadFromAttribute("Height"));
				return new Size(w, h);
			}
		}

		private sealed class RectanglePersister : XmlPersister<Rectangle>
		{
			protected override void StoreCore(XmlWriterContext context, Rectangle value)
			{
				context.StoreInAttribute("X", XmlConvert.ToString(value.X));
				context.StoreInAttribute("Y", XmlConvert.ToString(value.Y));
				context.StoreInAttribute("Width", XmlConvert.ToString(value.Width));
				context.StoreInAttribute("Height", XmlConvert.ToString(value.Height));
			}

			protected override Rectangle LoadCore(XmlReaderContext context)
			{
				var x = XmlConvert.ToInt32(context.LoadFromAttribute("X"));
				var y = XmlConvert.ToInt32(context.LoadFromAttribute("Y"));
				var w = XmlConvert.ToInt32(context.LoadFromAttribute("Width"));
				var h = XmlConvert.ToInt32(context.LoadFromAttribute("Height"));
				return new Rectangle(x, y, w, h);
			}
		}

		private sealed class SizeFPersister : XmlPersister<SizeF>
		{
			protected override void StoreCore(XmlWriterContext context, SizeF value)
			{
				context.StoreInAttribute("Width", XmlConvert.ToString(value.Width));
				context.StoreInAttribute("Height", XmlConvert.ToString(value.Height));
			}

			protected override SizeF LoadCore(XmlReaderContext context)
			{
				var w = XmlConvert.ToSingle(context.LoadFromAttribute("Width"));
				var h = XmlConvert.ToSingle(context.LoadFromAttribute("Height"));
				return new SizeF(w, h);
			}
		}

		private sealed class PointFPersister : XmlPersister<PointF>
		{
			protected override void StoreCore(XmlWriterContext context, PointF value)
			{
				context.StoreInAttribute("X", XmlConvert.ToString(value.X));
				context.StoreInAttribute("Y", XmlConvert.ToString(value.Y));
			}

			protected override PointF LoadCore(XmlReaderContext context)
			{
				var x = XmlConvert.ToSingle(context.LoadFromAttribute("X"));
				var y = XmlConvert.ToSingle(context.LoadFromAttribute("Y"));
				return new PointF(x, y);
			}
		}

		private sealed class RectangleFPersister : XmlPersister<RectangleF>
		{
			protected override void StoreCore(XmlWriterContext context, RectangleF value)
			{
				context.StoreInAttribute("X", XmlConvert.ToString(value.X));
				context.StoreInAttribute("Y", XmlConvert.ToString(value.Y));
				context.StoreInAttribute("Width", XmlConvert.ToString(value.Width));
				context.StoreInAttribute("Height", XmlConvert.ToString(value.Height));
			}

			protected override RectangleF LoadCore(XmlReaderContext context)
			{
				var x = XmlConvert.ToSingle(context.LoadFromAttribute("X"));
				var y = XmlConvert.ToSingle(context.LoadFromAttribute("Y"));
				var w = XmlConvert.ToSingle(context.LoadFromAttribute("Width"));
				var h = XmlConvert.ToSingle(context.LoadFromAttribute("Height"));
				return new RectangleF(x, y, w, h);
			}
		}

		private sealed class EnumPersister : IXmlPersister
		{
			public Type Type
			{
				get { return typeof(Enum); }
			}

			public void Store(XmlWriterContext context, Parameter parameter)
			{
				context.Push(parameter.Name);
				context.StoreInAttribute("Type", EncodeTypeName(parameter.Type));
				try
				{
					context.StoreInAttribute(parameter.Value.ToString());
				}
				finally
				{
					context.Pop();
				}
			}

			public object LoadValue(XmlReaderContext context)
			{
				var type = DecodeTypeName(context.LoadFromAttribute("Type"));
				var value = context.LoadFromAttribute();
				return Enum.Parse(type, value);
			}
		}

		private sealed class ArrayPersister : IXmlPersister
		{
			public Type Type
			{
				get { return typeof(Array); }
			}

			public void Store(XmlWriterContext context, Parameter parameter)
			{
				var arr = (Array)parameter.Value;
				int rank = arr.Rank;
				int[] values = new int[rank];
				int[] ubounds = new int[rank];
				int[] lbounds = new int[rank];
				var sb = new StringBuilder();
				bool empty = true;
				for(int i = 0; i < rank; ++i)
				{
					lbounds[i] = values[i] = arr.GetLowerBound(i);
					ubounds[i] = arr.GetUpperBound(i);
					int l = ubounds[i] - lbounds[i] + 1;
					if(l > 0) empty = false;
					sb.Append(l);
					if(i != rank - 1)
						sb.Append(';');
				}
				context.Push(parameter.Name);
				context.StoreInAttribute("Type", EncodeTypeName(parameter.Type));
				context.StoreInAttribute("Array", sb.ToString());
				try
				{
					if(!empty)
					{
						var etype = parameter.Type.GetElementType();
						bool inc = true;
						while(inc)
						{
							var value = arr.GetValue(values);
							var p = new Parameter("Item", TypeHelpers.GetType(etype, value), value);
							ParameterPersister.Store(context, p);
							inc = Increment(values, lbounds, ubounds);
						}
					}
				}
				finally
				{
					context.Pop();
				}
			}

			public object LoadValue(XmlReaderContext context)
			{
				var type = DecodeTypeName(context.LoadFromAttribute("Type"));
				var etype = type.GetElementType();
				var sranks = context.LoadFromAttribute("Array").Split(';');
				var lengths = new int[sranks.Length];
				bool empty = true;
				for(int i = 0; i < lengths.Length; ++i)
				{
					lengths[i] = int.Parse(sranks[i], System.Globalization.NumberStyles.None);
					if(lengths[i] > 0) empty = false;
				}

				Array res;
				if(lengths.Length == 1)
					res = Array.CreateInstance(etype, lengths[0]);
				else
					res = Array.CreateInstance(etype, lengths);
				if(empty) return res;
				var values = new int[lengths.Length];
				bool inc = true;

				context.Push();
				try
				{
					if(!empty)
					{
						while(inc)
						{
							var p = ParameterPersister.Load(context);
							res.SetValue(p.Value, values);
							inc = Increment(values, lengths);
							if(inc) context.Next();
						}
					}
				}
				finally
				{
					context.Pop();
				}
				return res;
			}
		}

		private static readonly Dictionary<Type, IXmlPersister> Persisters = new Dictionary<Type, IXmlPersister>()
		{
			{ typeof(Boolean),			new BooleanPersister() },
			{ typeof(Char),				new CharPersister() },
			{ typeof(Byte),				new BytePersister() },
			{ typeof(SByte),			new SBytePersister() },
			{ typeof(Int16),			new Int16Persister() },
			{ typeof(UInt16),			new UInt16Persister() },
			{ typeof(Int32),			new Int32Persister() },
			{ typeof(UInt32),			new UInt32Persister() },
			{ typeof(Int64),			new Int64Persister() },
			{ typeof(UInt64),			new UInt64Persister() },
			{ typeof(Single),			new SinglePersister() },
			{ typeof(Double),			new DoublePersister() },
			{ typeof(Decimal),			new DecimalPersister() },

			{ typeof(DateTime),			new DateTimePersister() },
			{ typeof(DateTimeOffset),	new DateTimeOffsetPersister() },
			{ typeof(TimeSpan),			new TimeSpanPersister() },

			{ typeof(Guid),				new GuidPersister() },
			{ typeof(String),			new StringPersister() },

			{ typeof(Point),			new PointPersister() },
			{ typeof(Size),				new SizePersister() },
			{ typeof(Rectangle),		new RectanglePersister() },
			{ typeof(PointF),			new PointFPersister() },
			{ typeof(SizeF),			new SizeFPersister() },
			{ typeof(RectangleF),		new RectangleFPersister() },

			{ typeof(Enum),				new EnumPersister() },
			{ typeof(Array),			new ArrayPersister() },
		};

		private static readonly Dictionary<string, string> TypeLookupTable;

		private static readonly Dictionary<string, string> ReverseLookupTable;

		static XmlAdapter()
		{
			TypeLookupTable = new Dictionary<string, string>()
			{
				{ "Boolean", typeof(Boolean).AssemblyQualifiedName },
				{ "Char", typeof(Char).AssemblyQualifiedName },
				{ "Byte", typeof(Byte).AssemblyQualifiedName },
				{ "SByte", typeof(SByte).AssemblyQualifiedName },
				{ "Int16", typeof(Int16).AssemblyQualifiedName },
				{ "UInt16", typeof(UInt16).AssemblyQualifiedName },
				{ "Int32", typeof(Int32).AssemblyQualifiedName },
				{ "UInt32", typeof(UInt32).AssemblyQualifiedName },
				{ "Int64", typeof(Int64).AssemblyQualifiedName },
				{ "UInt64", typeof(UInt64).AssemblyQualifiedName },
				{ "Single", typeof(Single).AssemblyQualifiedName },
				{ "Double", typeof(Double).AssemblyQualifiedName },
				{ "Decimal", typeof(Decimal).AssemblyQualifiedName },

				{ "DateTime", typeof(DateTime).AssemblyQualifiedName },
				{ "DateTimeOffset", typeof(DateTimeOffset).AssemblyQualifiedName },
				{ "TimeSpan", typeof(TimeSpan).AssemblyQualifiedName },
	
				{ "Guid", typeof(Guid).AssemblyQualifiedName },
				{ "String", typeof(String).AssemblyQualifiedName },

				{ "Point", typeof(Point).AssemblyQualifiedName },
				{ "Size", typeof(Size).AssemblyQualifiedName },
				{ "Rectangle", typeof(Rectangle).AssemblyQualifiedName },
				{ "PointF", typeof(PointF).AssemblyQualifiedName },
				{ "SizeF", typeof(SizeF).AssemblyQualifiedName },
				{ "RectangleF", typeof(RectangleF).AssemblyQualifiedName },
				{ "FormWindowState", typeof(System.Windows.Forms.FormWindowState).AssemblyQualifiedName },
				{ "Orientation", typeof(System.Windows.Forms.Orientation).AssemblyQualifiedName },
			};

			ReverseLookupTable = new Dictionary<string,string>(TypeLookupTable.Count);
			foreach(var kvp in TypeLookupTable)
			{
				ReverseLookupTable.Add(kvp.Value, kvp.Key);
			}
		}

		private static bool Increment(int[] values, int[] lengths)
		{
			for(int i = 0; i < values.Length; ++i)
			{
				++values[i];
				if(values[i] >= lengths[i])
				{
					if(i == values.Length - 1) return false;
					values[i] = 0;
				}
				else
				{
					return true;
				}
			}
			return false;
		}

		private static bool Increment(int[] values, int[] lbounds, int[] ubounds)
		{
			for(int i = 0; i < values.Length; ++i)
			{
				++values[i];
				if(values[i] > ubounds[i])
				{
					if(i == values.Length - 1) return false;
					values[i] = lbounds[i];
				}
				else
				{
					return true;
				}
			}
			return false;
		}

		private static class ParameterPersister
		{
			public static void Store(XmlWriterContext context, Parameter parameter)
			{
				IXmlPersister persister = null;
				if(parameter.Type.IsEnum)
				{
					Persisters.TryGetValue(typeof(Enum), out persister);
				}
				else if(parameter.Type.IsArray)
				{
					Persisters.TryGetValue(typeof(Array), out persister);
				}
				else
				{
					Persisters.TryGetValue(parameter.Type, out persister);
				}
				if(persister != null)
				{
					persister.Store(context, parameter);
				}
			}

			public static Parameter Load(XmlReaderContext context)
			{
				var type = DecodeTypeName(context.LoadFromAttribute("Type"));
				if(type == null) throw new NotSupportedException();
				var name = context.CurrentName;
				IXmlPersister persister = null;
				if(type.IsEnum)
				{
					Persisters.TryGetValue(typeof(Enum), out persister);
				}
				else if(type.IsArray)
				{
					Persisters.TryGetValue(typeof(Array), out persister);
				}
				else
				{
					Persisters.TryGetValue(type, out persister);
				}
				if(persister != null)
				{
					return new Parameter(name, type, persister.LoadValue(context));
				}
				else
				{
					return new Parameter(name, type, null);
				}
			}
		}

		private static class SectionPersister
		{
			public static void Store(XmlWriterContext context, Section section)
			{
				context.Push(section.Name);
				int sections = section.SectionCount;
				int parameters = section.ParameterCount;
				if(parameters != 0 || sections != 0)
				{
					bool saveDirect = sections == 0 || parameters == 0;
					if(parameters == 0)
					{
						context.StoreInAttribute("Style", "NoParameters");
					}
					else if(sections == 0)
					{
						context.StoreInAttribute("Style", "NoSections");
					}
					if(section.SectionCount != 0)
					{
						if(!saveDirect)
						{
							context.Push("Sections");
						}
						try
						{
							foreach(var subsection in section.Sections)
							{
								SectionPersister.Store(context, subsection);
							}
						}
						finally
						{
							if(!saveDirect)
							{
								context.Pop();
							}
						}
					}
					if(section.ParameterCount != 0)
					{
						if(!saveDirect)
						{
							context.Push("Parameters");
						}
						try
						{
							foreach(var parameter in section.Parameters)
							{
								ParameterPersister.Store(context, parameter);
							}
						}
						finally
						{
							if(!saveDirect)
							{
								context.Pop();
							}
						}
					}
				}
				context.Pop();
			}

			public static Section Load(XmlReaderContext context)
			{
				var name = context.CurrentName;
				var res = new Section(name);
				bool empty = context.Node.ChildNodes.Count == 0;
				if(empty) return res;
				var style = context.LoadFromAttribute("Style");
				bool noSections = style == "NoSections";
				bool noParameters = !noSections && style == "NoParameters";
				if(noParameters || context.TryPush("Sections"))
				{
					try
					{
						foreach(XmlNode subsectionNode in context.Node.ChildNodes)
						{
							context.Push(subsectionNode.Name);
							try
							{
								res.AddSection(SectionPersister.Load(context));
							}
							finally
							{
								context.Pop();
							}
						}
					}
					finally
					{
						if(!noParameters)
						{
							context.Pop();
						}
					}
				}
				if(noSections || context.TryPush("Parameters"))
				{
					try
					{
						foreach(XmlNode parameterNode in context.Node.ChildNodes)
						{
							context.Push(parameterNode.Name);
							try
							{
								res.AddParameter(ParameterPersister.Load(context));
							}
							catch(Exception exc)
							{
								if(exc.IsCritical())
								{
									throw;
								}
							}
							finally
							{
								context.Pop();
							}
						}
					}
					finally
					{
						if(!noSections)
						{
							context.Pop();
						}
					}
				}
				return res;
			}
		}

		private readonly Stream _stream;

		public XmlAdapter(Stream stream)
		{
			Verify.Argument.IsNotNull(stream, "stream");

			_stream = stream;
		}

		public void Store(Section section)
		{
			var doc = new XmlDocument();
			var context = new XmlWriterContext(doc);
			SectionPersister.Store(context, section);
			using(var writer = XmlWriter.Create(_stream, new XmlWriterSettings()
				{
					CloseOutput = false,
					Encoding = System.Text.Encoding.UTF8,
					Indent = true,
					IndentChars = "\t",
				}))
			{
				doc.Save(writer);
			}
		}

		public Section Load()
		{
			var doc = new XmlDocument();
			doc.Load(_stream);
			var context = new XmlReaderContext(doc);
			return SectionPersister.Load(context);
		}

		public void Dispose()
		{
			_stream.Dispose();
		}
	}
}
