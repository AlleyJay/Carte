using Microsoft.VisualBasic.FileIO;
using Sabio.Data.Providers;
using Sabio.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabio.Services.Interfaces;
using Sabio.Models.Files;
using Sabio.Models.Requests.Files;
using Sabio.Data;
using Amazon;
using Amazon.S3.Transfer;
using Amazon.Runtime;
using Amazon.S3;
using System.IO;
using File = Sabio.Models.Files.File;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Sabio.Models.Domain;
using Microsoft.Extensions.Options;

namespace Sabio.Services
{
    public class FileService : IFileService
    {
        private AwsKeys _awsKeys;

        IDataProvider _data = null;

        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
        private static IAmazonS3 s3Client;

        public FileService(IDataProvider data, IOptions<AwsKeys> awsKeys )
        {
            _data = data;
            _awsKeys = awsKeys.Value;
        }
        public async Task<List<FileBase>> UploadFileAsync(List<IFormFile> files, int userId)
        {
            List<FileBase> result = null;

            s3Client = new AmazonS3Client(_awsKeys.AccessKey, _awsKeys.Secret, RegionEndpoint.GetBySystemName(_awsKeys.BucketRegion));

            var fileTransferUtility = new TransferUtility(s3Client);
            foreach (IFormFile file in files)
            {
                int id = 0;
                string keyName = Guid.NewGuid().ToString() + "_" + file.Name;
                string url = null;

                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = _awsKeys.BucketName,
                    Key = keyName,
                    InputStream = file.OpenReadStream(),
                };

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                
                url = $"{_awsKeys.Domain}{keyName}";
                var fileType = Path.GetExtension(file.FileName).ToString();
                string fileName = file.FileName;
              
                FileAddRequest model = new()
                {
                    Name = fileName,
                    Url = url,
                    FileType = fileType,
                    CreatedBy = userId,
                };
                // Add to DB
                id = Add(model, userId);
            
                if (id > 0)
                {
                    FileBase newBase = new()
                    {
                        Id = id,
                        Url = url,
                        Name = fileName,
                    };

                    if (result == null)
                    {
                        result = new List<FileBase>();
                    }
                    result.Add(newBase);
                }
            }
            return result;
        }
        public int Add(FileAddRequest model, int userId)
        {
            int id = 0;

            string procName = "[dbo].[Files_Insert]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col, userId);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    col.Add(idOut);

                }, returnParameters: delegate (SqlParameterCollection returnCol)
                {
                    object objId = returnCol["@Id"].Value;

                    int.TryParse(objId.ToString(), out id);
                });
            return id;
        }
        public Paged<File> Get(int pageIndex, int pageSize)
        {
            Paged<File> pagedList = null;

            List<File> list = null;

            int totalCount = 0;

            _data.ExecuteCmd(
                "[dbo].[Files_Select_All]",
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    File file = MapSingleFile(reader, ref index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }

                    if (list == null)
                    {
                        list = new List<File>();
                    }
                    list.Add(file);
                });
            if (list != null)
            {
                pagedList = new Paged<File>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }
        public Paged<File> Get(int pageIndex, int pageSize, int userId)
        {
            Paged<File> pagedList = null;

            List<File> list = null;

            int totalCount = 0;

            _data.ExecuteCmd(
                "[dbo].[Files_Select_ByCreatedBy]",
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    File file = MapSingleFile(reader, ref index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }

                    if (list == null)
                    {
                        list = new List<File>();
                    }
                    list.Add(file);
                });
            if (list != null)
            {
                pagedList = new Paged<File>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;

        }
        public File Get(int id)
        {

            string procName = "[dbo].[Files_Select_ById]";

            File file = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {

                paramCollection.AddWithValue("@Id", id);

            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                file = MapSingleFile(reader, ref index);

            }
            );
            return file;
        }
        public Paged<File> GetByType(int pageIndex, int pageSize, int fileTypeId)
        {
            Paged<File> pagedList = null;

            List<File> list = null;

            int totalCount = 0;

            _data.ExecuteCmd(
                "[dbo].[Files_Select_ByFileTypeId]",
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                    parameterCollection.AddWithValue("@FileTypeId", fileTypeId);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    File file = MapSingleFile(reader, ref index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }

                    if (list == null)
                    {
                        list = new List<File>();
                    }
                    list.Add(file);
                });
            if (list != null)
            {
                pagedList = new Paged<File>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }
        public void Update(FileUpdateRequest model)
        {
            string procName = "[dbo].[Files_UpdateDeleteStatus]";
            _data.ExecuteNonQuery(procName,
              inputParamMapper: delegate (SqlParameterCollection col)
              {
                  col.AddWithValue("@Id", model.Id);

              }, returnParameters: null);
        }
        private static void AddCommonParams(FileAddRequest model, SqlParameterCollection col, int userId)
        {
            col.AddWithValue("@Name", model.Name);
            col.AddWithValue("@Url", model.Url);
            col.AddWithValue("@FileType", model.FileType);
            col.AddWithValue("@CreatedBy", userId);
        }
        private static File MapSingleFile(IDataReader reader, ref int startingIndex)
        {
            File file = new File();
            file.FileType = new LookUp();

            file.Id = reader.GetSafeInt32(startingIndex++);
            file.Name = reader.GetSafeString(startingIndex++);
            file.Url = reader.GetSafeString(startingIndex++);
            file.FileType.Id = reader.GetSafeInt32(startingIndex++);
            file.FileType.Name = reader.GetSafeString(startingIndex++);
            file.CreatedBy = reader.GetSafeInt32(startingIndex++);
            file.DateCreated = reader.GetSafeDateTime(startingIndex++);
            file.IsDeleted = reader.GetSafeBool(startingIndex++);

            return file;
        }
    }
}
