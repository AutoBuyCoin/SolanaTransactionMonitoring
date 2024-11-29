自动监控Solana链上指定地址的Token交易.语音提醒交易.

程序功能:
实时监控指定SOL地址的Token交易情况。并语音提醒。让你干其它事的时候不错过金狗
后续加入防夹买入Token功能。

在程序目录里token.txt里按格式添加需要监视的聪明钱包地址
格式： 
名称,钱包地址
例：
```INI
大聪明,CtyKix5CpXfBpaco1nftQsavBzgbEpxpM8qm25Scj7i2
金狗1号,71CPXu3TvH3iUKaY1bNkAAow24k6tjH473SsKprQBABC
```
注意：添加多个钱包时名称不能相同。逗号用半角(,)而不是(，)
 
配置文件CONFIG.INI
```INI
[CONFIG]
金狗提示阈值=10
WWS服务器地址=wss://api.mainnet-beta.solana.com
RPC服务器地址=https://solana-rpc.publicnode.com
是否保存日志=yes
播放详细交易=yes
```
可以自己搭建SOL节点服务器，可以获取更及时更稳定的推送  
设置金狗提醒价格(买入或卖出大于10SOL提醒) 
播放详细的交易 ：xxx买入xxx个xxx,价格xxx用了xxx个sol.
如果播放详细交易=no 只播放金狗买入与卖出提醒。不会播放详细内容

