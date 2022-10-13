using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

namespace TwilioCaller
{
    public static class Call
    {
        private const string GermanNumberPrefix = "+49";

        [FunctionName("call")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            
            string name = req.Query["name"];
            name ??= data?.name;

            string number = req.Query["number"];
            number ??= data?.number;

            if (IsInvalidPhoneNumber(number))
            {
                var warning = $"The phone number '{number}' is too short. Please provide a mobile phone number with at least 9 digits.";
                log.LogWarning(warning);
                return new BadRequestObjectResult(warning);
            }
            if (IsGermanMobileNumberWithoutLanguagePrefix(number))
            {
                number = string.Concat(GermanNumberPrefix, number.AsSpan(1));
            }

            //TODO inject twilio client and configuration in order to extend unit tests past this line
			TwilioClient.Init(GetEnvironmentVariable("TWILIO_ACCOUNT_SID"), GetEnvironmentVariable("TWILIO_ACCOUNT_TOKEN"));
			
            var to = new PhoneNumber(number);
			var from = new PhoneNumber(GetEnvironmentVariable("TWILIO_FROM_NUMBER"));
            var message = GetMessage(name);
            await CallResource.CreateAsync(to, from, twiml: message);

            return new OkObjectResult("A Twilio phone call was successfully triggered");
        }

        private static Twiml GetMessage(string name)
        {
            var voice = GetEnvironmentVariable("TWILIO_VOICE");
            var text = string.IsNullOrEmpty(name) 
                ? GetEnvironmentVariable("TEXT_NEUTRAL")
                : string.Format(GetEnvironmentVariable("TEXT_PERSONAL"), name);
            return new Twiml($"<Response><Say voice=\"{voice}\" language=\"de-DE\">{text}</Say></Response>");
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
        private static bool IsInvalidPhoneNumber(string number)
        {
            return string.IsNullOrEmpty(number) || number.Length < 9;
        }

        private static bool IsGermanMobileNumberWithoutLanguagePrefix(string number)
        {
            return number.StartsWith("01");
        }
    }
}
