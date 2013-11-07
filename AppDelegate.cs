using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using System.ServiceModel;
using DataModel;
using DataModel.DataContracts;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.AddressBook;

namespace app_iMeet
{
	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		UINavigationController navigationController;

		ConnectViewController connectViewController;
		
		string launchWithCustomKeyValue = string.Empty;
		
		//LoadingHUDView _loading;

		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (UIRemoteNotificationType.Alert 
			                                                                    | UIRemoteNotificationType.Badge 
			                                                                    | UIRemoteNotificationType.Sound);
			
			//The NSDictionary options variable would contain our notification data if the user clicked the 'view' button on the notification 
			// to launch the application.  So you could process it here.  I find it nice to have one method to process these options from the 
			// FinishedLaunching, as well as the ReceivedRemoteNotification methods.  
			if (options != null)
				processNotification (options, true);
			
			//See if the custom key value variable was set by our notification processing method
			if (!string.IsNullOrEmpty (launchWithCustomKeyValue)) {
				//Bypass the normal view that shows when launched and go right to something else since the user
				// launched with some custom value (eg: from a remote notification's 'View' button being pressed, or from a url handler)
				
				//TODO: Insert your own logic here				
			}
			
			// If you have defined a view, add it here:
			// window.AddSubview (navigationController.View);
			
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			connectViewController = new ConnectViewController ();
			
			navigationController = new UINavigationController (connectViewController);
			
			navigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			navigationController.NavigationBar.TintColor = UIColor.DarkGray;
			
			window.AddSubview (navigationController.View);
			
			window.MakeKeyAndVisible ();
			
			return true;
		}
		
		// Handle Notifications
		void processNotification (NSDictionary options, bool fromFinishedLaunching)
		{
			CheckForNewMeetings();
		}
		
		void CheckForNewMeetings()
		{
			if(ServiceHelper.instance.localPhoneNumber == null || ServiceHelper.instance.localPhoneNumber == string.Empty)
			{
				var phoneNumber = ServiceHelper.instance.readUserPhoneNumber();
				if(phoneNumber != string.Empty)
				{
					ServiceHelper.instance.loginUser(phoneNumber,cornercaseUserLoggedIn);
					
				}
				else
				{
					MessageHelper.showErrorMesage("User must do first login");
				}
			}
			else
			{	
				ServiceHelper.instance.getMeetingByPhoneNumber(ServiceHelper.instance.localPhoneNumber,HandleNewMeetings);						
			}
		}
		
		void HandleNewMeetings(Object sender, GetMeetingCompletedEventArgs e)
		{
			
			//List<string> numbers = new List<string>();

			foreach (var user in e.Result.Meeting.Users.Where(user => user.phoneNumber != ServiceHelper.instance.localPhoneNumber))
			{	
				//numbers.Add(user.phoneNumber); //add all contacts phones, not user	
				ServiceHelper.instance.targetPhoneNumber = user.phoneNumber;
				ServiceHelper.instance.targetUserID = user.userId;
			}
			
			ServiceHelper.instance.currentMeeting = e.Result.Meeting;
			
			//ABAddressBook addressBook = new ABAddressBook ();
			//ABPerson[] allContacts = addressBook.GetPeople ();
			
			//var requestingUser = e.Result.Meeting.Users.Where(x => x.phoneNumber != ServiceHelper.instance.localPhoneNumber).FirstOrDefault();
			
			InvokeOnMainThread (delegate() {
				ShowAlert(ServiceHelper.instance.targetPhoneNumber.ToString());			
			});		
		}
				
		void cornercaseUserLoggedIn(object sender, LoginUserCompletedEventArgs e)
		{
			CheckForNewMeetings();
		}		

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			//This method gets called whenever the app is already running and receives a push notification
			// YOU MUST HANDLE the notifications in this case.  Apple assumes if the app is running, it takes care of everything
			// this includes setting the badge, playing a sound, etc.
			if (userInfo != null)
				processNotification (userInfo, false);
		}

		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{			
			//The deviceToken is of interest here, this is what your push notification server needs to send out a notification
			// to the device.  So, most times you'd want to send the device Token to your servers when it has changed

			//First, get the last device token we know of
			string lastDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("deviceToken");
			
			//There's probably a better way to do this
			NSString strFormat = new NSString ("%@");
			NSString newDeviceToken = new NSString (MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (new MonoTouch.ObjCRuntime.Class ("NSString").Handle, new MonoTouch.ObjCRuntime.Selector ("stringWithFormat:").Handle, strFormat.Handle, deviceToken.Handle));
			
			
			//We only want to send the device token to the server if it hasn't changed since last time
			// no need to incur extra bandwidth by sending the device token every time 
			if (!newDeviceToken.Equals (lastDeviceToken)) {
				
				
				//TODO: Insert your own code to send the new device token to your server		
				
				//Save the new device token for next application launch
				NSUserDefaults.StandardUserDefaults.SetString (newDeviceToken, "deviceToken");
			}
		}

		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			//Registering for remote notifications failed for some reason
			//This is usually due to your provisioning profiles not being properly setup in your project options
			// or not having the right mobileprovision included on your device
			// or you may not have setup your app's product id to match the mobileprovision you made
			
			//Console.WriteLine ("Failed to Register for Remote Notifications: {0}", error.LocalizedDescription);
			
			using (var alert = new UIAlertView ("Notification Error", error.LocalizedDescription, null, "OK", null)) {
					alert.Show ();
				}
		}
		
		//alert stuff
		void ShowAlert(string name)
		{
			/*
			UIAlertView alert = new UIAlertView();
			alert.Title = "Meeting Request";
			alert.AddButton("OK");
			alert.AddButton("No Thanks");
			alert.Message = "Do you wish to meeting with " + name + "?";			
			alert.Clicked += HandleAlertClicked;
			navigationController.View.AddSubview(alert);
			navigationController.View.LayoutSubviews();
			navigationController.View.BringSubviewToFront(alert);
			alert.Show();	
			*/
			navigationController.PresentModalViewController(new ShowViewController(name, navigationController), true);
		}
		/*
		void HandleAlertClicked (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0)
			{
				InvokeOnMainThread (delegate() { navigationController.PushViewController (new MapViewController(), true); });
			}
		}
		*/
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
}
}