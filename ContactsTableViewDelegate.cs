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

namespace app_iMeet
{	
	public class ContactsTableViewDelegate : UITableViewDelegate
	{
		private ContactsViewController _controller;
		private List<ABPerson> _contacts;		
		LoadingHUDView _loading;
		
		public ContactsTableViewDelegate (ContactsViewController controller, List<ABPerson> contacts)
		{
			_controller = controller;
			_contacts = contacts;
			_loading = new LoadingHUDView("Loading", "Trying to create Meeting...");
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			_controller.View.AddSubview(_loading);
			_loading.StartAnimating();
			
			ABPerson person = _contacts[indexPath.Row];
			
			//create meeting
			createMeeting(person);
		}
		
		//create a meeting
		public void createMeeting (ABPerson target)
		{	
			if(target == null)
			{
				MessageHelper.showErrorMesage("No abPerson target createMeeting top");
			}
			else				
			if (target.GetPhones().Count > 1)
			{				
				List<string> trackerNumbers = new List<string>();
				
				foreach(var number in target.GetPhones())
				{
					trackerNumbers.Add(number.Value);				
				}				
				
				//ServiceHelper.instance.checkPhoneNumbers(trackerNumbers,delegate(Object sender, CheckNumbersCompletedEventArgs e){this.numbersChecked(sender,e);}); //get _trackNumber
			}
			else if(target.GetPhones().Count == 1)
			{		
				var targetPhones = target.GetPhones();
				var phoneArray = targetPhones.GetValues();
				
				var targetNumber = MessageHelper.cleanString(phoneArray[0]);
				
				ServiceHelper.instance.createMeeting(targetNumber,ServiceHelper.instance.localPhoneNumber,meetingRequested);
			}
			else
			{
				MessageHelper.showCouldNotCreateMeeting();
			}
				
		}		 
		
		public void meetingRequested (Object sender, CreateMeetingCompletedEventArgs e)
		{
			try
			{
			ServiceHelper.instance.currentMeeting = e.Result.CreatedMeeting;
			
			//stop animating 
			_loading.StopAnimating();
			_loading.RemoveFromSuperview();
			
			InvokeOnMainThread (delegate() { _controller.NavigationController.PushViewController (new MapViewController(), true); });
			}
			catch
			{				
				MessageHelper.showErrorMesage("getMeetingRequest failed");;
			}
		}

		// get correct number from database
		public void numbersChecked (Object sender, CheckNumbersCompletedEventArgs e)
		{
			try
			{
				if (e.Result.ValidNumbers != null)
				{
					ABAddressBook addressBook = new ABAddressBook ();
					ABPerson[] allContacts = addressBook.GetPeople ();
					
					foreach (var contact in allContacts) {
						var phone = contact.GetPhones ();
						foreach (var number in phone) 
						{
							if (e.Result.ValidNumbers.Contains(number.Value.Replace(" ", string.Empty))) 
							{
								ServiceHelper.instance.targetPhoneNumber = number.Value.Replace(" ", string.Empty);
							}
						}
					}
				ServiceHelper.instance.createMeeting(ServiceHelper.instance.targetPhoneNumber,ServiceHelper.instance.localPhoneNumber,meetingRequested);
				}
				else
				{
					MessageHelper.showCouldNotCreateMeeting();
				}
			}
			catch
				{
					MessageHelper.showErrorMesage("numbersChecked failed");
				}
		}	
	}
}

