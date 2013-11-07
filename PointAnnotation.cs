using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;

namespace app_iMeet
{
	public class PointAnnotation : MKAnnotation
	{
		string _title, _subtitle;
		public PointAnnotation (CLLocationCoordinate2D c, string title, string subtitle) : base()
		{
			_title = title;
			_subtitle = subtitle;
			Coordinate = c;
			//Console.WriteLine("Newly created pin at {0},{1}", c.Latitude, c.Longitude);
		}
		public override CLLocationCoordinate2D Coordinate {get;set;}
		
		public override string Title {
			get {
				return _title;
			}
		}
		public override string Subtitle {
			get {
				return _subtitle;
			}
		}
	}
}




	



