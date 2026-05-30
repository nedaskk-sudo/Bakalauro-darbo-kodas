using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Unity.VRTemplate
{
    /// <summary>
    /// Connects a UI slider control to a video player, allowing users to scrub to a particular time in th video.
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoTimeScrubControl : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Video play/pause button GameObject")]
        GameObject m_ButtonPlayOrPause;

        [SerializeField]
        [Tooltip("Slider that controls the video")]
        Slider m_Slider;

        [SerializeField]
        [Tooltip("Play icon sprite")]
        Sprite m_IconPlay;

        [SerializeField]
        [Tooltip("Pause icon sprite")]
        Sprite m_IconPause;

        [SerializeField]
        [Tooltip("Play or pause button image.")]
        Image m_ButtonPlayOrPauseIcon;

        [SerializeField]
        [Tooltip("Text that displays the current time of the video.")]
        TextMeshProUGUI m_VideoTimeText;

        [SerializeField]
        [Tooltip("If checked, the slider will fade off after a few seconds. If unchecked, the slider will remain on.")]
        bool m_HideSliderAfterFewSeconds;

        [SerializeField]
        [Tooltip("First button to show after video finishes")]
        GameObject m_EndButtondesine;

        [SerializeField]
        [Tooltip("Second button to show after video finishes")]
        GameObject m_EndButtonkaire;

        [SerializeField]
        [Tooltip("Second button to show after video finishes")]
        GameObject dabartinisUI;

        [SerializeField]
        [Tooltip("Second button to show after video finishes")]
        GameObject SekantisUIdesine;

        [SerializeField]
        [Tooltip("Time in seconds when choice buttons should appear")]
        float m_ShowButtonsAtTime = 15f;

        [SerializeField]
        GameObject SekantisUIIkaire;

        bool m_IsDragging;
        bool m_VideoIsPlaying;
        bool m_VideoJumpPending;
        long m_LastFrameBeforeScrub;
        bool m_EndButtonsShown;
        bool m_VideoEnded;
        //float m_PlayTimeCounter;

        //Coroutine m_EndButtonsCoroutine;

        VideoPlayer m_VideoPlayer;

        void Start()
        {
            m_VideoPlayer = GetComponent<VideoPlayer>();
            m_VideoPlayer.loopPointReached += OnVideoFinished;
            //m_VideoPlayer.frameReady += OnVideoFrameReady;

            if (m_EndButtondesine != null) m_EndButtondesine.SetActive(false);
            if (m_EndButtonkaire != null) m_EndButtonkaire.SetActive(false);


            if (!m_VideoPlayer.playOnAwake)
            {
                //m_VideoPlayer.playOnAwake = false; // Set play on awake for next enable.
                m_VideoPlayer.Play(); // Play video to load first frame.
                VideoStop(); // Stop the video to set correct state and pause frame.
            }
            else
            {
                VideoPlay(); // Play to ensure correct state.
            }

            if (m_ButtonPlayOrPause != null)
                m_ButtonPlayOrPause.SetActive(false);
        }

        void OnEnable()
        {
            m_EndButtonsShown = false;
            //m_PlayTimeCounter = 0f;
            m_VideoEnded = false;

            if (m_EndButtondesine != null) m_EndButtondesine.SetActive(false);
            if (m_EndButtonkaire != null) m_EndButtonkaire.SetActive(false);


            if (m_VideoPlayer != null)
            {
                m_VideoPlayer.frame = 0;
                VideoPlay(); // Ensures correct UI state update if paused.
            }

            m_Slider.value = 0.0f;
            m_Slider.onValueChanged.AddListener(OnSliderValueChange);
            m_Slider.gameObject.SetActive(true);
            if (m_HideSliderAfterFewSeconds)
                StartCoroutine(HideSliderAfterSeconds());
        }

        void Update()
        {
            if (m_VideoJumpPending)
            {
                // We're trying to jump to a new position, but we're checking to make sure the video player is updated to our new jump frame.
                if (m_LastFrameBeforeScrub == m_VideoPlayer.frame)
                    return;

                // If the video player has been updated with desired jump frame, reset these values.
                m_LastFrameBeforeScrub = long.MinValue;
                m_VideoJumpPending = false;
            }

            if (!m_IsDragging && !m_VideoJumpPending)
            {
                if (m_VideoPlayer.frameCount > 0)
                {
                    var progress = (float)m_VideoPlayer.frame / m_VideoPlayer.frameCount;
                    m_Slider.value = progress;
                }
            }

            // Show buttons at chosen video time
            //# Šis kodas buvo sugeneruotas naudojant ChatGPT (GPT-4.5, 2026-05-24).
            //# Užklausa: "heres code for a video player. I want for the choice buttons to apear at a certain point in the video, make the code work" 
            //# Rezultatas dalinai koreguotas.

            if (!m_EndButtonsShown && m_VideoPlayer.time >= m_ShowButtonsAtTime)
            {
                m_EndButtonsShown = true;

                if (m_EndButtondesine != null)
                    m_EndButtondesine.SetActive(true);

                if (m_EndButtonkaire != null)
                    m_EndButtonkaire.SetActive(true);
            }
        }

      

        void OnVideoFinished(VideoPlayer vp)
        {
            m_VideoEnded = true;
            m_VideoIsPlaying = false;

            if (m_EndButtondesine != null)
                m_EndButtondesine.SetActive(true);

            if (m_EndButtonkaire != null)
                m_EndButtonkaire.SetActive(true);

            if (m_ButtonPlayOrPause != null)
                m_ButtonPlayOrPause.SetActive(false);
        }


        public void OnPointerDown()
        {
            m_VideoJumpPending = true;
            VideoStop();
            VideoJump();
        }

        public void OnRelease()
        {
            m_IsDragging = false;
            VideoPlay();
            VideoJump();
        }

        void OnSliderValueChange(float sliderValue)
        {
            UpdateVideoTimeText();
        }

        IEnumerator HideSliderAfterSeconds(float duration = 1f)
        {
            yield return new WaitForSeconds(duration);
            m_Slider.gameObject.SetActive(false);
        }

        public void OnDrag()
        {
            m_IsDragging = true;
            m_VideoJumpPending = true;
        }

        void VideoJump()
        {
            m_VideoJumpPending = true;
            var frame = m_VideoPlayer.frameCount * m_Slider.value;
            m_LastFrameBeforeScrub = m_VideoPlayer.frame;
            m_VideoPlayer.frame = (long)frame;
        }

        public void PlayOrPauseVideo()
        {
            if (m_VideoEnded)
                return;

            if (m_VideoIsPlaying)
            {
                VideoStop();
            }
            else
            {
                VideoPlay();
            }
        }

        void UpdateVideoTimeText()
        {
            if (m_VideoPlayer != null && m_VideoTimeText != null)
            {
                var currentTimeTimeSpan = TimeSpan.FromSeconds(m_VideoPlayer.time);
                var totalTimeTimeSpan = TimeSpan.FromSeconds(m_VideoPlayer.length);
                var currentTimeString = string.Format("{0:D2}:{1:D2}",
                    currentTimeTimeSpan.Minutes,
                    currentTimeTimeSpan.Seconds
                );

                var totalTimeString = string.Format("{0:D2}:{1:D2}",
                    totalTimeTimeSpan.Minutes,
                    totalTimeTimeSpan.Seconds
                );
                m_VideoTimeText.SetText(currentTimeString + " / " + totalTimeString);
            }
        }

        void VideoStop()
        {
            m_VideoIsPlaying = false;
            m_VideoPlayer.Pause();
            m_ButtonPlayOrPauseIcon.sprite = m_IconPlay;
            m_ButtonPlayOrPause.SetActive(true);
            //m_EndButtonsCoroutine = null;

        }

        void VideoPlay()
        {
            m_VideoIsPlaying = true;
            m_VideoPlayer.Play();
            m_ButtonPlayOrPauseIcon.sprite = m_IconPause;
            m_ButtonPlayOrPause.SetActive(false);
            //if (!m_EndButtonsShown && m_EndButtonsCoroutine == null)
            //    m_EndButtonsCoroutine = StartCoroutine(ShowEndButtonsAfter15Seconds());


        }

        public void OnButtonEnddesine()
        {
            if (dabartinisUI != null)
                dabartinisUI.SetActive(false);

            if (SekantisUIdesine != null)
                SekantisUIdesine.SetActive(true);
        }

        public void OnButtonEndkaire()
        {
            if (dabartinisUI != null)
                dabartinisUI.SetActive(false);

            if (SekantisUIIkaire != null)
                SekantisUIIkaire.SetActive(true);
        }

    }
}
