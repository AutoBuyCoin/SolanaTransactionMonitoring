
using System.Speech.Synthesis;

public class TextToSpeechPlayer
{
    /// <summary>
    /// 异步播放文字并自动释放资源
    /// </summary>
    /// <param name="text">要播放的文字</param>
    /// <returns>Task</returns>
    /// 
     public  TextToSpeechPlayer()
        {

            using (var synthesizer = new SpeechSynthesizer())
            {
                // 检测是否有可用的 TTS 语音包
                if (!synthesizer.GetInstalledVoices().Any(voice => voice.Enabled))
                {
                    throw new InvalidOperationException("未检测到系统可用的 TTS 语音包，请安装语音包后重试。");
                } 
            }
        }
    public static Task PlayTextAsync(string text)
    {
        return Task.Run(() =>
        {
            using (var synthesizer = new SpeechSynthesizer())
            {
                // 检测是否有可用的 TTS 语音包
                if (!synthesizer.GetInstalledVoices().Any(voice => voice.Enabled))
                {
                    throw new InvalidOperationException("未检测到系统可用的 TTS 语音包，请安装语音包后重试。");
                }

                synthesizer.Rate = 5;     // 设置语速
                synthesizer.Volume = 100; // 设置音量

                var completionEvent = new ManualResetEvent(false);
                synthesizer.SpeakCompleted += (sender, e) =>
                {
                    completionEvent.Set();
                };
                synthesizer.SpeakAsync(text);
                completionEvent.WaitOne();
            }
        });
    }
}
