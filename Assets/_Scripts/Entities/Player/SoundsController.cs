using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsController : MonoBehaviour
{
    public AudioSource DashAudio;
    public AudioSource ArrowAudio;
    public AudioSource HitAudio;
    public AudioSource DeathAudio;
    public AudioSource SwordAudio;
    public AudioSource ThemeAudio;
    public AudioSource TeleportAudio;
    public AudioSource GameOverAudio;
    
    public void PlayDash() {
        DashAudio.Play();
    }

    public void PlayArrow() {
        ArrowAudio.Play();
    }

    public void PlayHit() {
        HitAudio.Play();
    }

    public void PlayDeath() {
        ThemeAudio.volume = 0;
        DeathAudio.Play();
    }

    public void PlayGameOver() {
        GameOverAudio.Play();
        ThemeAudio.volume = 0;
    }

    public void PlaySword() {
        SwordAudio.Play();
    }

    public void PlayTeleport() {
        TeleportAudio.Play();
    }

    public void PlayTheme() {
        DeathAudio.Stop();
        GameOverAudio.Stop();
        ThemeAudio.volume = 0.3f;
    }
}
