using System;
using System.Threading;
using System.Windows.Forms;

// Add DLL Reference @ C:\Program Files (x86)\MBTrading\MBT Desktop Pro\MbtCom.DLL
using MBTCOMLib;
using MBTQUOTELib;
using MBTORDERSLib;

namespace MBT_QUOTES { 

	class AppMain {
		static void Main () { 
			var MBT = new MBT { User="USERNAME", Pass="PASSWORD" };
			MBT.Connect();

			Console.WriteLine("\nHit enter to begin quotes. Hit escape to exit application.\n");
			WaitForKey(ConsoleKey.Enter);

			foreach (MbtAccount a in MBT.Orders.Accounts) { Console.WriteLine("Loading Account: " + a.Account); a.Load(); }
			Console.WriteLine("");
			Thread.Sleep(3333);
			foreach (MbtAccount a in MBT.Orders.Accounts) { Console.WriteLine(a.Account + " " + a.IsDemoAccount()); }
			Console.WriteLine("");

			MBT.Advise("EUR/USD");
			Console.WriteLine("");

			WaitForKey(ConsoleKey.Escape);
		}

		static void WaitForKey (ConsoleKey key) {
			while (true) { 
				Application.DoEvents();
				Thread.Sleep(9);
				if (Console.KeyAvailable) { if (Console.ReadKey(true).Key==key) { break; } }
			}
		}
	}

	class MBT {
		public MbtComMgr Client;
		public MbtQuotes Quotes;
		public MbtOrderClient Orders;
		
		QuoteNotifier QN;

		internal String User;
		internal String Pass;

		internal MBT () {
			Client = new MbtComMgr();
			Quotes = Client.Quotes;
			Orders = Client.OrderClient;

			Client.EnableSplash(false);
			Client.SilentMode = false;

			QN = new QuoteNotifier();
		}

		internal void Connect () {
			Console.Write("Connecting to MBT...");
			Client.DoLogin(9,User,Pass,"");
			while (!Client.IsConnected) { Thread.Sleep(9); }
			Quotes.Connect();
			while (Quotes.ConnectionState!=enumConnectionState.csLoggedIn) { Thread.Sleep(9); }
			Console.WriteLine("Connected!");
		}

		internal void Advise (String symbol) { 
			Console.WriteLine("Subscribing To Quotes: "+symbol);
			Quotes.AdviseSymbol(QN,symbol,(int)enumQuoteServiceFlags.qsfLevelOne); 
		}
	}

	class QuoteNotifier : IMbtQuotesNotify {
		void IMbtQuotesNotify.OnQuoteData (ref QUOTERECORD data) { 
			var bid = data.dBid.ToString();
			var ask = data.dAsk.ToString();
			var msg = bid.PadLeft(9,' ') + " / " + ask.PadLeft(9,' ');
			Console.WriteLine(msg); 
		}

		void IMbtQuotesNotify.OnLevel2Data  (ref LEVEL2RECORD data)  { }
		void IMbtQuotesNotify.OnOptionsData (ref OPTIONSRECORD data) { }
		void IMbtQuotesNotify.OnTSData      (ref TSRECORD data)      { }
	}

}
