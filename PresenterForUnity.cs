using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NobleMuffins.MVVM
{
	public class PresenterForUnity: MonoBehaviour, IPresenter
	{
		private readonly IDictionary<AbstractViewModel, GameObject> extantViews;
        private AsyncOperation loadingOperation;
		private bool wasLoading;

        public GameObject loadingOverlay;
        		
		public PresenterForUnity (): base()
		{
			wasLoading = true;
			
			extantViews = new Dictionary<AbstractViewModel, GameObject> ();
        }

		public void Show<TViewModel> () where TViewModel: AbstractViewModel, new() {
			var viewModel = new TViewModel ();
			Show<TViewModel>(viewModel);
		}

		public void Show<TViewModel>(TViewModel viewModel) where TViewModel: AbstractViewModel {
			if (extantViews.ContainsKey (viewModel) == false) {
				UnityEngine.Object prefab = null;
                var _controllerType = typeof(AbstractViewController<>);
                var typeArgs = new [] { viewModel.GetType() };
                var controllerType = _controllerType.MakeGenericType(typeArgs);
				var assembly = Assembly.GetExecutingAssembly();
				var types = assembly.GetTypes();
				foreach(var type in types) {
					var isRelevant = controllerType.IsAssignableFrom(type);
					var isConcretion = type.IsAbstract == false;
					if(isRelevant && isConcretion) {
						var candidates = Resources.LoadAll(string.Empty, type);
						if(candidates.Length > 0) {
							prefab = candidates[0];
						}
						if(candidates.Length > 1) {
							Debug.LogWarningFormat("Found multiple view controllers for view model type {0}.", viewModel.GetType().Name);
						}
					}
				}
				if(prefab != null) {
					var viewController = (AbstractViewController)GameObject.Instantiate (prefab);
                    GameObject.DontDestroyOnLoad(viewController.gameObject);
					if(!string.IsNullOrEmpty(viewController.preferredScene) && (SceneManager.GetActiveScene().name != viewController.preferredScene)) {
                        loadingOperation = SceneManager.LoadSceneAsync(viewController.preferredScene, LoadSceneMode.Single);
					}
					var toClose = extantViews.Values.ToArray ();
					foreach (var v in toClose) {
						GameObject.Destroy(v.gameObject);
					}
					extantViews.Clear();
                    viewController.ViewModel = viewModel;
                    extantViews [viewModel] = viewController.gameObject;
				} else {
					Debug.LogErrorFormat("Failed to resolve view controller for {0}", viewModel.GetType().Name);
				}
			} else {
				Debug.LogWarningFormat ("PresenterForUnity asked to show viewModel represented in extant views: {0}");
			}
		}

        bool IsLoading
        {
            get
            {
                return loadingOperation != null && !loadingOperation.isDone;
            }
        }

        void Update()
        {
			if (wasLoading != IsLoading)
            {
                loadingOverlay.SetActive(IsLoading);
            }
			wasLoading = IsLoading;
        }
	}
}

