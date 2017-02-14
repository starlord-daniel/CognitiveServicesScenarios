using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;

namespace ImageDescription.TextToSpeech
{
    class Voice
    {
        public VoiceLanguage voiceLanguage;

        public VoiceGender voiceGender;

        public Voice(VoiceGender voiceGender, VoiceLanguage voiceLanguage)
        {
            this.voiceGender = voiceGender;
            this.voiceLanguage = voiceLanguage;
        }
    }

    public enum VoiceLanguage
    {
        English = 0,
        German = 1
    }

    public enum VoiceGender
    {
        Female = 0,
        Male = 1
    }
}
