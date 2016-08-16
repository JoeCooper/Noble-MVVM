namespace NobleMuffins.MVVM {
	public interface IPresenter {
		void Show<TViewModel> () where TViewModel: AbstractViewModel, new();
		void Show<TViewModel> (TViewModel viewModel) where TViewModel: AbstractViewModel;
	}
}