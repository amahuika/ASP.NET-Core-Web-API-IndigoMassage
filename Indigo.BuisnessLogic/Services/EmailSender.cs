using Indigo.BuisnessLogic.Utilities;
using Indigo.Models.DTO;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;


namespace Indigo.BuisnessLogic.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;




        public EmailSender(IConfiguration configuration)
        {

            _configuration = configuration;

        }



        public void SendBookingEmail(BookingResponseDTO bookingResponse)
        {

            var emailSettings = _configuration.GetSection("EmailSettings");


            // create new message object
            var message = new MimeMessage();

            // set the sender and recipient email address
            message.From.Add(new MailboxAddress("Indigo Massage", emailSettings["Email"]));

            message.To.Add(new MailboxAddress(bookingResponse.FirstName + " " + bookingResponse.LastName, bookingResponse.Email));

            // set the subject and body
            message.Subject = $"Indigo Massage Booking {bookingResponse.Status}";



            var bodyBuilder = new BodyBuilder();


            bodyBuilder.HtmlBody = getClientEmailBody(bookingResponse); ;
            message.Body = bodyBuilder.ToMessageBody();







            // send the message
            using (var client = new SmtpClient())
            {
                // connect to the server and send the message

                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                client.Authenticate(emailSettings["Email"], emailSettings["EmailPassword"]);

                client.Send(message);

                client.Disconnect(true);
            }






        }

        public void SendBookingEmailToAdmin(BookingResponseDTO bookingResponse)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            // create new message object
            var message = new MimeMessage();

            // set the sender and recipient email address
            message.From.Add(new MailboxAddress("Indigo Massage Bookings", emailSettings["Email"]));

            message.To.Add(new MailboxAddress("Indigo Massage", "{Admin Email Here}"));

            // set the subject and body

            message.Subject = $"Indigo Massage New Booking";

            var bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = getAdminEmailBody(bookingResponse);

            message.Body = bodyBuilder.ToMessageBody();

            // send the message

            using (var client = new SmtpClient())
            {
                // connect to the server and send the message
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate(emailSettings["Email"], emailSettings["EmailPassword"]);

                client.Send(message);

                client.Disconnect(true);
            }



        }







        private string getClientEmailBody(BookingResponseDTO bookingResponse)
        {
            string pending = bookingResponse.Status == StaticDetails.SD_BookingStatus_Pending ?
               "We will contact you shortly via email to confirm your booking.\n\n" :
               "We look forward to seeing you soon!";

            string address = "{Buisness Address} <a href='{Your address link here}' target='_blank' rel='noopener noreferrer'>see map</a>";


            string intro = bookingResponse.Status == StaticDetails.SD_BookingStatus_Pending ?
                $"Booking {bookingResponse.Status}" :

                $"Booking {bookingResponse.Status}!";

            string body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{
                        margin: 0;
                        padding: 0;
                        font-family: Arial, sans-serif;
                        font-size: 14px;
                        line-height: 1.6;
                        color: #333333;
                    }}
        
                    .header {{
                        background-color: #5C6BC0;
                        padding: 20px;
                        text-align: center;
                        color: #FFFFFF;
                    }}
        
                    .content {{
                        padding: 20px;
                    }}
        
                    .footer {{
                        background-color: #333333;
                        padding: 20px;
                        text-align: center;
                        color: #FFFFFF;
                    }}
                </style>
            </head>
            <body>
                <div class=""header"">
                    <h1>Indigo Massage</h1>
                </div>
                <div class=""content"">
                <p>Hi {bookingResponse.FirstName} {bookingResponse.LastName},</p>

                    <h2>{intro}</h2>
                    <p>{pending}</p>
                    <p>Below are your booking details:</p>
                    
         <table>
                          <tbody>
                            <tr>
        <td><strong>Service:</strong> </td>
                              <td >
                              {bookingResponse.serviceName}
                              </td>
                            </tr>
                            <tr>
                              <td> <strong>Date: </strong></td>
                              <td>
        {bookingResponse.Date}</td>
                            </tr>
                            <tr>
                              <td> <strong>Time:</strong></td>
                              <td>{bookingResponse.Time} ( {bookingResponse.Duration} minutes )</td>
                            </tr>
                            <tr>
                              <td><strong>Price:</strong></td>
                              <td>${bookingResponse.Cost}</td>
                            </tr>
                             <tr>
                                <td><strong>Address:</strong></td>
                                <td >{address}</td>
                             </tr>
             
          
                          </tbody>
                        </table>
                    <p>Thank you,</p>
                    <p>Indigo Massage</p>
                        <hr/>
                        <p style='font-size: 20px;'>Cancelations</p>
                        <p>If you wish to cancel this appointment please call us on 123 456 789 </p>
                        <hr/>
                        <p style='font-size: 20px;'>Do not reply to  this email.</p>
                        <hr/>


                </div>
                <div class=""footer"">
                    <p>Get in touch</h5>
                    <p>Phone: 123 456 789</p>
                    <p>Email: hello@mail.com</p>
                </div>
            </body>
            </html>

";






            return body;
        }

        private string getAdminEmailBody(BookingResponseDTO bookingResponse)
        {

            string body = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                body {{
                                    margin: 0;
                                    padding: 0;
                                    font-family: Arial, sans-serif;
                                    font-size: 14px;
                                    line-height: 1.6;
                                    color: #333333;
                                }}
        
                                .header {{
                                    background-color: #5C6BC0;
                                    padding: 20px;
                                    text-align: center;
                                    color: #FFFFFF;
                                }}
        
                                .header h1 {{
                                    margin: 0;
                                }}
        
                                .content {{
                                    padding: 20px;
                                }}
        
                                .booking-details {{
                                    margin-bottom: 20px;
                                }}
        
                                .client-details {{
                                    margin-bottom: 20px;
                                }}
        
                                .button {{
                                    display: inline-block;
                                    background-color: #333333;
                                    color: #FFFFFF;
                                    padding: 10px 20px;
                                    text-decoration: none;

                                }}
                            </style>
                        </head>
                        <body>
                            <div class=""header"">
                                <h1>New Booking</h1>
                            </div>
                            <div class=""content"">
                             <table class=""booking-details"">
            <tr>
                <th colspan=""2"">Booking Details</th>
            </tr>
            <tr>
                <td><strong>Service Name:</strong></td>
                <td>{bookingResponse.serviceName}</td>
            </tr>
            <tr>
                <td><strong>Date:</strong></td>
                <td>{bookingResponse.Date}</td>
            </tr>
            <tr>
                <td><strong>Time:</strong></td>
                <td>{bookingResponse.Time}</td>
            </tr>
            <tr>
                <td><strong>Length:</strong></td>
                <td>{bookingResponse.Duration} min</td>
            </tr>
            <tr>
                <td><strong>Price:</strong></td>
                <td>${bookingResponse.Cost}</td>
            </tr>
          
        </table>
        <table class=""client-details"">
            <tr>
                <th colspan=""2"">Client Details</th>
            </tr>
            <tr>
                <td><strong>Name:</strong></td>
                <td>{bookingResponse.FirstName} {bookingResponse.LastName}</td>
            </tr>
            <tr>
                <td><strong>Email:</strong></td>
                <td>{bookingResponse.Email}</td>
            </tr>
            <tr>
                <td><strong>Phone:</strong></td>
                <td>{bookingResponse.PhoneNumber}</td>
            </tr>
<tr>
                <td><strong>Notes:</strong></td>
                <td>{bookingResponse.Notes}</td>
            </tr>
        </table>
                                <p>Please click the button below to login and confirm booking:</p>
                                <a class=""button"" href=""http://localhost:3000/login"">Login Confirm Booking</a>
                            </div>
                        </body>
                        </html>


";

            return body;

        }


    }
}
