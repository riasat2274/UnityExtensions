
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace RAK
{
    public class AudioLibrary : MonoBehaviour
    {
        public static Dictionary<string, ClipSetInfo> allSounds;
        public List<ClipSetInfo> soundInfos;
        private void Awake()
        {
            if(allSounds == null)
            {
                allSounds = new();
                foreach (var item in soundInfos)
                {
                    allSounds.Add(item.key, item);
                }
            }
        }

        public static AudioClipInfo GetClip(string key, int clipIndex = -1)
        {
            if (allSounds == null) return null;
            if (!allSounds.ContainsKey(key)) return null;
            return allSounds[key].GetClip(clipIndex); 
        }

        

        public static void PlaySound(string key, int clipIndex = -1)
        {
            if(allSounds==null) return;
            if(!allSounds.ContainsKey(key)) return;
            allSounds[key].Play(Vector3.zero,1, clipIndex);
        }
        public static void PlaySoundAt(string key, Vector3 position, int clipIndex = -1)
        {
            if (allSounds == null) return;
            if (!allSounds.ContainsKey(key)) return;
            allSounds[key].Play(position,1, clipIndex);
        }
        public static void PlaySoundWithVolumeMultiplier(string key,float volumeMultiplier, int clipIndex = -1)
        {
            if (allSounds == null) return;
            if (!allSounds.ContainsKey(key)) return;
            allSounds[key].Play(Vector3.zero, volumeMultiplier, clipIndex);
        }





    }
    public class OverPlayVolumeController
    {

        public List<AudioNerfableSource> sources = new();
        float nerfFactor = .5f;
        public OverPlayVolumeController(float nerfFactor) 
        { 
            this.nerfFactor = nerfFactor;
        }

        public void Play(string key, int clipIndex=-1)
        {
            AudioClipInfo clipInfo = AudioLibrary.GetClip(key, clipIndex);
            AudioSource asource = clipInfo.DirectPlay(Vector3.zero, 1);
            if(asource==null) return;
            foreach (var item in sources)
            {
                item.Nerf(nerfFactor);
            }
            AudioNerfableSource anf = new AudioNerfableSource(asource);
            sources.Add(anf);
            Centralizer.Add_DelayedAct(() => {
                sources.Remove(anf);
            }, clipInfo.clip.length);

        }
        public class AudioNerfableSource
        {
            public float startVolume;
            AudioSource source;
            public AudioNerfableSource(AudioSource source)
            {
                this.source = source;
                startVolume = source.volume;
            }
            public void Nerf(float nerfFactor)
            { 
                source.volume *= nerfFactor;
            }
        }
    }


    [System.Serializable]
    public class ClipSetInfo
    {
        public string key;
        public float groupDelay;
        public List<AudioClipInfo> clipList;
        public AudioClipInfo GetClip(int clipIndex = -1)
        {
            if (clipIndex < 0 || clipIndex >= clipList.Count)
            {
                return clipList.RandomItem();
            }
            else
            {
                return clipList[clipIndex];
            }
        }
        public void Play(Vector3 pos,float volumeMultiplier,int clipIndex = -1)
        {
            AudioClipInfo cinfo = GetClip(clipIndex);
            float totalDelay = groupDelay + cinfo.specificDelay;
            if (totalDelay <= 0)
            {
                cinfo.DirectPlay(pos,volumeMultiplier);
            }
            else
            {
                Centralizer.Add_DelayedAct(() => {
                    cinfo.DirectPlay(pos, volumeMultiplier);
                }, totalDelay);
            }

        }
    }
    [System.Serializable]
    public class AudioClipInfo
    {
        public AudioClip clip;
        public float specificDelay;
        [Range(0, 1)]
        public float volume = 1;
        public AudioSource DirectPlay(Vector3 pos, float volumeMultiplier)
        {
           return AudioMan.PlayClip(clip, pos, volume* volumeMultiplier);
        }

#if UNITY_EDITOR
        [UnityEditor.CustomPropertyDrawer(typeof(AudioClipInfo))]
        public class AudioClipInfoDrawer : LinePropertyDrawer
        {
            public override bool IsExpandible => false;
            public override int AdditionalRowCount => 1;
            protected override void OnGUICore()
            {
                base.OnGUICore();
                AddField("clip", .3f);
                AddField("volume", .3f);
                AddLabel("Delay:", .15f);
                AddField("specificDelay", .25f);
            }
        }
#endif
    }
}
