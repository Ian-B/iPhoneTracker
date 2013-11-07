using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.AddressBook;
using System.ServiceModel;
using DataModel;
using DataModel.DataContracts;
using System.Linq;
using System.IO;
using System.Threading;

namespace app_iMeet
{	
	public class ServiceHelper
	{
		private ServiceHelper ()
		{
			dir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "../Documents");
			filePath = Path.Combine (dir, "number.txt");
		}
		//use documents directory
		private string dir;
		public readonly	string filePath;
		
		Semaphore sem = new System.Threading.Semaphore(1,20);
		
		public static ServiceHelper instance = new ServiceHelper();
		
		public string targetPhoneNumber{get; set;}
		public string localPhoneNumber{get;set;}		
		public Guid localuserID{get;set;}
		public Guid targetUserID{get; set;}		
		public Meeting currentMeeting{get;set;}		
		public string myPushToken{get;set;}
		
		#region delegate stuff
		
		Func<object,CreateMeetingCompletedEventArgs,Void> createdMeetingMethodToCall;
		Func<object,CheckNumbersCompletedEventArgs,Void> numbersCheckedMethodToCall;
		Func<object,GetMeetingCompletedEventArgs,Void> getMeetingMethodToCall;
		Func<object,LoginUserCompletedEventArgs,Void> loginMethodToCall;
		Func<object,UpdateMeetingCompletedEventArgs, Void> updateMeetingMethodToCall;
		
		private void callCreatedMeetingDelegate(object sender, CreateMeetingCompletedEventArgs e)
		{
			createdMeetingMethodToCall(sender,e);
		}
		
		private void callNumbersCheckedDelegate(object sender, CheckNumbersCompletedEventArgs e)
		{
			numbersCheckedMethodToCall(sender,e);
		}
		
		private void callgetMeetingDelegate(object sender, GetMeetingCompletedEventArgs e)
		{
			getMeetingMethodToCall(sender,e);
		}
		
		private void callLoginDelegate(object sender, LoginUserCompletedEventArgs e)
		{
			loginMethodToCall(sender,e);
		}
		
		private void callUpdateMeetingDelegate(object sender, UpdateMeetingCompletedEventArgs e)
		{
			updateMeetingMethodToCall(sender,e);
		}
		#endregion
		
		public void createMeeting (string targetNumber, string requestingNumber, Func<object,CreateMeetingCompletedEventArgs,Void> methodToCall)
		{
			createdMeetingMethodToCall = methodToCall;
			this.targetPhoneNumber = targetNumber;
			try {
				var client = new MeetingServiceClient (new BasicHttpBinding (), new EndpointAddress (EndpointHelper.EndpointAddress));
				
				
				var request = new CreateMeetingRequest { RequestingNumber = requestingNumber, TargetNumber = targetNumber };
				
				
				
				client.CreateMeetingCompleted += callCreatedMeetingDelegate;
				client.CreateMeetingAsync (request);
			} catch {
				MessageHelper.showErrorMesage ("createMeeting failed");
			}
		}
		
		void getMeetingData(object sender, CreateMeetingCompletedEventArgs e)
		{
			if(e.Result.CreatedMeeting != null)
			{
				this.currentMeeting = e.Result.CreatedMeeting;
				if(this.localuserID != Guid.Empty && e.Result.CreatedMeeting.Users.Count() > 0)
				{
					foreach(var user in e.Result.CreatedMeeting.Users)
					{
						if(user.userId != this.localuserID)
						{
							this.targetUserID = user.userId;
							this.targetPhoneNumber = user.phoneNumber;
						}
					}
				}
			}
			callCreatedMeetingDelegate(sender,e);
		}
		
		public void checkPhoneNumbers (List<string> numbersToCheck, Func<object,CheckNumbersCompletedEventArgs,Void> methodToCall)
		{
			numbersCheckedMethodToCall = methodToCall;
			try {
				var client = new MeetingServiceClient (new BasicHttpBinding (), new EndpointAddress (EndpointHelper.EndpointAddress));
				
				var request = new CheckNumbersRequest { PhoneNumbers = numbersToCheck.ToArray () };
				client.CheckNumbersCompleted += callNumbersCheckedDelegate;
				client.CheckNumbersAsync (request);
			} catch {
				MessageHelper.showErrorMesage ("checkPhoneNumbers failed");
			}
		}
		
		public void getMeetingByPhoneNumber(string PhoneNumber, Func<object,GetMeetingCompletedEventArgs,Void> functionToCall)
		{	
			try
			{
			getMeetingMethodToCall = functionToCall;
				
			var client = new MeetingServiceClient (new BasicHttpBinding (), 
			                                       new EndpointAddress (EndpointHelper.EndpointAddress));
			
			var request = new GetMeetingRequest
			{
				PhoneNumber = PhoneNumber
			};
			
			client.GetMeetingCompleted += getMeetingByPhoneData;
			client.GetMeetingAsync(request);	
			}
			catch
			{
				MessageHelper.showErrorMesage("GetMeetingID failed");
			}
		}
		
		void getMeetingByPhoneData(object sender, GetMeetingCompletedEventArgs e)
		{
			if (e.Result.Meeting != null)
			{
				if(this.localuserID != Guid.Empty && e.Result.Meeting.Users.Count() > 0)
				{
					foreach(var user in e.Result.Meeting.Users)
					{
						if(user.userId != this.localuserID)
						{
							this.targetUserID = user.userId;
							this.targetPhoneNumber = user.phoneNumber;
						}
					}
				}
			}
			if(e.Result.Meeting != null)
			{
				this.currentMeeting = e.Result.Meeting;
			}
			callgetMeetingDelegate(sender,e);
		}
		
		public void loginUser (string phoneNumber, Func<object,LoginUserCompletedEventArgs,Void> functionToCall)
		{			
			
			loginMethodToCall = functionToCall;
			
			string lastDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceToken");
					
			if (lastDeviceToken == null)
			{
				lastDeviceToken = string.Empty;
			}
			
			var client = new MeetingServiceClient (new BasicHttpBinding (), 
			                                       new EndpointAddress (EndpointHelper.EndpointAddress));
			
			var request = new LoginRequest()
			{
				DeviceToken = lastDeviceToken,//UIDevice.CurrentDevice.UniqueIdentifier.ToString(),
				PhoneNumber = phoneNumber
			};
			
			/*
			using (var alert = new UIAlertView ("Device Token: ", lastDeviceToken, null, "OK", null)) {
					alert.Show ();
				}
			*/
			
			client.LoginUserCompleted += storeLoginData;
			client.LoginUserAsync (request);		
		}
		
		void storeLoginData(object sender, LoginUserCompletedEventArgs e)
		{
			if (e.Result.LoggedInUser != null)
			{
				this.localuserID = e.Result.LoggedInUser.userId;
				this.localPhoneNumber = e.Result.LoggedInUser.phoneNumber;
				this.myPushToken = e.Result.LoggedInUser.deviceID;	
			}
			callLoginDelegate(sender,e);
		}
		
		public void updateMeeting(Guid meetingID, Guid requestingUser, float myX, float myY, Func<Object, UpdateMeetingCompletedEventArgs, Void> functionToCall)
		{
			updateMeetingMethodToCall = functionToCall;
			
			var client = new MeetingServiceClient (new BasicHttpBinding (), new EndpointAddress (EndpointHelper.EndpointAddress));
					
			var request = new UpdateMeetingRequest
			{
				MeetingId = meetingID,
				MyX = myX,
				MyY = myY,
				RequestingUserID = requestingUser
			};

			sem.WaitOne(100);

			client.UpdateMeetingCompleted += storeMeeting;
			client.UpdateMeetingAsync (request);

		}
		
		public void storeMeeting(object sender, UpdateMeetingCompletedEventArgs e)
		{
			callUpdateMeetingDelegate(sender,e);			
		}
			
		
		public void releaseSemaphore()
		{
			sem.Release();
		}
		
		public string readUserPhoneNumber()
		{
			try
			{
				return File.ReadAllText(filePath).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}


	