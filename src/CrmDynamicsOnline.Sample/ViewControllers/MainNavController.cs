using System;
using MonoTouch.UIKit;
using OData;
using MonoTouch.Foundation;

namespace CrmDynamicsOnline.Sample
{
	public class MainNavController : UINavigationController
	{
		LoginViewController login;
		FeedsController feeds;
		
		public MainNavController ()
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			login = new LoginViewController();
			login.RetreivedResultAction = Handle_RetreivedResultAction;
			PushViewController(login, true);
			
		}

		void Handle_RetreivedResultAction(bool result)
		{
			if(result)
			{
				feeds = new FeedsController();
				using(var pool = new NSAutoreleasePool())
				{
					pool.BeginInvokeOnMainThread(()=>{
						PushViewController(feeds, true);
					});
				}
				
				
			}
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
		}
	}
}

