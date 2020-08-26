
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Jc.Core.AntChain
{
    /// <summary>
    /// SinoChainHelper
    /// </summary>
    public class SinoChainHelper
    {
        private AntChainHelper chainHelper;
        private AntChainConfig config;

        private static SinoChainHelper instance;
        public static SinoChainHelper Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new SinoChainHelper();
                }
                return instance;
            }
        }

        private SinoChainHelper()
        {
            config = new AntChainConfig() {
                AccessId = "UaP57TiaLEUNSLQR",
                AccessKey = GetAccessKey(),
                TenantId = "LEUNSLQR",
                Account = "sinocloud",
                MykmsKeyId = "X16U3lOwLEUNSLQR1596598416300",
            };
            chainHelper = new AntChainHelper(config);
        }

        /// <summary>
        /// 插入记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string SetExamRecord(RecordModel model)
        {
            string orderId = $"order_{Guid.NewGuid()}";
            string contractName = "ExamRecord4";
            string methodSignature = "setExamRecord(string,string,string,string,string,string,string,string,string,string,string,string)";

            List<string> list = new List<string>()
            {   
                model.RecordId,
                model.ExamId,
                model.PatientName,
                model.ExamNumber,
                model.ExamDate.ToString("yyyy-MM-dd HH:mm"),
                model.UploadDate.ToString("yyyy-MM-dd HH:mm"),
                model.Uploader,
                model.Operation,
                model.Operator,
                model.PowerUser,
                model.PowerDate.ToString("yyyy-MM-dd HH:mm"),
                model.PowerNote
            };             
            string paramListStr = JsonSerializer.Serialize(list);
            AntChainRequestResult result = chainHelper.CallSolidityContract(orderId, contractName, methodSignature, paramListStr);
            if(!result.Success)
            {
                throw new Exception($"Set Record Error,Error Code :{result.Code}");
            }
            return result.Data;
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string UpdateRecord(RecordModel model)
        {
            string orderId = $"order_{Guid.NewGuid()}";
            string contractName = "ExamRecord3";
            string methodSignature = "setExamRecord(string,string,string,string,string,string)";

            List<string> list = new List<string>()
            {   model.RecordId,
                model.Operation,
                model.Operator,
                model.PowerUser,
                model.PowerDate.ToString("yyyy-MM-dd HH:mm"),
                model.PowerNote
            };
            string paramListStr = JsonSerializer.Serialize(list);
            AntChainRequestResult result = chainHelper.CallSolidityContract(orderId, contractName, methodSignature, paramListStr);
            if (!result.Success)
            {
                throw new Exception($"Set Record Error,Error Code :{result.Code}");
            }
            return result.Data;
        }


        /// <summary>
        /// Get Access Key
        /// </summary>
        /// <returns></returns>
        private string GetAccessKey()
        {
            string key = @"MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCcpiAm0c8kAh5z
FuOnz6TBg6oUMuG6JUN + UHw6zIfwtjp3ZUAfvPWVNZj0NIIQjKKSmjPgqyCyD8EN
GwiSFHR9nOPnMTCUiX1Q +/ WEpxXCCCROkGH0xDSqYzScs2TulBgS1 + JrbMwvzmKG
ayXbpp1N0CDCFimRD2zR028VB6rhdNx3cyrpMotCsjLptV + wH / t5mHYpoOwb + jhB
Y1R30a8C / 3XReaYxQ5NjgB22YgB6cN3Qtp5UC8OPkIaQdy / iLj8yWQ0lf9W21w3 /
  plNuSzFnqLobWuuazjDFw3xHLqduc4sFGpsVFW73uF8gCY7NvjK9u6aPWQFvWdlt
n8BbhatnAgMBAAECggEADYjjBUeihKN0OxByuhicTSYRp24gB8PZeYv9t9zdMASm
t5M1f6iFdY9seEkjJcfo8g7Fxbcze38V + Ipp6qk + yW2pWVvsLSFWBQ3IEF6ZaCro
7CrYc9wSCtjIfnOXmG + ORu6FPy / m0oicBa4zVq + mLsd5VRuyGrkA934zvlbrwlAC
WAD3n5yuCasKYAg1g6R + asiGziZmI / NNZkwx8aQniLIMdPKTUuipDUN7a / PWSbqt
a + kYu9 / 6XTxINaadBzPTKN3Ax4ebXhCgb90D8sItN03sRY5ZfegUD9rWZW / LqjMb
hvYoBvrBfWvL6WXi7eqd0EqeIfD19RhCNygP6qvJUQKBgQDYsOy8bsFPmRhgvciH
qAdaFpMVLjvjVQuUCBUpEvfKyYkTuJTUwY7QD2PWkIJS8lKfofl4G5BQ0ZqEoL0k
+ PtfxR5OQXNs4HJxjsQ91CG7T + XZ3jwRKVF8BPfkMGsuOlsxy / wwRyuJU + ZEFulK
/ nwnPOLXUz6AdtSMcVknRLhblwKBgQC5ENvCVraCmnmERZ + CtHsBCbv / 3RKDcBZA
gmHwFhGaw / VqZLWEOAfzXghPOIidnWnZbcKoifaQ6O0ajmAoWczXNmTf4ZEsKIRH
yUUxTalWj1WrHkgyBopWlZRQWe / AdEfUwWr1L2VGlYtKynl4sPclB0IslxV3rXxE
cdTBsw9osQKBgAzV3hADV3wJi4Ife8NdVqIleCznAjEjFn58RBScxQTED1SVuGsp
0 / XLE5TgFngnVnGSPJ66sz + 2SrrUcp / AB9PSb0sfYWDvopfLZyBqcl0QDINtQI2b
rvHzsWNY4uBoIILAnH2XmaKWz6r02HjSvjPszVsH2UnuwARZqnhC / 9CdAoGAZAmK
4VhCPAMIoMmJ2ft5aiw65ao48bfLL3 / LNR6Y0ZIPvT1HKXuoQJOZ5kjnAFww9Ylx
ae15zQc789j2fZldMxCCIssp0Dbummdf8bDLUrEUW / V4jSIf8YHVtEGJafzxPz52
dKb0Fd2MeBdO5kCyt99ek//vaMvsmor1uObz82ECgYACj1OkcaIzOQfIUjYZ8JcL
RPy5PJfvbl / kLYqTxGRGHP / +0A43A6ddst9Pu3tMUPwYdtn3O0fjD5kvu / jwI1ef
FIng9rIaAaVlwWGRH7CyCNJObZZPWneWa1 + EeRwE972e5EQci7Sj6PyXfEI00NsK
HdhdoC / gUmLDO39EwNjMMQ == ";

            return key;
        }

    }

    /// <summary>
    /// 记录插入Model
    /// </summary>
    public class RecordModel
    {
        /// <summary>
        /// 记录Id 唯一 插入记录,更新记录使用
        /// </summary>
        public string RecordId { get; set; }

        /// <summary>
        /// 检查记录Id
        /// </summary>
        public string ExamId { get; set; }

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 检查编号
        /// </summary>
        public string ExamNumber { get; set; }

        /// <summary>
        /// 检查日期
        /// </summary>
        public DateTime ExamDate { get; set; }

        /// <summary>
        /// 上传日期
        /// </summary>
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        public string Uploader { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 授权人
        /// </summary>
        public string PowerUser { get; set; }

        /// <summary>
        /// 授权日期
        /// </summary>
        public DateTime PowerDate { get; set; }

        /// <summary>
        /// 授权备注
        /// </summary>
        public string PowerNote { get; set; }
    }

}