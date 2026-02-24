using RAK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAK
{
    public static class AudioMan
    {
        public static PrefabT<AudioSource> audioFab;

        public static void EnsureFab()
        {
            if (audioFab == null || audioFab.Reference == null)
            {
                audioFab = new PrefabT<AudioSource>();
                GameObject go = new GameObject("AudioFab");
                GameObject.DontDestroyOnLoad(go);
                go.SetActive(false);
                AudioSource asource = go.AddComponent<AudioSource>();
                audioFab.SetReference(asource);
                go.SetActive(false);
            }
        }
        public static AudioSource PlayClip(AudioClip clip, Vector3 position = new Vector3(), float volume=1, Action<AudioSource> onPreDestroy = null)
        {
            if (!GlobalToggle.IsEnabled(GlobalToggleType.Sound)) return null;
            EnsureFab();
            AudioSource source = audioFab.Instantiate_T(position, Quaternion.identity, null);
            source.gameObject.SetActive(true);
            source.gameObject.name = $"Audio Fab Clip: {clip.name} (Pooled)";
            source.volume = volume;
            source.clip = clip;
            source.Play();
            Centralizer.Add_DelayedAct(() =>
            {
                source.clip = null;
                onPreDestroy?.Invoke(source);
                Pool.Destroy(source.gameObject);
            }, clip.length);
            return source;
        }
    }
}