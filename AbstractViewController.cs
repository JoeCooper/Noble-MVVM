using System;
using UnityEngine;

namespace NobleMuffins.MVVM
{
    public abstract class AbstractViewController: MonoBehaviour
    {
        public AbstractViewModel ViewModel { get; set; }

        public string preferredScene;

        protected void Start()
        {
            if (Application.isEditor && ViewModel == null)
            {
                try
                {
                    ViewModel = GetViewModelForTestPurposes();
                }
                catch(Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (ViewModel == null)
            {
                Debug.LogWarning("ViewController launched with no ViewModel. Self destructing.", this);
                GameObject.Destroy(gameObject);
            }

            ViewModel.Start();
        }

        protected void OnDestroy()
        {
            ViewModel.Stop();
        }

        protected virtual AbstractViewModel GetViewModelForTestPurposes()
        {
            throw new NotImplementedException();
        }

    }

    public abstract class AbstractViewController<TViewModel>: AbstractViewController where TViewModel: AbstractViewModel
	{
		public new TViewModel ViewModel
        {
            get
            {
                return (TViewModel) base.ViewModel;
            }
            set
            {
                base.ViewModel = value;
            }
        }        
	}
}

