
using System;
using System.Collections.Generic;

namespace Jc.AntChain
{
    public enum Method
    {
        /// <summary>
        /// 存证
        /// </summary>
        DEPOSIT,
        /// <summary>
        /// 部署WASM合约
        /// </summary>
        DEPLOYWASMCONTRACT,
        /// <summary>
        /// 调用WASM合约
        /// </summary>
        CALLWASMCONTRACTASYNC,
        /// <summary>
        /// 部署Solidity合约
        /// </summary>
        DEPLOYCONTRACTFORBIZ,
        /// <summary>
        /// 异步调用Solidity合约
        /// </summary>
        CALLCONTRACTBIZASYNC,
        /// <summary>
        /// 部署合约
        /// </summary>
        DEPLOYCONTRACT,
        /// <summary>
        /// 调用合约
        /// </summary>
        CALLCONTRACT,
        /// <summary>
        /// 创建账号
        /// </summary>
        CREATEACCOUNT
    }

    /// <summary>
    /// AntChain Config
    /// </summary>
    public class AntChainConfig
    {
        /// <summary>
        /// AccessId 申请 AccessKey 时返回的 AccessKey ID
        /// </summary>
        public string AccessId { get; set; }

        /// <summary>
        /// AccessKey
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// 账户名
        /// 使用非托管账户需要，创建链账户时的账户名
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 私钥
        /// 使用非托管账户需要，创建链账户时返回的私钥
        /// </summary>
        public string MykmsKeyId { get; set; }

        /// <summary>
        /// 租户Id
        /// 发起账户是密钥托管的链账户时需要，非托管模式不需要该参数
        /// </summary>
        public string TenantId { get; set; }
    }

    /// <summary>
    /// AntChain RequestResult
    /// </summary>
    public class AntChainRequestResult
    {
        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }


        /// <summary>
        /// Data
        /// </summary>
        public string Data { get; set; }
    }
}