using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;

namespace app_iMeet
{
	public class TouchView :  UIView
	{
		public MKMapView _mapView;

		private MKAnnotation point;
		private bool HasPoint = false;

		public TouchView (RectangleF frame) : base(frame)
		{
			MultipleTouchEnabled = true;
			Hidden = true;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			UITouch aTouch = (UITouch)touches.AnyObject;
			PointF location = aTouch.LocationInView (this);
			
			var coordinate = _mapView.ConvertPoint (location, this);
			
			if (this.HasPoint) {
				_mapView.RemoveAnnotation (point);
				point = new PointAnnotation (coordinate, "Meeting Place", "Here");
				_mapView.AddAnnotation (point);
			} else {
				point = new PointAnnotation (coordinate, "Meeting Place", "Here");
				_mapView.AddAnnotation (point);
				this.HasPoint = true;
			}			
		}
	}
}

