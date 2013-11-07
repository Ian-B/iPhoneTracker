using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;
using MonoTouch.AddressBook;
using System.ServiceModel;
using DataModel;
using DataModel.DataContracts;
using MonoTouch.ObjCRuntime;
using System.Linq;
using System.IO;

namespace app_iMeet
{
	public class MapViewController : UIViewController
	{
		MKMapView _mapView;
		TouchView _touchView;
		CLLocationManager _locationMgr;
		MKAnnotation _trackPoint;
		CLLocationCoordinate2D _trackCoordinate;
		CLLocationCoordinate2D _userLocation;
		UILabel _distanceLbl;
		NSTimer _timer;

		public MKPolyline RouteLine;
		public MKPolylineView RouteLineView;

		public MapViewController () : base()
		{
			try {
				_trackPoint = null;
				_trackCoordinate = new CLLocationCoordinate2D (17.731693, -102.030657);
				_userLocation = new CLLocationCoordinate2D (17.731693, -102.030657);
				
				//initial
			} catch (Exception e) {
				MessageHelper.showErrorMesage ("Problem in ctor of mapviewcontroller " + e.Message);
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		// Load View
		public override void ViewDidLoad ()
		{
			try {
				base.ViewDidLoad ();
				
				CLLocationCoordinate2D[] points = new CLLocationCoordinate2D[2];				
				points[0] = _userLocation;
				points[1] = _trackCoordinate;
				points.Count();
				
				RouteLine = MKPolyline.FromCoordinates(points);				
				
				this.Title = "Tracker";
				
				_mapView = new MKMapView ();
				_mapView.Frame = new RectangleF (0, 0, this.View.Frame.Width, this.View.Frame.Height);
				//_mapView.SizeToFit();
				
				//initialize catch touch Events
				_touchView = new TouchView (new RectangleF (0, 0, this.View.Frame.Width, this.View.Frame.Height));
				_touchView.BackgroundColor = UIColor.Clear;
				_touchView._mapView = _mapView;
				
				var segmentedControl = new UISegmentedControl ();
				segmentedControl.Frame = new RectangleF (20, 360, 282, 30);
				segmentedControl.InsertSegment ("Map", 0, false);
				segmentedControl.InsertSegment ("Satellite", 1, false);
				segmentedControl.InsertSegment ("Hybrid", 2, false);
				segmentedControl.SelectedSegment = 0;
				segmentedControl.ControlStyle = UISegmentedControlStyle.Bar;
				segmentedControl.TintColor = UIColor.DarkGray;
				
				segmentedControl.ValueChanged += delegate {
					if (segmentedControl.SelectedSegment == 0)
						_mapView.MapType = MKMapType.Standard; else if (segmentedControl.SelectedSegment == 1)
						_mapView.MapType = MKMapType.Satellite; else if (segmentedControl.SelectedSegment == 2)
						_mapView.MapType = MKMapType.Hybrid;
				};
				
				_distanceLbl = new UILabel();
				_distanceLbl.Frame = new RectangleF(0, 0, 45, 28);
				_distanceLbl.BackgroundColor = UIColor.FromRGBA(0.0f, 0.0f, 0.0f, 0.75f);
				_distanceLbl.TextColor = UIColor.White;
				_distanceLbl.Font = UIFont.SystemFontOfSize(13);
				_distanceLbl.Text = "> 9000";
				
				// Add Views
				this.View.AddSubview (_mapView);
				View.AddSubview (_touchView);
				this.View.AddSubview (segmentedControl);
				this.View.AddSubview(_distanceLbl);
				
//				// Map Info Button
//				var infoButton = UIButton.FromType (UIButtonType.InfoDark);
//				infoButton.Frame = new RectangleF (288, 14, 22, 22);
//				infoButton.Title (UIControlState.Normal);
//				//this.View.AddSubview (infoButton);
//				
//				// Map add button
//				var addButton = UIButton.FromType (UIButtonType.RoundedRect);
//				addButton.Frame = new RectangleF (288, 44, 22, 22);
//				addButton.Title (UIControlState.Normal);
//				//this.View.AddSubview (addButton);
//				
//				// Map set Button
//				var setButton = UIButton.FromType (UIButtonType.RoundedRect);
//				setButton.Frame = new RectangleF (288, 74, 22, 22);
//				setButton.Title (UIControlState.Normal);
//				//this.View.AddSubview (setButton);
				
				// Location Mgr
				_locationMgr = new CLLocationManager ();
				_locationMgr.Delegate = new LocationDelegate ();
				
				_locationMgr.DesiredAccuracy = -1;
				//best
				_locationMgr.DistanceFilter = 2;
				//update every 2 meters??
				_locationMgr.StartUpdatingLocation ();
				
				_mapView.ShowsUserLocation = true;
				//show user
				_mapView.UserLocation.Title = "You are Here!";
				
				_mapView.AddOverlay(RouteLine);
				
//				// button events		
//				addButton.TouchUpInside += delegate { _touchView.Hidden = false; };
//				
//				setButton.TouchUpInside += delegate { _touchView.Hidden = true; };
			
				_timer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromSeconds(11),hack);
				
			} catch (Exception e) {				
				MessageHelper.showErrorMesage ("problem in viewdidload" + e.Message);
			}
		}
		
		void hack()
		{
			try
			{
				_timer.Dispose();
				updateMeeting();
				_timer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromSeconds(11),hack);
			}
			catch(Exception e)
			{
				//if the semaphore is full, do nothing.
			}
		}
		
		public void updateTrackLocation ()
		{
			try {
					// no gps update yet(not on screen), -180, -180
					if (_mapView.UserLocation.Coordinate.Latitude != -180 && _mapView.UserLocation.Coordinate.Longitude != -180) 
					{
						if (_trackCoordinate.IsValid()) 
						{
							if (_trackPoint == null) 
							{
								_trackPoint = new PointAnnotation (_trackCoordinate, "Contact", "Here");
								_mapView.AddAnnotation(_trackPoint);
							}
					
							_userLocation = _mapView.UserLocation.Coordinate;
					
							if(_userLocation.IsValid() && RouteLine != null)
							{
								_userLocation = _mapView.UserLocation.Coordinate;
								RouteLine.Points[0].X = _userLocation.Latitude;
								RouteLine.Points[0].Y = _userLocation.Longitude;
								RouteLine.Points[1].X = _trackCoordinate.Latitude;
								RouteLine.Points[1].Y = _trackCoordinate.Longitude;
								
								_trackPoint.Coordinate = _trackCoordinate;
								_mapView.AddAnnotation(_trackPoint);
								//put in line here							
								//_mapView.AddOverlay(RouteLine);
							
								ZoomIn();
							}
							else
							{
								throw new Exception("_UserLocation is null");
							}
						
							//calc dist
							if (_mapView.UserLocation != null)
							{
								CLLocation location = new CLLocation(_trackCoordinate.Latitude, _trackCoordinate.Longitude);
								var distance = Math.Round(_mapView.UserLocation.Location.Distancefrom(location), 2);
								if (distance > 9000)
								{
									_distanceLbl.Text = "> 9000";
								}
								else
								{
									_distanceLbl.Text = distance.ToString();
								}
							}
						}
					ServiceHelper.instance.releaseSemaphore();
					}
			} catch (Exception e) {
				MessageHelper.showErrorMesage ("Problem in updateTrackLocation " + e.Message);
			}
		}

		//zoom
		public void ZoomIn ()
		{
			//Zoom in
			
			// user location
			var userCoord = new CLLocationCoordinate2D { Latitude = _userLocation.Latitude, Longitude = _userLocation.Longitude };
			// tracking location
			var trackCoord = new CLLocationCoordinate2D { Latitude = _trackCoordinate.Latitude, Longitude = _trackCoordinate.Longitude };
			
			List<CLLocationCoordinate2D> annotations = new List<CLLocationCoordinate2D> ();
			annotations.Add (userCoord);
			annotations.Add (trackCoord);
			
			var tl = new CLLocationCoordinate2D (-90, 180);
			//top left
			var br = new CLLocationCoordinate2D (90, -180);
			//bottom right
			foreach (var anno in annotations) {
				tl.Longitude = Math.Min (tl.Longitude, anno.Longitude);
				tl.Latitude = Math.Max (tl.Latitude, anno.Latitude);
				br.Longitude = Math.Max (br.Longitude, anno.Longitude);
				br.Latitude = Math.Min (br.Latitude, anno.Latitude);
			}
			var center = new CLLocationCoordinate2D { Latitude = tl.Latitude - (tl.Latitude - br.Latitude) * 0.5, Longitude = tl.Longitude + (br.Longitude - tl.Longitude) * 0.5 };
			var span = new MKCoordinateSpan { LatitudeDelta = Math.Abs (tl.Latitude - br.Latitude) * 1.6, LongitudeDelta = Math.Abs (br.Longitude - tl.Longitude) * 1.6 };
			var region = new MKCoordinateRegion { Center = center, Span = span };
			region = _mapView.RegionThatFits (region);
			_mapView.SetRegion (region, true);
		}

		// map view delegate
		class MapViewDelegate : MKMapViewDelegate
		{
			MapViewController _viewController;
			public MapViewDelegate (MapViewController viewController)
			{
				try {
					_viewController = viewController;
				} catch (Exception e) {
					MessageHelper.showErrorMesage ("ctor MapViewDelegate " + e.Message);
				}
			}

			public override MKOverlayView GetViewForOverlay (MKMapView mapView, NSObject overlay)
			{
				try {
					MKOverlayView overlayView = null;
					if (overlay == _viewController.RouteLine) {
						//if (null == _viewController.RouteLineView) {
						_viewController.RouteLineView = new MKPolylineView (_viewController.RouteLine);
						_viewController.RouteLineView.FillColor = UIColor.Red;
						_viewController.RouteLineView.StrokeColor = UIColor.Red;
						_viewController.RouteLineView.LineWidth = 2;
						//}
						overlayView = _viewController.RouteLineView;
					}
					return overlayView;
				} catch (Exception e) {
					MessageHelper.showErrorMesage ("getViewForOverlay " + e.Message);
					return null;
				}
			}
		}

		public override void ViewDidDisappear (bool animated)
		{
			_timer.Invalidate();
			_timer.Dispose();
			base.ViewDidDisappear (animated);
		}

		public override void ViewDidUnload ()
		{
			try {
				base.ViewDidUnload ();
				_locationMgr.StopUpdatingLocation ();
			} catch (Exception e) {
				MessageHelper.showErrorMesage ("vieDidUnload" + e.Message);
			}
		}

		//update a meeting
		public void updateMeeting()
		{
			//try {
			
			if (ServiceHelper.instance.currentMeeting == null) {
				ServiceHelper.instance.getMeetingByPhoneNumber (ServiceHelper.instance.localPhoneNumber, getMeeting);
			} else {
				doUpdate ();
			}
			//} catch (Exception e) {
			//	MessageHelper.showErrorMesage ("updateMEETING " + e.Message);
			//}
			
		}

		public void getMeeting (object sender, GetMeetingCompletedEventArgs e)
		{
			//try {
			if (e.Result.Meeting == null) {
				MessageHelper.showErrorMesage ("Could not find the current meeting!");
			} else {
				doUpdate ();
			}
			//} catch (Exception ex) {
			//	MessageHelper.showErrorMesage ("getMeeting " + ex.Message);
			//}
		}

		public void doUpdate ()
		{
			//try {
			var x = (float)_mapView.UserLocation.Coordinate.Latitude;
			//lat
			var y = (float)_mapView.UserLocation.Coordinate.Longitude;
			//long
			
			if (ServiceHelper.instance.currentMeeting != null) {
				ServiceHelper.instance.updateMeeting (ServiceHelper.instance.currentMeeting.MeetingID, ServiceHelper.instance.localuserID, x, y, meetingUpdate);
			}			
			
			//} catch (Exception e) {
			//	MessageHelper.showErrorMesage ("doUpdate " + e.Message);
			//}
		}

		public void meetingUpdate (Object sender, UpdateMeetingCompletedEventArgs e)
		{
			//try {				
			foreach (var userLocation in e.Result.UsersPositions.Where (userLocation => userLocation.UserId != ServiceHelper.instance.localuserID))
			{
				_trackCoordinate.Latitude = userLocation.xCoord;
				_trackCoordinate.Longitude = userLocation.yCoord;
			}
			
			InvokeOnMainThread (delegate()
			{
				updateTrackLocation();
			});			
		}		
	}
}

