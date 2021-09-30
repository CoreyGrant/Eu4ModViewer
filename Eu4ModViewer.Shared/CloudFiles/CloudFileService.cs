using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eu4ModViewer.Shared.Files
{
	public interface ICloudFileService
	{
		Task AddFileAsync(Stream file, string filename, long? modId);
		Task AddJsonFileAsync(object obj, string filename, long? modId);
		Task<byte[]> GetFileAsync(string filename, long? modId);
		Task<byte[]> GetImageFileAsync(string filename, long? modId, string imageFolder);
		Task AddImageFileAsync(byte[] image, string filename, long? modId, string imageFolder);
	}

	public class CloudFileService : ICloudFileService
	{

		private const string containerName = "prod";
		private readonly CloudBlobClient _client;
		private readonly DefaultContractResolver contractResolver = new DefaultContractResolver
		{
			NamingStrategy = new CamelCaseNamingStrategy()
		};
		private readonly JsonSerializerSettings serializerSettings;
		public CloudFileService(IConfiguration configuration)
		{
			serializerSettings = new JsonSerializerSettings
			{
				ContractResolver = contractResolver,
				Formatting = Formatting.None
			};
			_client = CloudStorageAccount.Parse(configuration.GetConnectionString
("AzureFileContainer")).CreateCloudBlobClient();
		}

		public async Task AddFileAsync(Stream file, string filename, long? modId)
		{
			var container = _client.GetContainerReference(containerName);
			var blobReference = container.GetBlockBlobReference(GetFilename(filename, modId));
			await blobReference.DeleteIfExistsAsync();
			await blobReference.UploadFromStreamAsync(file);
		}

		public async Task AddJsonFileAsync(object obj, string filename, long? modId)
		{
			var json = JsonConvert.SerializeObject(obj, serializerSettings);
			var container = _client.GetContainerReference(containerName);
			var blobReference = container.GetBlockBlobReference(GetFilename(filename, modId));
			await blobReference.DeleteIfExistsAsync();
			await blobReference.UploadTextAsync(json);
		}

		public async Task AddImageFileAsync(byte[] image, string filename, long? modId, string imageFolder)
		{
			var container = _client.GetContainerReference(containerName);
			var blobReference = container.GetBlockBlobReference(GetImageFilename(filename, modId, imageFolder));
			await blobReference.DeleteIfExistsAsync();
			await blobReference.UploadFromByteArrayAsync(image,0, image.Length);
		}

		public async Task<byte[]> GetImageFileAsync(string filename, long? modId, string imageFolder)
		{
			if (!filename.EndsWith(".png"))
			{
				filename += ".png";
			}
			var container = _client.GetContainerReference(containerName);
			var blobReference = container.GetBlockBlobReference(GetImageFilename(filename, modId, imageFolder));
			await blobReference.FetchAttributesAsync();
			var ba = new byte[blobReference.Properties.Length];
			await blobReference.DownloadToByteArrayAsync(ba, 0);
			return ba;
		}

		public async Task<byte[]> GetFileAsync(string filename, long? modId)
		{
			var container = _client.GetContainerReference(containerName);
			var blobReference = container.GetBlockBlobReference(GetFilename(filename, modId));
			await blobReference.FetchAttributesAsync();
			var ba = new byte[blobReference.Properties.Length];
			await blobReference.DownloadToByteArrayAsync(ba, 0);
			return ba;
		}

		private string GetImageFilename(string filename, long? modId, string imageFolder)
		{
			return modId.HasValue
				? $"{modId.Value}|image|{imageFolder}|{filename}"
				: $"base|image|{imageFolder}|{filename}";
		}

		private string GetFilename(string filename, long? modId)
		{
			return modId.HasValue
				? $"{modId.Value}|{filename}"
				: $"base|{filename}";
		}
	}
}
