using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeEditor.Composition.Primitives
{
	public class ImportDefinitionProvider
	{
		public IEnumerable<ImportDefinition> ImportsFor(Type type)
		{
			return type
				.InstanceMembers()
				.Select(m => new {Member = m, Attribute = ImportAttributeFrom(m)})
				.Where(m => m.Attribute != null)
				.Select(m => FromMember(m.Member, m.Attribute));
		}

		private static ImportDefinition FromMember(MemberInfo member, ImportAttribute import)
		{
			var property = member as PropertyInfo;
			if (property != null)
				return FromProperty(property, import);

			var field = member as FieldInfo;
			if (field != null)
				return FromField(field, import);

			throw new CompositionException(new CompositionError(import.ContractType, string.Format("Unsupported import `{0}'.", member)));
		}

		private static ImportDefinition FromField(FieldInfo fieldInfo, ImportAttribute import)
		{
			return ImportDefinitionFrom(import, fieldInfo.FieldType, fieldInfo.SetValue);
		}

		private static ImportDefinition FromProperty(PropertyInfo p, ImportAttribute import)
		{
			return ImportDefinitionFrom(import, p.PropertyType, (part, value) => p.SetValue(part, value, null));
		}

		private static ImportDefinition ImportDefinitionFrom(ImportAttribute import, Type actualType, Action<object, object> setter)
		{
			return new ImportDefinitionBuilder(import, actualType, setter).Build();
		}

		private static ImportAttribute ImportAttributeFrom(MemberInfo member)
		{
			return CustomAttribute<ImportAttribute>.From(member);
		}
	}

	internal class ImportDefinitionBuilder
	{
		private readonly ImportAttribute _import;
		private readonly Type _actualType;
		private readonly Action<object, object> _setter;
		private readonly bool _isLazyType;
		private readonly Type _contractType;
		private readonly Type _elementType;

		public ImportDefinitionBuilder(ImportAttribute import, Type actualType, Action<object, object> setter)
		{
			_import = import;
			_actualType = actualType;
			_setter = setter;
			_elementType = ElementType();
			_isLazyType = IsLazyType(_elementType);
			_contractType = ContractType();
		}

		public ImportDefinition Build()
		{
			return new ImportDefinition(_contractType, Cardinality, BuildSetter(), Constraint());
		}

		private Func<Export, bool> Constraint()
		{
			if (_isLazyType && MetadataType != null)
				return MetadataConstraintFor(MetadataType);
			return _ => true;
		}

		private static Func<Export, bool> MetadataConstraintFor(Type metadataType)
		{
			return export => export.Metadata.Any(metadataType.IsInstanceOfType);
		}

		private ImportCardinality Cardinality
		{
			get { return _import.Cardinality; }
		}

		private Action<Export[], object> BuildSetter()
		{
			return _isLazyType
				? LazySetterFor(_elementType, _setter, Cardinality, MetadataType)
				: EagerSetterFor(_elementType, _setter, Cardinality);
		}

		private Type MetadataType
		{
			get
			{
				var genericArguments = _elementType.GetGenericArguments();
				return genericArguments.Length == 2 ? genericArguments[1] : null;
			}
		}

		private static Action<Export[], object> LazySetterFor(Type elementType, Action<object, object> setter, ImportCardinality cardinality, Type metadataType)
		{
			if (cardinality == ImportCardinality.Many)
				return (exports, part) => setter(part, ArrayOf(elementType, exports.Select(e => LazyInstanceFor(elementType, metadataType, e))));
			return (exports, part) => setter(part, LazyInstanceFor(elementType, metadataType, exports.Single()));
		}

		private static Action<Export[], object> EagerSetterFor(Type elementType, Action<object, object> setter, ImportCardinality cardinality)
		{
			if (cardinality == ImportCardinality.Many)
				return (exports, part) => setter(part, ArrayOf(elementType, exports.Select(e => e.Value)));
			return (exports, part) => setter(part, exports.Single().Value);
		}

		private static Array ArrayOf(Type elementType, IEnumerable<object> elements)
		{
			var source = elements.ToArray();
			var result = Array.CreateInstance(elementType, source.Length);
			Array.Copy(source, result, source.Length);
			return result;
		}

		private static object LazyInstanceFor(Type lazyType, Type metadataType, Export export)
		{
			Func<object> factory = () => export.Value;
			if (metadataType == null)
				return lazyType.GetMethod("FromUntyped").Invoke(null, new object[] { factory });
			var metadata = export.Metadata.Single(metadataType.IsInstanceOfType);
			return lazyType.GetMethod("FromUntypedWithMetadata").Invoke(null, new[] { factory, metadata });
		}

		private Type ContractType()
		{
			return _import.ContractType ?? InferContractTypeFromElementType();
		}

		private Type InferContractTypeFromElementType()
		{
			return _isLazyType
				? _elementType.GetGenericArguments()[0]
				: _elementType;
		}

		private Type ElementType()
		{
			return _import.Cardinality == ImportCardinality.Many
				? _actualType.GetElementType()
				: _actualType;
		}

		private static bool IsLazyType(Type t)
		{
			if (!t.IsGenericType)
				return false;
			var definition = t.GetGenericTypeDefinition();
			return definition == typeof(Lazy<,>) || definition == typeof(Lazy<>);
		}
	}
}
