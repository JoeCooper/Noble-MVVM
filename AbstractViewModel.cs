using System;

namespace NobleMuffins.MVVM {
	public abstract class AbstractViewModel {
		
		public static void ShowViewModel<T>() where T: AbstractViewModel, new() {
			var viewModel = new T();
			var presenter = Singleton.Get<IPresenter>();
			presenter.Show(viewModel);
		}

        public static void ShowViewModel(AbstractViewModel viewModel)
        {
            var presenter = Singleton.Get<IPresenter>();
            presenter.Show(viewModel);
        }

        public virtual void Start() {
			//Empty default implementation
		}

		public virtual void Stop() {
			//Empty default implementation
		}
	}
}