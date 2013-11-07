using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using DataModel;
using DataModel.DataContracts;
using System.ServiceModel;

namespace app_iMeet
{
	public partial class ConnectViewController : UIViewController
	{
		LoadingHUDView _loading;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public ConnectViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ConnectViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ConnectViewController () : base("ConnectViewController", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
			_loading = new LoadingHUDView("Loading iMeet", "");
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.Title = "Setup";
			
			//use documents directory
			var localNumber = ServiceHelper.instance.readUserPhoneNumber();
			// Skip setup if done before
			if (localNumber != string.Empty) 
			{
				//hide stuff
				this.Title = "iMeet";
				joinButton.Hidden = true;
				phoneTxt.Hidden = true;
				phoneLbl.Hidden = true;
				//add loading
				this.View.AddSubview(_loading);
				_loading.StartAnimating();

				ServiceHelper.instance.loginUser(localNumber,userLoggedIn);
			} 
			else 
			{
				joinButton.TouchUpInside += delegate 
				{
					File.WriteAllText (ServiceHelper.instance.filePath, phoneTxt.Text.Replace(" ", string.Empty).Trim());
					//add loading
					this.View.AddSubview(_loading);
					_loading.StartAnimating();
					//login
					ServiceHelper.instance.loginUser (phoneTxt.Text.Replace(" ", string.Empty),userLoggedIn);
				};
			}			
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			_loading.StopAnimating();
			_loading.RemoveFromSuperview();	//remove loading
		}

		

		public void userLoggedIn (Object sender, LoginUserCompletedEventArgs e)
		{
			ServiceHelper.instance.localPhoneNumber = e.Result.LoggedInUser.phoneNumber;
			ServiceHelper.instance.localuserID = e.Result.LoggedInUser.userId;
			if (e.Result.LoggedInUser.userId != Guid.Empty) {
				
				InvokeOnMainThread (delegate() { this.NavigationController.PushViewController (new ContactsViewController (), false); });
			}
		}
	}
}


