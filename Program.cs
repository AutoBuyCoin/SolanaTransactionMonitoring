using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solnet.Rpc; 
using System.Data;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using Websocket.Client;
using static System.TimeZoneInfo;
using Solnet.Metaplex;
using Solnet.Metaplex.NFT.Library;
using Solnet.Rpc.Types;  

namespace Solana链监控指定帐号代币交易
{
   
    internal class Program
    {
        public static List<MonitoredAddress> subAddresses = new List<MonitoredAddress>();
        public static ConsoleManager manager = new ConsoleManager();
        public static  CONFIG cONFIG = new CONFIG();
        static async Task Main(string[] args)
        {
            try
            {
                IniFile ini = new IniFile("CONFIG.INI");
                string? MaxBuy = ini["CONFIG", "金狗提示阈值"];
                string? RpcServer = ini["CONFIG", "RPC服务器地址"];
                string? WWSServer = ini["CONFIG", "WWS服务器地址"];
                string? savelog = ini["CONFIG", "是否保存日志"];
                string? Play = ini["CONFIG", "播放详细交易"]; 
                cONFIG.MaxBuy = MaxBuy;
                cONFIG.RpcClient = RpcServer;
                cONFIG.WWSServer = WWSServer;
                cONFIG.SaveLog = "yes" == savelog.ToLower();
                cONFIG.Play = "yes" == Play.ToLower();
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message);
            } 
            Console.Title = "金狗监视";
            try
            {
                var list = File.ReadAllLines("token.txt");
                var trimmedArray = list.Select(line => line.Trim()).ToArray();
                manager.AddWindows(
                   trimmedArray
                );
                foreach (var line in trimmedArray)
                {
                    var Moit = new MonitoredAddress();
                    Moit.Name = line.Split(',')[0];
                    Moit.Address = line.Split(',')[1];
                    subAddresses.Add(Moit);
                }
            }
            catch (Exception)
            {

                Console.WriteLine("目录下token.txt文件里的监控地址或名称不正确!!!");
                return;
            } 
            _ =Solana.RunWebsocketClientAccount(subAddresses, Print,cONFIG);
            manager.Run();
        }
       static  void Print(string Title,string msg)
        {
            manager.AddContent(Title, msg);
            if (cONFIG.SaveLog)
            {
                File.AppendAllText("日志.log", Title + "--" + msg + "\r\n");
            } 
        }
       
    }


}
