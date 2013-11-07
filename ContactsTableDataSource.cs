using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.AddressBook;
using System.Collections.Generic;

namespace app_iMeet
{
	public class ContactsTableDataSource : UITableViewDataSource
	{
		private List<ABPerson> _contacts;
		
		private string _cellId;
		
		//private ContactsViewController _parentController;
		
		public ContactsTableDataSource (List<ABPerson> contacts)
		{
			_cellId = "Cell";
			
			_contacts = contacts;
		}
		
		public override int RowsInSection(UITableView tableView, int section)
		{
			return _contacts.Count;
		}	
		
		public override int NumberOfSections(UITableView tableView)
		{
			return 1;
		}
		
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			string cellId = _cellId;			
			
			UITableViewCell cell = tableView.DequeueReusableCell(cellId);
			
			if (cell == null)
			{
				cell = new UITableViewCell(UITableViewCellStyle.Default, cellId);
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}
			
			// get contacts
			cell.TextLabel.Text = _contacts[indexPath.Row].FirstName + " " + _contacts[indexPath.Row].LastName;

			return cell;
		}
	}
}

