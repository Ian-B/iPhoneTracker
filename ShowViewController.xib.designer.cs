// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace app_iMeet {
	
	
	// Base type probably should be MonoTouch.UIKit.UIViewController or subclass
	[MonoTouch.Foundation.Register("ShowViewController")]
	public partial class ShowViewController {
		
		private MonoTouch.UIKit.UIView __mt_view;
		
		private MonoTouch.UIKit.UILabel __mt_phoneLbl;
		
		private MonoTouch.UIKit.UIButton __mt_noBtn;
		
		private MonoTouch.UIKit.UIButton __mt_okBtn;
		
		#pragma warning disable 0169
		[MonoTouch.Foundation.Connect("view")]
		private MonoTouch.UIKit.UIView view {
			get {
				this.__mt_view = ((MonoTouch.UIKit.UIView)(this.GetNativeField("view")));
				return this.__mt_view;
			}
			set {
				this.__mt_view = value;
				this.SetNativeField("view", value);
			}
		}
		
		[MonoTouch.Foundation.Connect("phoneLbl")]
		private MonoTouch.UIKit.UILabel phoneLbl {
			get {
				this.__mt_phoneLbl = ((MonoTouch.UIKit.UILabel)(this.GetNativeField("phoneLbl")));
				return this.__mt_phoneLbl;
			}
			set {
				this.__mt_phoneLbl = value;
				this.SetNativeField("phoneLbl", value);
			}
		}
		
		[MonoTouch.Foundation.Connect("noBtn")]
		private MonoTouch.UIKit.UIButton noBtn {
			get {
				this.__mt_noBtn = ((MonoTouch.UIKit.UIButton)(this.GetNativeField("noBtn")));
				return this.__mt_noBtn;
			}
			set {
				this.__mt_noBtn = value;
				this.SetNativeField("noBtn", value);
			}
		}
		
		[MonoTouch.Foundation.Connect("okBtn")]
		private MonoTouch.UIKit.UIButton okBtn {
			get {
				this.__mt_okBtn = ((MonoTouch.UIKit.UIButton)(this.GetNativeField("okBtn")));
				return this.__mt_okBtn;
			}
			set {
				this.__mt_okBtn = value;
				this.SetNativeField("okBtn", value);
			}
		}
	}
}