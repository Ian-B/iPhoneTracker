using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AddressBook;
using MonoTouch.AddressBookUI;
using DataModel;
using DataModel.DataContracts;
using System.ServiceModel;

namespace app_iMeet
{
	public class ContactsViewController : UIViewController//UITableViewController
	{
		UIBarButtonItem _updateButton;		
		UITableView _tableView;

		private List<ABPerson> _contacts;
		
		List<string> _numbers; //address book numbers
		
		LoadingHUDView _loading;

		public ContactsViewController () : base()
		{
			_contacts = new List<ABPerson> ();
			
			_numbers = new List<string>();
			
			_loading = new LoadingHUDView("Loading", "Updating Contacts List...");
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();			
			
			_updateButton = new UIBarButtonItem ();
			_updateButton.Title = "Update";
			
			UIBarButtonItem checkButton = new UIBarButtonItem();
			checkButton.Title = "Check";
			
			this.Title = "Meet With";
			this.NavigationItem.LeftBarButtonItem = _updateButton;
			this.NavigationItem.RightBarButtonItem = checkButton;
			
			/* -----------------------------------------------
			 * Need to Sort out which Contacts have iMeet here,
			 * then gets passed to dataSource to display
			 */
			_tableView = new UITableView(new RectangleF(0,0,this.View.Frame.Width, this.View.Frame.Height), UITableViewStyle.Plain);
			this.Add(_tableView);
			_tableView.DataSource = new ContactsTableDataSource (_contacts);
			_tableView.Delegate = new ContactsTableViewDelegate (this, _contacts);
			
			// use phone number
			ABAddressBook addressBook = new ABAddressBook ();
			ABPerson[] allContacts = addressBook.GetPeople ();
			
			//keeps crashing around here?
			foreach(var contact in allContacts.Where(contact => contact.GetPhones().Where(phone => (phone.Label == ABPersonPhoneLabel.Mobile 
			                                                                                        || phone.Label == ABPersonPhoneLabel.iPhone
			                                                                                        || phone.Label == ABPersonPhoneLabel.Main)).Count() > 0))
			{
				_numbers.Add(contact.GetPhones().Select(z => z.Value.Replace(" ", string.Empty).Trim().ToString()).FirstOrDefault());
			}			
			
			this.View.AddSubview(_loading);
			_loading.StartAnimating(); //start animating
			checkPhoneNumbers(_numbers);
			
			_updateButton.Clicked += HandleUpdateButtonClicked;
			checkButton.Clicked += HandleCheckButtonClicked;
		}
		
		//find meetings
		void HandleCheckButtonClicked (object sender, EventArgs e)
		{
			ServiceHelper.instance.getMeetingByPhoneNumber(ServiceHelper.instance.localPhoneNumber,HandleNewMeetings);
		}
		
		void HandleNewMeetings(Object sender, GetMeetingCompletedEventArgs e)
		{
			string name = string.Empty;
			if (e.Result.Meeting != null)
			{
			foreach (var user in e.Result.Meeting.Users.Where(user => user.phoneNumber != ServiceHelper.instance.localPhoneNumber))
			{	
				ServiceHelper.instance.targetPhoneNumber = user.phoneNumber;
				ServiceHelper.instance.targetUserID = user.userId;
				name = user.phoneNumber;
			}			
			ServiceHelper.instance.currentMeeting = e.Result.Meeting;
			
				InvokeOnMainThread (delegate() {
					this.NavigationController.PresentModalViewController(new ShowViewController(name, this.NavigationController), true);					
				});				
			}
			else
			{
				InvokeOnMainThread (delegate() {
					using (UIAlertView alert = new UIAlertView ("iMeet", "No Meetings Found", null, "OK", null)) 
					{				
						alert.Show ();
					}
				});	
			}
		}	
		
		void HandleUpdateButtonClicked (object sender, EventArgs e)
		{
			this.View.AddSubview(_loading);
			_loading.StartAnimating();
			checkPhoneNumbers(_numbers);
		}	
		
		//update contacts
		public void checkPhoneNumbers (List<string> numbersToCheck)
		{
			ServiceHelper.instance.checkPhoneNumbers(numbersToCheck,numbersChecked);				
		}
		
		public void numbersChecked (Object sender, CheckNumbersCompletedEventArgs e)
		{
			if (e.Result.ValidNumbers != null)
			{
				
				InvokeOnMainThread (delegate() {
					//address book stuff
					ABAddressBook addressBook = new ABAddressBook ();
					ABPerson[] allContacts = addressBook.GetPeople ();
					
					_contacts.Clear(); //remove old 
					
					foreach (var contact in allContacts) {
						var phone = contact.GetPhones ();
						foreach (var number in phone) 
						{
							if (e.Result.ValidNumbers.Contains(number.Value.Replace(" ", string.Empty).ToString())) 
							{
								_contacts.Add (contact);
							}
						}
					}
					//-----------------------------------------------
					
					//TableView.DataSource = new ContactsTableDataSource (this, _contacts);
					//TableView.Delegate = new ContactsTableViewDelegate (this, _contacts);				
											
					_loading.StopAnimating(); //stop animating
					_loading.RemoveFromSuperview();
					_tableView.ReloadData();
				});
			}
		}		
	}
}

