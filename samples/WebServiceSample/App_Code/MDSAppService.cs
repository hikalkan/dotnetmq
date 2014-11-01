using System;
using System.Web.Services;
using MDS.Client.WebServices;

[WebService(Namespace = "http://www.dotnetmq.com/mds")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class MDSAppService : WebService
{
    /// <summary>
    /// MDS server sends messages to this method.
    /// </summary>
    /// <param name="bytesOfMessage">Byte array form of message</param>
    /// <returns>Response message to incoming message</returns>
    [WebMethod(Description = "Receives incoming messages to this web service.")]
    public byte[] ReceiveMDSMessage(byte[] bytesOfMessage)
    {
        var message = WebServiceHelper.DeserializeMessage(bytesOfMessage);
        try
        {
            var response = ProcessMDSMessage(message);
            return WebServiceHelper.SerializeMessage(response);
        }
        catch (Exception ex)
        {
            var response = message.CreateResponseMessage();
            response.Result.Success = false;
            response.Result.ResultText = "Error in ProcessMDSMessage method: " + ex.Message;
            return WebServiceHelper.SerializeMessage(response);
        }
    }

    /// <summary>
    /// Processes incoming messages to this web service.
    /// </summary>
    /// <param name="message">Message to process</param>
    /// <returns>Response Message</returns>
    private IWebServiceResponseMessage ProcessMDSMessage(IWebServiceIncomingMessage message)
    {
        //Process message

        //Send response/result
        var response = message.CreateResponseMessage();
        response.Result.Success = true;
        return response;
    }
}

