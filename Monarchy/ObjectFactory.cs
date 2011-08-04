using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Monarchy {
	public class ObjectFactory {
		static readonly IDictionary<Type, object> Container = new Dictionary<Type, object>();

		public static T Get<T>() {
			return (T) Get(typeof (T));
		}

		public static object Get(Type type) {
			try {
				return _Get(type);
			}
			catch (Exception ex) {
				throw new Exception("Monarchy threw an exception: " + ex.Message);
			}
		}

		private static object _Get(Type type) {
			if (Container.ContainsKey(type))
				return Activator.CreateInstance(Container[type].GetType());
			else {
				ConstructorInfo[] ctors = type.GetConstructors();
				ConstructorInfo largestCtor = ctors.OrderByDescending(c => c.GetParameters().Length).First();
				var args = new object[largestCtor.GetParameters().Length];
				Array.ForEach(largestCtor.GetParameters(), p => { args[p.Position] = _Get(p.ParameterType); });
				return Activator.CreateInstance(type, args);
			}
		}

		 public static void Register(object instance) {
			Container[instance.GetType()] = instance;
		}
	}
}