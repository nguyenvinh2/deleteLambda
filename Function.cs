using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace deleteLambda
{
  public class Function
  {
    IAmazonS3 S3Client { get; set; }
    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
      S3Client = new AmazonS3Client();
    }

    public Function(IAmazonS3 s3Client)
    {
      this.S3Client = s3Client;
    }
    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
      foreach (var message in evnt.Records)
      {
        await ProcessMessageAsync(message, context);
      }
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
      context.Logger.LogLine($"Processed message {message.Body}");
      DeleteObjectRequest deleteRequest = new DeleteObjectRequest()
      {
        BucketName = "taskmaster-storage",
        Key = message.Body
      };
      
      DeleteObjectRequest deleteThumbnailRequest = new DeleteObjectRequest()
      {
        BucketName = "taskmaster-thumbnail",
        Key = $"thumbnail-{message.Body}"
      };


      // TODO: Do interesting work based on the new message
      await S3Client.DeleteObjectAsync(deleteRequest);
      await S3Client.DeleteObjectAsync(deleteThumbnailRequest);
    }
  }
}
