using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 
using Solnet.Metaplex.NFT.Library;
using Solnet.Rpc;
using Solnet.Wallet;
using System.Net.WebSockets;
using System.Text;
using Websocket.Client;
using NAudio.Wave; 
{
    public class Solana
    {
        private static  Action<string,string > _Print;

        public static Uri ServerUrl = new Uri("wss://api.mainnet-beta.solana.com");
        public static string RpcServer = "https://solana-rpc.publicnode.com"; 
        // public static  Uri ServerUrl = new Uri("wss://solana-rpc.publicnode.com");
        public  static  List<MonitoredAddress> subAddresses = new List<MonitoredAddress>(); 
        public static string BuyBase64 = "data:audio/mpeg;base64,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VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV";
        public static string SellBase64 = "data:audio/mpeg;base64,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VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVMQU1FMy4xMDBVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//NkxHwAAANIAAAAAFVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV";
        public static int  MaxTrans = 10;
        public static decimal TransCt = 0;
        public static  bool SaveLog = true;
        public static bool Play = true;
        public static bool isTTS = false;
        public static Dictionary<int, MonitoredAddress> subscriptionMap = new Dictionary<int, MonitoredAddress>(); 
        public static bool _needReconnect = false;
        public static List<string> nodes = new List<string>
            {
                "https://solana-rpc.publicnode.com",
                "https://api.mainnet-beta.solana.com",
                "https://solana-api.projectserum.com",
                "https://rpc.ankr.com/solana",
                "https://mainnet.rpcpool.com",
            };

        public static int currentNodeIndex = 0; 
        public static async Task RunWebsocketClientAccount(List<MonitoredAddress> _subAddresses, Action<string, string> Print,CONFIG cONFIG)
        {
            try
            {
                TextToSpeechPlayer  player = new TextToSpeechPlayer();
                isTTS = true;
            }
            catch (Exception ex)
            {
                _Print("Logs", ex.Message);
                isTTS = false;
            }
            ServerUrl =new Uri(cONFIG.WWSServer);
            RpcServer = cONFIG.RpcClient;
            subAddresses = _subAddresses;
            SaveLog = cONFIG.SaveLog;
            MaxTrans =int.Parse( cONFIG.MaxBuy);
            Play = cONFIG.Play;
            _Print = Print;
            _Print("Logs",$"RPC:{RpcServer}");
            _Print("Logs", $"WWS:{cONFIG.WWSServer}");
          
            while (true)
            {
                try
                {
                    _needReconnect = false;
                    using (var client = new WebsocketClient(ServerUrl))
                    {
                        // 增加超时时间
                        client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                        client.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);

                        // 添加心跳定时器
                        var pingTimer = new System.Timers.Timer(15000); // 15秒发送一次心跳
                        pingTimer.Elapsed += (sender, e) =>
                        {
                            try
                            {
                                if (client.IsRunning)
                                {
                                    var pingMessage = new
                                    {
                                        jsonrpc = "2.0",
                                        id = "ping",
                                        method = "ping"
                                    };
                                    client.Send(JsonConvert.SerializeObject(pingMessage));
                                }
                            }
                            catch (Exception ex)
                            {
                                _Print("Logs", $"发送心跳出错: {ex.Message}");
                            }
                        };
                        pingTimer.Start();

                        client.MessageReceived.Subscribe(msg => HandleMessageAccount(msg, client));

                        client.ReconnectionHappened.Subscribe(info =>
                        {
                            _Print("Logs", $"WebSocket连接: {info.Type}");
                            if (info.Type != ReconnectionType.Initial)
                            {
                                // 重新订阅所有地址
                                foreach (var item in subAddresses)
                                {
                                    SendSubscriptionWithMentions(client, item);
                                }
                            }

                        });

                        client.DisconnectionHappened.Subscribe(info =>
                        {
                            _Print("Logs", $"WebSocket断开连接: {info.Type}");
                            if (info.Type == DisconnectionType.NoMessageReceived ||
                                info.Type == DisconnectionType.Error)
                            {
                                _needReconnect = true;
                            }
                        });

                        await client.Start();

                        // 订阅所有地址
                        foreach (var item in subAddresses)
                        {
                            SendSubscriptionWithMentions(client, item);
                        }

                        // 等待直到需要重连
                        while (!_needReconnect)
                        {
                            await Task.Delay(1000);
                        }

                        // 清理资源
                        pingTimer.Stop();
                        pingTimer.Dispose();
                        await client.Stop(WebSocketCloseStatus.NormalClosure, "Reconnecting");
                        _Print("Logs", "正在关闭旧连接...");
                    }

                    _Print("Logs", "等待5秒后重新连接...");
                    await Task.Delay(5000);
                }
                catch (Exception ex)
                {
                    _Print("Logs", $"连接错误: {ex.Message}");
                    await Task.Delay(5000);
                }
            }
        }
        public static  List<JToken> sj = new List<JToken>();
        public static async void HandleMessageAccount(ResponseMessage msg, WebsocketClient client)
        {
           
            try
            {
                var json = JObject.Parse(msg.Text);

                // 处理心跳响应
                if (json["id"]?.ToString() == "ping")
                {
                    // Console.WriteLine("收到心跳响应");
                    return;
                }

                // 处理订阅结果
                // 处理订阅响应，保存订阅ID
                if (json["result"] != null && json["id"] != null)
                {
                    string addressName = json["id"].ToString();
                    int subscriptionId = json["result"].Value<int>();

                    var address = subAddresses.FirstOrDefault(a => a.Name == addressName);
                    if (address != null)
                    {
                        address.SubscriptionId = subscriptionId;
                        subscriptionMap[subscriptionId] = address;
                        _Print("Logs", $"{address.Name} 订阅成功，订阅ID: {subscriptionId}");
                    }
                    return;
                }


                // 处理日志通知
                if (json["method"]?.ToString() == "logsNotification")
                {
                    var subscription = json["params"]?["subscription"]?.Value<int>();
                    if (subscription.HasValue && subscriptionMap.TryGetValue(subscription.Value, out var monitoredAddress))
                    {
                        var signature = json["params"]?["result"]?["value"]?["signature"]?.ToString();
                        var logs = json["params"]?["result"]?["value"]?["logs"] as JArray;
                        if (!sj.Contains(signature))
                        { 
                            if (signature != null && logs != null)
                            {
                             await   GetTransactionDetails(signature, monitoredAddress);
                                //  File.AppendAllText("ALLJS.JSON", json.ToString());

                                if (sj.Count > 1000)
                                {
                                    sj.Clear();
                                }
                                sj.Add(signature);
                            }
                        }
                    }



                }

            }
            catch (Exception ex)
            {
                _Print("Logs", $"处理消息时出错: {ex.Message}");
            }
        }
        public static void SendSubscriptionWithMentions(WebsocketClient client, MonitoredAddress address)
        {
            try
            {
                var subscribeMessage = new
                {
                    jsonrpc = "2.0",
                    id = address.Name,  // 使用地址名称作为请求ID
                    method = "logsSubscribe",
                    @params = new object[]
                    {
                new
                {
                    mentions = new string[] { address.Address }
                },
                new
                {
                    commitment = "finalized"
                }
                    }
                };

                string jsonRequest = JsonConvert.SerializeObject(subscribeMessage);
                //   Console.WriteLine($"发送订阅请求: {jsonRequest}");
                client.Send(jsonRequest);
                //  Console.WriteLine($"已发送 {address.Name} 的订阅请求");
            }
            catch (Exception ex)
            {
                _Print("Logs", $"发送订阅请求时出错: {ex.Message}");
                _needReconnect = true;
            }
        }  //订阅 
        public static async Task GetTransactionDetails(string signature, MonitoredAddress address)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new
                    {
                        jsonrpc = "2.0",
                        id = 1,
                        method = "getTransaction",
                        @params = new object[]
                        {
                    signature,
                    new
                    {
                        encoding = "jsonParsed",
                        commitment = "confirmed",
                        maxSupportedTransactionVersion = 0
                    }
                        }
                    };

                    var content = new StringContent(
                        JsonConvert.SerializeObject(request),
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.PostAsync(
                        RpcServer,
                        content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var transactionJson = JObject.Parse(result);
                        var transaction = transactionJson["result"];
                        //  Console.WriteLine(transaction);
                        if (transaction != null)
                        {
                            var meta = transaction["meta"];
                            if (meta != null)
                            {
                                string dexType = GetDexType(transaction);
                                bool isSwap = !string.IsNullOrEmpty(dexType);
                                var defaultCl = Console.ForegroundColor;

                                if (isSwap)
                                {
                                    ProcessSwapTransaction(transaction, meta, signature, address, dexType);
                                }
                                else
                                {
                                    ProcessNonSwapTransaction(transaction, meta, signature, address);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _Print("Logs", $"获取交易详情时出错: {ex.Message}");
                //if (ex.Message.Contains("The SSL connection could not be established") && retryCount<nodes.Count)
                //{
                //    string nextNode = GetNextNode();
                //    Print($"节点连接失败，切换到下一个节点: {nextNode}");
                //    await Task.Delay(1000);
                //    await GetTransactionDetails(signature, address, retryCount + 1);
                //}
                //else if (retryCount >= nodes.Count)
                //{
                //    Console.WriteLine("已尝试所有节点，仍然失败");
                //}
            }
        }

        public static void ProcessSwapTransaction(JToken transaction, JToken meta, string signature, MonitoredAddress address, string dexType)
        {
            
            long? blockTime = transaction["blockTime"]?.Value<long>();
            DateTime? transactionTime = blockTime.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(blockTime.Value).LocalDateTime
                : null;

            var preTokenBalances = meta["preTokenBalances"] as JArray;
            var postTokenBalances = meta["postTokenBalances"] as JArray;


            // 检查新创建的代币账户
            var newTokenAccounts = postTokenBalances?
                .Where(p => p["owner"]?.ToString() == address.Address &&
                           !preTokenBalances.Any(pre => pre["mint"]?.ToString() == p["mint"]?.ToString() &&
                                                      pre["owner"]?.ToString() == address.Address))
                .ToList();

            if (newTokenAccounts?.Any() == true)
            {
                // 处理新创建的代币账户
                foreach (var newAccount in newTokenAccounts)
                {
                    ProcessNewTokenAccount(transaction, meta, signature, address, newAccount, transactionTime);
                }
            }
            else
            {
                // 处理现有代币账户的变化
                foreach (var preBalance in preTokenBalances ?? new JArray())
                {
                    if (preBalance["owner"]?.ToString() == address.Address)
                    {
                        ProcessExistingTokenAccount(transaction, meta, signature, address, preBalance, postTokenBalances, transactionTime);
                    }
                }
            }

            
        }

        public static void ProcessNewTokenAccount(JToken transaction, JToken meta, string signature,
            MonitoredAddress address, JToken newAccount, DateTime? transactionTime)
        {
            string mint = newAccount["mint"].ToString();
            decimal postAmount = GetTokenAmount(newAccount["uiTokenAmount"]);

            if (mint != "So11111111111111111111111111111111111111112" && postAmount > 0)
            {
                var accountIndex = GetAccountIndex(transaction, address.Address);
                if (accountIndex.HasValue)
                {
                    var (solChange, solFinalBalance) = CalculateSolChange(meta, accountIndex.Value);

                    PrintTransactionDetails(
                        signature,
                        transactionTime,
                        solChange,
                        solFinalBalance,
                        postAmount,
                        0,
                        mint,
                        "======买入======",
                        address
                    );
                }
            }
        }

        public static void ProcessExistingTokenAccount(JToken transaction, JToken meta, string signature,
            MonitoredAddress address, JToken preBalance, JArray postTokenBalances, DateTime? transactionTime)
        {
            string mint = preBalance["mint"].ToString();
            decimal preAmount = GetTokenAmount(preBalance["uiTokenAmount"]);

            var postBalance = postTokenBalances?
                .FirstOrDefault(b => b["mint"]?.ToString() == mint &&
                                   b["owner"]?.ToString() == address.Address);

            decimal postAmount = postBalance != null ? GetTokenAmount(postBalance["uiTokenAmount"]) : 0;
            decimal change = postAmount - preAmount;

            if (mint != "So11111111111111111111111111111111111111112" && change != 0)
            {
                var accountIndex = GetAccountIndex(transaction, address.Address);
                if (accountIndex.HasValue)
                {
                    var (solChange, solFinalBalance) = CalculateSolChange(meta, accountIndex.Value);

                    PrintTransactionDetails(
                        signature,
                        transactionTime,
                        solChange,
                        solFinalBalance,
                        Math.Abs(change),
                        postAmount,
                        mint,
                        change < 0 ? "======卖出======" : "=====买入======",
                        address
                    );
                }
            }
        }

        public static decimal GetTokenAmount(JToken tokenAmount)
        {
            if (tokenAmount["uiAmount"] != null && !tokenAmount["uiAmount"].Type.HasFlag(JTokenType.Null))
            {
                return tokenAmount["uiAmount"].Value<decimal>();
            }

            var uiAmountString = tokenAmount["uiAmountString"]?.ToString();
            if (!string.IsNullOrEmpty(uiAmountString))
            {
                if (decimal.TryParse(uiAmountString, out decimal xamount))
                {
                    return xamount;
                }
            }

            var amount = tokenAmount["amount"]?.ToString();
            var decimals = tokenAmount["decimals"]?.Value<int>() ?? 0;
            if (!string.IsNullOrEmpty(amount) && decimal.TryParse(amount, out var rawAmount))
            {
                return rawAmount / (decimal)Math.Pow(10, decimals);
            }

            return 0;
        }

        public static int? GetAccountIndex(JToken transaction, string address)
        {
            return transaction["transaction"]?["message"]?["accountKeys"]?
                .Select((account, index) => new { Account = account, Index = index })
                .FirstOrDefault(a => a.Account["pubkey"]?.ToString() == address)?.Index;
        }

        public static (decimal change, decimal finalBalance) CalculateSolChange(JToken meta, int accountIndex)
        {
            var preBalances = meta["preBalances"] as JArray;
            var postBalances = meta["postBalances"] as JArray;

            decimal preSol = preBalances[accountIndex].Value<long>() / 1e9m;
            decimal postSol = postBalances[accountIndex].Value<long>() / 1e9m;

            return (postSol - preSol, postSol);
        }
        public static void Print(string str, ConsoleColor consoleColor = ConsoleColor.White)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine($"[{DateTime.Now}] {str}");
            Console.ForegroundColor = ConsoleColor.White;


        }
        public static   List<string> tempsig = new List<string>();
        public static async void PrintTransactionDetails(string signature, DateTime? transactionTime,
            decimal solChange, decimal solFinalBalance, decimal tokenAmount, decimal tokenBalance,
            string mint, string direction, MonitoredAddress address)
          {

            if (tempsig.Contains(signature))
            {
                return;
            }

            if (solChange != 0 && tokenAmount != 0)
            {
                decimal price = Math.Abs(solChange / tokenAmount);
                var user = address.Name + "," + address.Address;
                _Print(user, $"\n\r====有新交易====");
                if (Play)
                {
                    var a = TextToSpeechPlayer.PlayTextAsync($"{address.Name}有新交易");
                }
                if (transactionTime.HasValue)
                {
                    _Print(user, $"交易时间: {transactionTime.Value:yyyy-MM-dd HH:mm:ss}");
                }
                string tokenName = await GetTokenSymbol(mint);
                if (solChange < MaxTrans && solChange > -MaxTrans)
                {
                    _Print(user, $"====PVP====");
                    _Print(user, $"[{direction}]\r\n数量:[{tokenAmount:F6}]\r\n[{tokenName}]\r\n合约地址:{mint})");
                    var solprice = await GetSolPrice();
                    var usdtprice = solprice * price;
                    _Print(user, $"价格: [{usdtprice:F9}] USDT");

                    if (tokenBalance > 0)
                    {
                        _Print(user, $"剩余持仓: [{tokenBalance:F6}]");
                    }
                    if (direction.Contains("买入"))
                    {
                        _Print(user, $"买入金额:{solChange:F9}SOL     {solChange * solprice:F2}USDT");
                    }
                    else
                    {
                        _Print(user, $"卖出金额:{solChange:F9}SOL     {solChange * solprice:F2}USDT");
                    }
                
                }
                else
                {
                    _Print(user, $"====金狗====");
                    if (direction.Contains("买入"))
                    {
                        if (isTTS)
                        {
                            var a = TextToSpeechPlayer.PlayTextAsync($"{address.Name}买入金狗");
                        }
                        else
                        {
                            PlayBase64Audio(BuyBase64);
                        }
                       
                        _Print("Logs",$"{address.Name}买入金狗\r\n{mint}");
                    }
                    else
                    {
                        if (isTTS)
                        {
                            var a = TextToSpeechPlayer.PlayTextAsync($"{address.Name}卖出金狗");
                        }
                        else
                        {
                            PlayBase64Audio(SellBase64);
                        }
                       
                        _Print("Logs", $"{address.Name}卖出金狗\r\n{mint}");
                    }
                    _Print(user, $"[{direction}]\r\n数量:[{tokenAmount:F6}]\r\n[{tokenName}]\r\n合约地址:{mint}(点击复制)");
                    var solprice = await GetSolPrice();
                    var usdtprice = solprice * price;
                    _Print(user, $"价格: [{usdtprice:F9}] USDT");

                    if (tokenBalance > 0)
                    {
                        _Print(user, $"剩余持仓: [{tokenBalance:F6}]");
                    }
                    if (direction.Contains("买入"))
                    {
                        _Print(user, $"买入金额:{solChange:F9}SOL     {solChange * solprice:F2}USDT");
                    }
                    else
                    {
                        _Print(user, $"卖出金额:{solChange:F9}SOL     {solChange * solprice:F2}USDT");
                    }

                    if (Play)
                    {
                        string result = direction.Contains("买入") ? "买入" :
                                        direction.Contains("卖出") ? "卖出":"";
                        var playinfo = $"{address.Name}{result}{tokenAmount:F2}枚{tokenName}{result}价格{usdtprice:F6}{result}金额{solChange:F6}搜";
                        var a = TextToSpeechPlayer.PlayTextAsync($"{playinfo}");
                    }
                    else
                    {

                    }
                }  
            }
            else
            {
                //Console.WriteLine($"=== 交易详情 ===");
                //if (transactionTime.HasValue)
                //{
                //    Console.WriteLine($"交易时间: {transactionTime.Value:yyyy-MM-dd HH:mm:ss}");
                //}
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"交易方向: [转入代币]\r\n数量:[{tokenAmount:F6}]\r\n {tokenName} (合约地址:{mint})");
                //if (tokenBalance > 0)
                //{
                //    Console.WriteLine($"剩余持仓: [{tokenBalance:F6}]");
                //}
            }
            if (tempsig.Count>1000)
            {
                tempsig.Clear();
            }
            tempsig.Add(signature);

        }
        public static async Task<decimal> GetSolPrice()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(
                        "https://api.binance.com/api/v3/ticker/price?symbol=SOLUSDT");

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var json = JObject.Parse(result);
                        return json["price"].Value<decimal>();
                    }
                }
            }
            catch (Exception ex)
            {
                _Print("Logs", $"获取SOL价格时出错: {ex.Message}");
            }
            return 0;
        }
        public static void ProcessNonSwapTransaction(JToken transaction, JToken meta, string signature, MonitoredAddress address)
        {
           
            var preTokenBalances = meta["preTokenBalances"] as JArray;
            var postTokenBalances = meta["postTokenBalances"] as JArray;
            var preBalances = meta["preBalances"] as JArray;
            var postBalances = meta["postBalances"] as JArray;

            // 跟踪SOL和代币变化
            decimal? solChange = null;
            decimal? solFinalBalance = null;
            Dictionary<string, (decimal change, decimal finalBalance, string name)> tokenChanges =
                new Dictionary<string, (decimal, decimal, string)>();

            // 计算SOL变化
            var accountIndex = transaction["transaction"]?["message"]?["accountKeys"]?
                .Select((account, index) => new { Account = account, Index = index })
                .FirstOrDefault(a => a.Account["pubkey"]?.ToString() == address.Address)?.Index;

            if (accountIndex.HasValue)
            {
                decimal preSol = preBalances[accountIndex.Value].Value<long>() / 1e9m;
                decimal postSol = postBalances[accountIndex.Value].Value<long>() / 1e9m;
                solChange = postSol - preSol;
                solFinalBalance = postSol;
            }

            // 计算代币变化

            //try
            //{
            //    foreach (var postBalance in postTokenBalances ?? new JArray())
            //    {
            //        if (postBalance["owner"]?.ToString() == address.Address)
            //        {
            //            var mint = postBalance["mint"].ToString();
            //            var postAmount = postBalance["uiTokenAmount"]["uiAmount"].Value<decimal>();
            //            var preAmount = preTokenBalances?
            //                .FirstOrDefault(b => b["mint"]?.ToString() == mint)?
            //                ["uiTokenAmount"]["uiAmount"]?
            //                .Value<decimal>() ?? 0;

            //            // 直接使用mint地址，不需要GetTokenName
            //            tokenChanges[mint] = (postAmount - preAmount, postAmount, mint);
            //        }

            //    }
            //}
            //catch (Exception)
            //{ 
            //    // 获取交易时间
            //    long? blockTime = transaction["blockTime"]?.Value<long>();
            //    DateTime? transactionTime = blockTime.HasValue
            //        ? DateTimeOffset.FromUnixTimeSeconds(blockTime.Value).LocalDateTime
            //        : null; 
            //    // 处理代币转账
            //    foreach (var preBalance in preTokenBalances ?? new JArray())
            //    {
            //        if (preBalance["owner"]?.ToString() == address.Address)
            //        {
            //            string mint = preBalance["mint"].ToString();
            //            decimal preAmount = 0;

            //            // 安全地获取preAmount
            //            var preAmountToken = preBalance["uiTokenAmount"];
            //            if (preAmountToken != null)
            //            {
            //                if (preAmountToken["uiAmount"] != null && !preAmountToken["uiAmount"].Type.HasFlag(JTokenType.Null))
            //                {
            //                    preAmount = preAmountToken["uiAmount"].Value<decimal>();
            //                }
            //                else if (!string.IsNullOrEmpty(preAmountToken["uiAmountString"]?.ToString()))
            //                {
            //                    decimal.TryParse(preAmountToken["uiAmountString"].ToString(), out preAmount);
            //                }
            //            }

            //            // 查找对应的post余额
            //            var postBalance = postTokenBalances?
            //                .FirstOrDefault(b => b["mint"]?.ToString() == mint &&
            //                                   b["owner"]?.ToString() == address.Address);

            //            decimal postAmount = 0;
            //            if (postBalance != null)
            //            {
            //                var postAmountToken = postBalance["uiTokenAmount"];
            //                if (postAmountToken != null)
            //                {
            //                    if (postAmountToken["uiAmount"] != null && !postAmountToken["uiAmount"].Type.HasFlag(JTokenType.Null))
            //                    {
            //                        postAmount = postAmountToken["uiAmount"].Value<decimal>();
            //                    }
            //                    else if (!string.IsNullOrEmpty(postAmountToken["uiAmountString"]?.ToString()))
            //                    {
            //                        decimal.TryParse(postAmountToken["uiAmountString"].ToString(), out postAmount);
            //                    }
            //                }
            //            }

            //            decimal change = postAmount - preAmount;

            //            if (change != 0)
            //            {
            //                // 计算SOL变化
            //                var accountIndexx = GetAccountIndex(transaction, address.Address);
            //                if (accountIndexx.HasValue)
            //                {
            //                    var (solChanges, solFinalBalances) = CalculateSolChange(meta, accountIndexx.Value);

            //                    Console.WriteLine($"\n=== Token转账 ===");
            //                    if (transactionTime.HasValue)
            //                    {
            //                        Console.WriteLine($"交易时间: {transactionTime.Value:yyyy-MM-dd HH:mm:ss}");
            //                    }
            //                    string direction = change < 0 ? "转出" : "收到";
            //                    Console.WriteLine($"代币{direction}: {Math.Abs(change):F6} ({mint})");
            //                    Console.WriteLine($"代币当前余额:{postAmount:F6}");
            //                    Console.WriteLine($"交易签名: {signature}");
            //                    Console.WriteLine($"SOL变化: {solChanges:F9} SOL");
            //                    Console.WriteLine($"SOL当前余额: {solFinalBalances:F9} SOL");



            //                }
            //            }
            //        }
            //    }
            //    return;
            //}

            try
            {
                ProcessSwapTransaction(transaction, meta, signature, address, "");
            }
            catch (Exception e)
            {
                // 打印交易信息
                if ((solChange.HasValue && solChange != 0) || tokenChanges.Any())
                {
                    var user = $"{address.Name},{address.Address}";
                    _Print(user, $"===有新交易(非Swap)===");
                    // Console.WriteLine($"交易签名: {signature}");

                    if (solChange.HasValue && solChange != 0)
                    {
                        string direction = solChange < 0 ? "支付" : "收到";
                        _Print(user, $"SOL {direction}: {Math.Abs(solChange.Value):F9} SOL");
                        if (solFinalBalance.HasValue)
                        {
                            _Print(user, $"SOL 当前余额: {solFinalBalance:F9} SOL");
                        }
                    }

                    foreach (var tokenInfo in tokenChanges)
                    {
                        var (change, finalBalance, mint) = tokenInfo.Value;
                        if (change != 0)
                        {
                            string direction = change > 0 ? "收到" : "支付";
                            _Print(user, $"代币 {direction}: {Math.Abs(change)} ({mint})");
                            _Print(user, $"代币当前余额: {finalBalance}");
                            if (solChange.HasValue && solChange != 0)
                            {
                                decimal price = Math.Abs(solChange.Value) / Math.Abs(change);
                                _Print(user, $"价格: {price:F9} SOL/{mint}");
                            }
                        }
                    }

                     
                }

            }
             
        }
        public static string GetDexType(JToken transaction)
        {
            var programIds = new Dictionary<string, string>
    {
        // 主流DEX
        {"6EF8rrecthR5Dkzon8Nwu78hRvfCKubJ14M5uBEwF6P", "Pump.fun"},
        {"675kPX9MHTjS2zt1qfr1NYHuzeLXfQM9H24wFSUt1Mp8", "Raydium"},
        {"GP8StUXNYSZjPikyRsvkTbvRV1GBxMErb59cpeCJnDf1","Jupiter"},
        {"5Q544fKrFoe6tsEbD7S8EmxGTJYAKtTVhAW5Q5pge4j1","Raydium Authority V4" },
        {"JUP4Fb2cqiRUcaTHdrPC8h2gNsA2ETXiPDD33WcGuJB", "Jupiter"},
        {"whirLbMiicVdio4qvUfM5KAg6Ct8VwpYzGff3uctyCc", "Orca"},
        {"9W959DqEETiGZocYWCQPaJ6sBmUzgfxXfqGeTEdp3aQP", "Orca Whirlpools"},
        {"CAMMCzo5YL8w4VFF8KVHrK22GGUsp5VTaW7grrKgrWqK", "Raydium CLMM"},
        {"DjVE6JNiYqPL2QXyCUUh8rNjHrbz9hXHNYt99MQ59qw1", "Openbook"},
        {"BSfD6SHZigAfDWSjzD5Q41jw8LmKwtmjskPH9XW1mrRW", "Pump.fun"},
        // Serum v3 markets
        {"srmqPvymJeFKQ4zGQed1GFppgkRHL9kaELCbyksJtPX", "Serum v3"},
        {"9xQeWvG816bUx9EPjHmaT23yvVM2ZWbrrpZb9PusVFin", "Serum v3"},
        {"22Y43yTVxuUkoRKdm9thyRhQ3SdgQS7c7kB6UNCiaczD", "Serum v3"},
        {"EUqojwWA2rd19FZrzeBncJsm38Jm1hEhE3zsmX3bRc2o", "Serum v3"},
        {"BJ3jrUzddfuSrZHXSCxMUUQsjKEyLmuuyZebkcaFp2fg", "Serum v3"},
        
        // 其他DEX
        {"5cLrMai1DsLRYc1Nio9qMTicsWtvzjzZfJPXyAoF4t1Z", "Phoenix"},
        {"PhoeNiXZ8ByJGLkxNfZRnkUfjvmuYqLR89jjFHGqdXY", "Phoenix"},
        {"Dooar9JkhdZ7J3LHN3A7YCuoGRUggXhQaG4kijfLGU2j", "Dooar"},
        {"CURVGoZn8zycx6FXwwevgBTB2gVvdbGTEpvMJDbgs2t4", "Crema Finance"},
        {"MEVQUwww8q6E8JHghUkWqJXHxKGnvqwZQUZm5d3YR9F", "Mercurial"},
        {"SSwpkEEcbUqx4vtoEByFjSkhKdCT862DNVb52nZg1UZ", "Saber"},
        {"27haf8L6oxUeXrHrgEgsexjSY5hbVUWEmvv9Nyxg8vQv", "Raydium Legacy"},
        {"SwaPpA9LAaLfeLi3a68M4DjnLqgtticKg6CnyNwgAC8", "Raydium Legacy"},
        {"HebHBPyYGXqupCn4vYhz1Pxti2ZK8jFWQZBUgdUUTYE", "Invariant"},
        {"METAewgxyPbgwsseH8T16a39CQ5VyVxZi9zXiDPY18m", "Meteora"},
        {"FLUXubRmkEi2q6K3Y9kBPg9248ggaZVsoSFhtJHSrm1X", "FluxBeam"},
        {"GFXsSL5sSaDfNFQUYsHekbWBW1TsFdjDYzACh62tEHxn", "GooseFX"},
        {"CrAFTUv7zKXBaS5471aCwHx7mq9Xn8B6cNUwzPGodZ3f", "Cykura"},
        {"StepAscQoEioFxxWGnh2sLBDFp9d8rvKz2Yp39iDpyT", "Step Finance"},
        {"Zo1ggzTUKMY5bYnDvT5mtVeZxzf2FaLTbKkmvGUhUQk", "Zebec"},
        {"DystopianSwap11111111111111111111111111111", "Dystopian"},
        {"AURUqAkqUVbTBzKzAUTAVGiNWY6THaGZsHZfZuSNYYYr", "Aurory"},
        {"SWABtvDnJwWwAb9CbSA3nv7nTnrtYjrACAVtuP3gyBB", "Saros"},
        {"2wT8Yq49kHgDzXuPxZSaeLaH1qbmGXtEyPy64bL7aD3c", "Aldrin"},
        {"SWAPtMPAf9Dhg7RUwmcCwFsgxYN1d7jSyMqpvxpH98K", "Penguin"},
        {"DFpLFcQZqDKykyDePgip49jmee7bzGXhEjWpXryDUDxn", "Drift Protocol"},
        {"MarBmsSgKXdrN1egZf5sqe1TMai9K1rChYNDJgjq7aD", "Mango Markets"}
    };

            var instructions = transaction["transaction"]?["message"]?["instructions"] as JArray;

            if (instructions != null)
            {
                foreach (var instruction in instructions)
                {
                    string programId = instruction["programId"]?.ToString();
                    if (programIds.ContainsKey(programId))
                    {
                        // 检查是否是Pump.fun的买入/卖出操作
                        var logMessages = transaction["meta"]?["logMessages"] as JArray;
                        if (logMessages != null)
                        {
                            foreach (var log in logMessages)
                            {
                                string logStr = log.ToString();
                                if (logStr.Contains("Instruction: PumpBuy"))
                                    return "Pump.fun Buy";
                                if (logStr.Contains("Instruction: PumpSell"))
                                    return "Pump.fun Sell";
                            }
                        }
                        return programIds[programId];
                    }
                }
            }

            return null;
        } 
        public static async Task<string> GetTokenSymbol(string mintAddressx)
        {
            try
            {
                var rpcClient = ClientFactory.GetClient(Cluster.MainNet);
                var mintAddress = new PublicKey(mintAddressx);
                var metadataAddress = await MetadataAccount.GetAccount(rpcClient, mintAddress);

                return metadataAddress.metadata.symbol;

            }
            catch (Exception ex)
            {

                return mintAddressx;
            }
        }  
        public static void PlayBase64Audio(string base64Audio)
        {
            try
            {
                // 移除 data URI scheme 前缀
                string cleanBase64 = base64Audio;
                if (base64Audio.Contains(","))
                {
                    cleanBase64 = base64Audio.Split(',')[1];
                }

                byte[] audioBytes = Convert.FromBase64String(cleanBase64);

                using (var ms = new MemoryStream(audioBytes))
                using (var mp3Reader = new Mp3FileReader(ms))
                using (var waveOut = new WaveOutEvent())
                {
                    waveOut.Init(mp3Reader);
                    waveOut.Play();

                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }
                    waveOut.Stop();
                    waveOut?.Dispose();
                }
            }
            catch (Exception ex)
            {
                _Print("Logs",$"播放音频时出错: {ex.Message}");
            }
        }


    }
}
