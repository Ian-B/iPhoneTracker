using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreLocation;

namespace app_iMeet
{
	public class LocationDelegate : CLLocationManagerDelegate
	{
		public LocationDelegate ()
		{
		}

		public override void Failed (CLLocationManager locationMgr, NSError e)
		{
			switch (e.Code) {
				/*
			case (int)CLError.LocationUnknown:
				using (var alert = new UIAlertView ("Location Unkown", "Failed to get Location", null, "OK", null)) {
					alert.Show ();
				}

				break;
				*/
			case (int)CLError.Denied:
				locationMgr.StopUpdatingLocation ();
				using (var alert = new UIAlertView ("Location Unavailable", "Location Service is Turned Off", null, "OK", null)) {
					alert.Show ();
				}

				break;
			case (int)CLError.Network:
				using (var alert = new UIAlertView ("Network Unavailable", "Network Unreachable", null, "OK", null)) {
					alert.Show ();
				}
				break;
			}
		}
	}
}

