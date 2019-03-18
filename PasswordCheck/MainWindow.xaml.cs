using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace PasswordCheck
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string HashedPassword;
		private Stopwatch stopWatch = new Stopwatch();

		public MainWindow()
		{
			InitializeComponent();

			// Start the stop watch
			stopWatch.Start();

			textPassword.KeyDown += new KeyEventHandler(TextPassword_keyDown);
			passBoxPassword.KeyDown += new KeyEventHandler(PassBoxPassword_keyDown);
		}



		// Hides the textbox input box and makes the password input box visible
		// when unchecked
		private void PasswordMask_Checked(object sender, RoutedEventArgs e)
		{
			textPassword.Visibility = Visibility.Hidden;
			passBoxPassword.Visibility = Visibility.Visible;

			textPassword.Width = 0;
			textPassword.MinWidth = 0;

			passBoxPassword.Password = textPassword.Text;
		}

		// Hides the password input box and makes the textbox visible
		// when unchecked
		private void PasswordMask_Unchecked(object sender, RoutedEventArgs e)
		{
			textPassword.Visibility = Visibility.Visible;
			passBoxPassword.Visibility = Visibility.Hidden;

			textPassword.Width = 280;
			textPassword.MinWidth = 280;

			textPassword.Text = passBoxPassword.Password;
		}




		// Updates the password string with a SHA1 hash of the user input
		private void TextPassword_Changed(object sender, TextChangedEventArgs e)
		{
			HashedPassword = SHA1HASH(textPassword.Text);
			textResult.Text = "";
		}

		private void PassBoxPassword_Changed(object sender, RoutedEventArgs e)
		{
			HashedPassword = SHA1HASH(passBoxPassword.Password);
			textResult.Text = "";			
		}



		// Checks to see if the SHA1 hash of the password is in haveibeenpwned's archive
		private void Check_password(object sender, RoutedEventArgs e)
		{
			if (HashedPassword != null)
			{

				// Rate limit of haveibeenpwned is one request every 1500ms
				// Instead of getting a 429 error, we might as well just wait.
				while (stopWatch.ElapsedMilliseconds < 1500)
				{
					textResult.Text = "Waiting...";
					Thread.Sleep(100);
				}

				
				// Retrieving data from haveibeenpwned
				textResult.Text = "Retrieving data from haveibeenpwned";
				string res = Get(HashedPassword.Substring(0, 5));

				stopWatch.Restart();

				if (res != null)
				{
					textResult.Text = "Parsing data";

					// Section of the hash not sent.
					string pTail = HashedPassword.Substring(5, HashedPassword.Length - 5);

					string[] rSplit;
					bool found = false;

					// Look at each line returned individually
					foreach (string r in res.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
					{
						// Split the hash from the number of occurances
						rSplit = r.Split(':');

						// Does the hash returned match the hashed password?
						if (pTail.Equals(rSplit[0])) // Yes
						{
							textResult.Text = "Your password was found " + rSplit[1].ToString() + " times in Haveibeenpwned's archive." +
								"\n\nThere is a very good chance many bad people have your password; you should change it if you're still using it!";
							found = true;
							break;
						}
					}

					if (!found)
					{
						textResult.Text = "Your password wan't found in Haveibeenpwned's archive. Nice!" +
							"\n\nThis just means it's less likely a bad person has your password; although they still might!";
					}
				}
				else
				{
					textResult.Text = "Connection error:" +
						"\n- Ensure your computer is connected to the internet" +
						"\n- Haveibeenpwned's server might be down" +
						"\n\nTry again later once resolved";
				}

			} else
			{
				textResult.Text = "Please enter a password before clicking the Check button";
			}
		}



		// GET request to haveibeenpwned.com's API
		// First 5 characters of the SHA1 hash is sent
		// Returns all hashes that starts with the 5 characters sent and the number of occurrences
		// Return hashes exclude the characters sent

		// Eg. Password1
		// 70CCD9007338D6D81DD3B6271621B9CF9A97EA00
		// Sent = 70CCD
		// Received = 9007338D6D81DD3B6271621B9CF9A97EA00:111658
		// 111658 is the number of occurrences
		public string Get(string uri)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.pwnedpasswords.com/range/" + uri);
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			try
			{
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Stream stream = response.GetResponseStream();
				StreamReader reader = new StreamReader(stream);
				string returnData = reader.ReadToEnd();
				((IDisposable)response).Dispose();
				((IDisposable)stream).Dispose();
				((IDisposable)reader).Dispose();
				return returnData;

			} catch (WebException e)
			{
				//Console.WriteLine("Exception: " + e);
				return null;
			}
		}



		// Convert plain text string to a SHA1 string
		public static string SHA1HASH(string s)
		{
			byte[] passBytes = BytesFromUTF8String(s);

			var sha1 = SHA1.Create();
			byte[] hashedBytes = sha1.ComputeHash(passBytes);

			return HexStringFromBytes(hashedBytes).ToUpper();
		}


		
		// Convert UTF8 string to bytes
		public static byte[] BytesFromUTF8String(string s)
		{
			byte[] b = Encoding.UTF8.GetBytes(s);

			return b;
		}



		// Convert bytes to a hex string
		public static string HexStringFromBytes(byte[] bs)
		{
			var sb = new StringBuilder();
			foreach (byte b in bs)
			{
				var hex = b.ToString("x2");
				sb.Append(hex);
			}
			return sb.ToString();
		}



		// Call the check_password method when the user presses the enter/return key
		// whilst the input password text box is in focus
		private void TextPassword_keyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Check_password(sender, e);
			}
		}



		// Call the check_password method when the user presses the enter/return key
		// whilst the input passwordbox text box is in focus
		private void PassBoxPassword_keyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Check_password(sender, e);
			}
		}
	}
}
