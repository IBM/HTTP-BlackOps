using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;

namespace TrafficViewerUnitTest
{
	/// <summary>
	/// Summary description for Generic
	/// </summary>
	[TestClass]
	public class Generic
	{
		public Generic()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void CheckScriptDebugging()
		{
			if (Utils.IsScriptDebuggingEnabled())
			{
				//check disabling and enabling back
				Utils.DisableScriptDebugging();

				//now is script debugging enabled should return false
				Assert.IsFalse(Utils.IsScriptDebuggingEnabled());

				Utils.ModRegistryKey("HKEY_CURRENT_USER\\Software\\Microsoft\\Internet Explorer\\Main", "Disable Script Debugger", "no");
			
				Assert.IsTrue(Utils.IsScriptDebuggingEnabled());
			}
			else
			{ 
				//check enabling it and disabling it
				Utils.ModRegistryKey("HKEY_CURRENT_USER\\Software\\Microsoft\\Internet Explorer\\Main", "Disable Script Debugger", "no");

				Assert.IsTrue(Utils.IsScriptDebuggingEnabled());

				Utils.DisableScriptDebugging();

				//now is script debugging enabled should return false
				Assert.IsFalse(Utils.IsScriptDebuggingEnabled());
			}
		}

		[TestMethod]
		public void CheckEncryptDecryptString()
		{
			string text = "Demo1234";
			string encryptedText = Encryptor.EncryptToString(text);
			Assert.IsNotNull(encryptedText);
			string decryptedText = Encryptor.DecryptToString(encryptedText);
			Assert.IsNotNull(decryptedText);
			Assert.AreNotEqual(text, encryptedText);
			Assert.AreEqual(text, decryptedText);

		}


        [TestMethod]
        public void CheckEncryptDecryptLoremIpsum()
        {
            string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            string encryptedText = Encryptor.EncryptToString(text);
            Assert.IsNotNull(encryptedText);
            string decryptedText = Encryptor.DecryptToString(encryptedText);
            Assert.IsNotNull(decryptedText);
            Assert.AreNotEqual(text, encryptedText);
            Assert.AreEqual(text, decryptedText);

        }

	

	
	}
}
