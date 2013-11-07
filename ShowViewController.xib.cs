
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace app_iMeet
{
	public partial class ShowViewController : UIViewController
	{
		UINavigationController _controller;
		string _phone = string.Empty;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public ShowViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ShowViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ShowViewController (string phoneNumber, UINavigationController controller) : base("ShowViewController", null)
		{
			_controller = controller;
			_phone = phoneNumber;
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			phoneLbl.Text = _phone;
			
			okBtn.TouchUpInside += delegate {
				this.DismissModalViewControllerAnimated(true);
				
				_controller.PushViewController(new MapViewController(), true);
			};
			
			noBtn.TouchUpInside += delegate {
				this.DismissModalViewControllerAnimated(true);
			};
		}
	}
}

