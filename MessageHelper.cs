using System;
using MonoTouch.UIKit;
namespace app_iMeet
{
	public static class MessageHelper
	{
		public static void showCouldNotCreateMeeting ()
		{
			showErrorMesage ("Could not create meeting :(");
		}
		
		public static void showErrorMesage (string message)
		{
			
			using (UIAlertView alert = new UIAlertView ("iMeet", message, null, "OK", null)) 
				{
					alert.BeginInvokeOnMainThread(alert.Show);
					//alert.Show ();
				}
		}
		
		public static string cleanString(string stringToBeCleansed)
		{
			return stringToBeCleansed.Replace(" ","").Replace("<","").Replace(">","").Replace("'","");
		}
	}
}

