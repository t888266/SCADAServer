using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Reflection.Emit;
using System;

namespace SCADAServer.Routers.UserRouter.Services
{
    public enum TypeCode
    {
        RST,
        REG,
    }
    public class CodeVerify
    {
        ConcurrentDictionary<int,string> regCodes = new ConcurrentDictionary<int, string>();
        ConcurrentDictionary<int, string> rstCodes = new ConcurrentDictionary<int, string>();
        public int VerifyCodeFor(string token, TypeCode typeCode)
        {
            int rd = 0;
            switch (typeCode)
            {
                case TypeCode.RST:
                    {
                        do
                        {
                            rd = RandomNumberGenerator.GetInt32(100000, 1000000);
                        } while (!rstCodes.TryAdd(rd, token));
                        return rd;
                    }
                default:
                    {
                        do
                        {
                            rd = RandomNumberGenerator.GetInt32(100000, 1000000);
                        } while (!regCodes.TryAdd(rd, token));
                        return rd;
                    }
            }
        }
        public void DeleteVerifyCodeAfter(int rdCode,string token,
        int miliseconds, TypeCode typeCode)
        {
            Task.Run(async () =>
            {
                await Task.Delay(miliseconds);
                switch (typeCode)
                {
                    case TypeCode.RST:
                        {
                            rstCodes.TryRemove(new KeyValuePair<int, string>(rdCode, token));
                            break;
                        }
                    default:
                        {
                            regCodes.TryRemove(new KeyValuePair<int, string>(rdCode, token));
                            break;
                        }
                }
            });
        }
        public bool CheckCode(int code, string token,TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.RST:
                    {
                        if (rstCodes.TryGetValue(code, out string tmpToken))
                        {
                            if (tmpToken.Equals(token))
                            {
                                rstCodes.TryRemove(new KeyValuePair<int, string>(code, 
                                tmpToken));
                                return true;
                            }
                        }
                        break;
                    }
                default:
                    {
                        if (regCodes.TryGetValue(code, out string tmpToken))
                        {
                            if (tmpToken.Equals(token))
                            {
                                regCodes.TryRemove(new KeyValuePair<int, string>(code, 
                                tmpToken));
                                return true;
                            }
                        }
                        break;
                    }
            }
            return false;
        }
    }
}
