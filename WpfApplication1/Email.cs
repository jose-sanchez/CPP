using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace WpfApplication1
{
      class  Email
    {
          
          public static Boolean SendEmail(string ClientID, string ProductID,int Price)
          {
              
              Boolean EmailSent = true;
              Model1Container _dataDC = ModelSingleton.getDataDC;
              //Recover Client and Product Data
              ClientSet Client;

                  
                  Client = _dataDC.ClientSet.First(S => S.Id == ClientID);
                  if (Client.Email != null && Client.Email != "")
                  {
                      ProductsSet Product;
                      Product = _dataDC.ProductsSet.First(S => S.Id == ProductID);
                      //Build the destination address
                      var toAddress = new MailAddress(Client.Email, Client.Name + " " + Client.Surname);
                      var fromAddress = new MailAddress(Properties.Settings.Default.EmailAddress, "Jose Maria Sanchez Colino");
                      //Set the password
                      const string fromPassword = "11122333";
                      //Build the subject
                      string subject = Product.Producto + " " + Product.Marca + " " + Product.Modelo;
                      //Build the body
                      string body = BuildBody(Product, Client, Price);

                      //collecting attached
                      List<ProductImageSet> ImageList = new ObservableProductImage(_dataDC, Product.Id).Where(S => S.Email== true).ToList();


                      var smtp = new SmtpClient
                      {
                          Host = "smtp.gmail.com",
                          Port = 587,
                          EnableSsl = true,
                          DeliveryMethod = SmtpDeliveryMethod.Network,
                          UseDefaultCredentials = false,
                          Credentials = new NetworkCredential(fromAddress.Address, fromPassword)

                      };
                      //Create the email
                      MailMessage message = new MailMessage(fromAddress, toAddress);
                      message.Subject = subject;
                      message.Body = body;
                      foreach (ProductImageSet Image in ImageList)
                      {
                          System.Net.Mail.Attachment attachment;
                          attachment = new System.Net.Mail.Attachment(Image.Path);
                          message.Attachments.Add(attachment);
                      }
                      try
                      {
                          smtp.Send(message);
                      }
                      catch (Exception e) { EmailSent = false; System.Windows.MessageBox.Show(e.ToString()); }
                  }
                  else System.Windows.MessageBox.Show("El cliente " + Client.Name + "no tiene asociado un correo electronico por lo que el correo no le sera enviado");
                  return EmailSent;
          }

          private static string BuildBody(ProductsSet product, ClientSet client, int Price)
          {
              StringBuilder body = new StringBuilder();
              string introduction = "De acuerdo a conversaciones previas habiendo expresado su interes por un/una "
                  + product.Producto;
              body.Append(introduction);
              if(product.Marca !="")
              {
              body.Append(" de la marca " + product.Marca);
              }
              if (product.Modelo != "")
              {

                  body.Append(" modelo " + product.Modelo);
              }
              body.AppendLine(" le envio informacion sobre este/esta " + product.Producto + "el cual podria interesarle:");
              body.AppendLine();
              body.AppendLine("Producto: " + product.Producto);
              body.AppendLine("Modelo: " + product.Modelo);
              body.AppendLine("Marca: " + product.Marca);
              body.AppendLine("Año: " + product.Año);
              body.AppendLine("Peso: " + product.Peso + " Kilos");
              body.AppendLine("Potencia: " + product.Potencia + "CV");
              body.AppendLine("Precio: " + Price + " Euros");
              return body.ToString();
          }
    }
}
