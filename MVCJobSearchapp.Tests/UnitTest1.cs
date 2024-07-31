using NUnit.Framework;
using MVCJobSearchApp.Controllers; 


namespace MVCJobSearchApp.Tests
{

        [TestFixture]
        public class LoginTests
        {
            [Test]
            public void HandleUserInput_ValidInput_ReturnsInput()
            {
                string input = "valid";
                Console.SetIn(new StringReader(input));
                
                string result = UserController.HandleUserInput("Prompt: ");
                
                Assert.AreEqual(input, result);
            }

             [Test]
            public void HandleUserInput_InvalidInput_RetriesUntilValid()
            {
                var inputs = new[] { "invalid!", "validInput", "duc1234" }; 
                var stringReader = new StringReader(string.Join(Environment.NewLine, inputs));
                Console.SetIn(stringReader);

                string result = UserController.HandleUserInput("Prompt: ");

                Assert.AreEqual("validInput", result); 
            }
            [Test]
            public void AuthenticateUser_ValidCredentials_ReturnsUser()
            {

                string username = "admin1";
                string password = "password1";
                int expectedUserId = 1; 
                string expectedRole = "admin";

                var result = UserController.AuthenticateUser(username, password);

                Assert.AreEqual(expectedUserId, result.Value.userId);
                Assert.AreEqual(expectedRole, result.Value.role);
            }

        }
}
