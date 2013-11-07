
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace app_iMeet
{
	public partial class SignUpViewController : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public SignUpViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public SignUpViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public SignUpViewController () : base("SignUpViewController", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		UIBarButtonItem backButton;	
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			backButton = new UIBarButtonItem();
			backButton.Title = "Back";
			
			this.Title = "Sign Up";
			this.NavigationItem.LeftBarButtonItem = backButton;	
			
			//back button
			backButton.Clicked += delegate
			{
				this.NavigationController.PopToRootViewController(true);				
			};	
			
			//this.passTxt; //password
			//this.userTxt; //username
			//this.phoneTxt; //phone number
			
			//register
			registerButton.TouchUpInside += delegate 
			{
				this.NavigationController.PopToRootViewController(true);
			};
		}
	}
}

