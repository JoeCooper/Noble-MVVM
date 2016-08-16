using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NobleMuffins.MVVM {
	public static class Singleton {

		private static readonly IDictionary<Type,object> Map = new Dictionary<Type,object>();

        public static void Register<T>(T t) where T: class {

            var givenType = typeof(T);

            Map[givenType] = t;
        }

		public static T Get<T>() where T: class {
			var givenType = typeof(T);

			object result = null;

			Map.TryGetValue(givenType, out result);

			if(result == null) {
				foreach(var candidate in Map.Values) {
					if(candidate is T) {
						result = candidate;
						break;
					}
				}

				if(result != null) {
					Map[givenType] = result;
				}
			}

			if(result == null) {
				var assembly = Assembly.GetExecutingAssembly();
				var types = assembly.GetTypes();
				foreach(var type in types) {
					var isRelevant = givenType.IsAssignableFrom(type);
					var isConcretion = type.IsAbstract == false;
					if(isRelevant && isConcretion) {
						if(type.IsSubclassOf(typeof(MonoBehaviour))) {
							result = GameObject.FindObjectOfType(type);
							if(result == null) {
								var go = new GameObject();
								go.name = string.Format("{0} (Singleton)", givenType.Name);
								GameObject.DontDestroyOnLoad(go);
								result = go.AddComponent(type);
							}
                            else
                            {
                                var go = ((MonoBehaviour)result).gameObject;
                                GameObject.DontDestroyOnLoad(go);
                            }
						} else {
							result = Activator.CreateInstance(type);
						}
						Map[givenType] = result;
						Map[type] = result;
					}
				}
			}

			if(result == null) {
				throw new Exception("Failed to resolve or create instance of type " + givenType.ToString());
			}

			return (T) result;
		}
	}
}