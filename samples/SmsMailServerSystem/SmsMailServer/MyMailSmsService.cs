using System;
using MDS.Client.MDSServices;

namespace SmsMailServer
{
    [MDSService(Description = "This service is a sample mail/sms service.", Version = "1.0.0.0")]
    public class MyMailSmsService : MDSService
    {
        //All parameters and return values can be defined.
        [MDSServiceMethod(Description = "This method is used send an SMS.")]
        public void SendSms(
            [MDSServiceMethodParameter("Phone number to send SMS.")] string phone,
            [MDSServiceMethodParameter("SMS text to be sent.")]string smsText)
        {
            //Process SMS
            Console.WriteLine("Sending SMS to phone: " + phone);
            Console.WriteLine("Sms Text: " + smsText);

            //Acknowledge the message
            IncomingMessage.Acknowledge();
        }

        //You do not have to define any parameters
        [MDSServiceMethod]
        public void SendEmail(string emailAddress, string header, string body)
        {
            //Process email
            Console.WriteLine("Sending an email to " + emailAddress);
            Console.WriteLine("Header: " + header);
            Console.WriteLine("Body  : " + body);

            //Acknowledge the message
            IncomingMessage.Acknowledge();
        }

        // A simple method just to show return values.
        [MDSServiceMethod]
        [return: MDSServiceMethodParameter("True, if phone number is valid.")]
        public bool IsValidPhone([MDSServiceMethodParameter("Phone number to send SMS.")] string phone)
        {
            //Acknowledge the message
            IncomingMessage.Acknowledge();

            //Return result
            return (phone.Length == 10);
        }
    }
}