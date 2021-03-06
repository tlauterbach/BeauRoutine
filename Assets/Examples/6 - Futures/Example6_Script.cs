﻿using System.Collections;
using BeauRoutine;
using UnityEngine;
using UnityEngine.UI;

namespace BeauRoutine.Examples
{
    public class Example6_Script : MonoBehaviour
    {
        [SerializeField]
        private Example6_Service m_Service = null;

        [Header("Reverse Strings")]

        [SerializeField]
        private InputField m_ReverseBoxInput = null;

        [SerializeField]
        private Button m_ReverseBoxButton = null;

        [SerializeField]
        private Text m_ReverseBoxOutput = null;

        [Header("Parse Color")]

        [SerializeField]
        private InputField m_ColorBoxInput = null;

        [SerializeField]
        private Button m_ColorBoxButton = null;

        [SerializeField]
        private Image m_ColorBoxOutput = null;

        [Header("Download Text")]

        [SerializeField]
        private InputField m_URLBoxInput = null;

        [SerializeField]
        private Button m_URLBoxButton = null;

        [SerializeField]
        private Text m_URLBoxOutput = null;

        [Header("Download Texture")]

        [SerializeField]
        private InputField m_URLImageBoxInput = null;

        [SerializeField]
        private Button m_URLImageBoxButton = null;

        [SerializeField]
        private RawImage m_URLImageBoxOutput = null;

        [Header("Download Audio")]

        [SerializeField]
        private InputField m_URLAudioBoxInput = null;

        [SerializeField]
        private Button m_URLAudioBoxButton = null;

        [SerializeField]
        private AudioSource m_URLAudioBoxOutput = null;

        private void Start()
        {
            m_ReverseBoxButton.onClick.AddListener(OnClickReverseButton);
            m_ColorBoxButton.onClick.AddListener(OnClickColorButton);
            m_URLBoxButton.onClick.AddListener(OnClickURLButton);
            m_URLImageBoxButton.onClick.AddListener(OnClickURLImageButton);
            m_URLAudioBoxButton.onClick.AddListener(OnClickURLAudioButton);
        }

        private void OnClickReverseButton()
        {
            Routine.Start(WaitForReversedFuture());
        }

        private void OnClickColorButton()
        {
            // You can set up complete and failure callbacks for a Future.
            // In this manner you don't have to create a coroutine to
            // wait for the future to complete.
            var future = m_Service.ParseColor(m_ColorBoxInput.text)
                .OnComplete((c) =>
                {
                    m_ColorBoxOutput.color = c;
                    m_ColorBoxInput.textComponent.color = Color.black;
                })
                .OnFail(() =>
                {
                    m_ColorBoxOutput.color = Color.clear;
                    m_ColorBoxInput.textComponent.color = Color.red;
                });

            future.OnProgress((f) => Debug.Log(future));

            Routine.Start(WaitForFutureToComplete(future));
        }

        private void OnClickURLButton()
        {
            // You can set up complete and failure callbacks for a Future.
            // In this manner you don't have to create a coroutine to
            // wait for the future to complete.
            var future = m_Service.ParseURL(m_URLBoxInput.text)
                .OnComplete((text) =>
                {
                    m_URLBoxOutput.text = text;
                    m_URLBoxInput.textComponent.color = Color.black;
                })
                .OnFail((o) =>
                {
                    m_URLBoxOutput.text = o.ToString();
                    m_URLBoxInput.textComponent.color = Color.red;
                });

            future.OnProgress((f) => Debug.Log(future));

            Routine.Start(WaitForFutureToComplete(future));
        }

        private void OnClickURLImageButton()
        {
            var future = m_Service.ParseURLTexture(m_URLImageBoxInput.text)
                .OnComplete((texture) =>
                {
                    Texture oldTexture = m_URLImageBoxOutput.texture;
                    m_URLImageBoxOutput.texture = texture;
                    Destroy(oldTexture);

                    m_URLImageBoxOutput.transform.SetScale((float)texture.width / texture.height, Axis.X);

                    m_URLImageBoxInput.textComponent.color = Color.black;
                })
                .OnFail((o) =>
                {
                    Debug.Log("Image download failed: " + o.ToString());
                    m_URLImageBoxInput.textComponent.color = Color.red;
                });

            future.OnProgress((f) => Debug.Log(future));

            Routine.Start(WaitForFutureToComplete(future));
        }

        private void OnClickURLAudioButton()
        {
            var future = Future.Download.AudioClip(m_URLAudioBoxInput.text, true)
                .OnComplete((clip) =>
                {
                    m_URLAudioBoxOutput.Stop();

                    AudioClip oldClip = m_URLAudioBoxOutput.clip;
                    m_URLAudioBoxOutput.clip = clip;
                    Destroy(oldClip);

                    m_URLAudioBoxOutput.loop = true;
                    m_URLAudioBoxOutput.Play();

                    m_URLAudioBoxInput.textComponent.color = Color.black;
                })
                .OnFail((o) =>
                {
                    m_URLAudioBoxOutput.Stop();

                    Debug.Log("Audio download failed: " + o.ToString());
                    m_URLAudioBoxInput.textComponent.color = Color.red;
                });

            future.OnProgress((f) => Debug.Log(future));

            Routine.Start(WaitForFutureToComplete(future));
        }

        private IEnumerator WaitForReversedFuture()
        {
            var future = m_Service.ReverseString(m_ReverseBoxInput.text);
            future.OnProgress((f) => Debug.Log(future));

            Routine.Start(WaitForFutureToComplete(future));

            // You can yield a Future to wait for it to complete or fail.
            yield return future;
            if (future.IsComplete())
            {
                // You can implicitly cast a future to its return type.
                // Be careful, though - this will throw an exception if
                // the future has not been completed.
                m_ReverseBoxOutput.text = future;
            }
            else
            {
                m_ReverseBoxOutput.text = "[Something failed]";
            }
        }

        private IEnumerator WaitForFutureToComplete(IFuture inFuture)
        {
            Debug.Log("Future is running!");

            yield return inFuture;

            if (inFuture.IsComplete())
                Debug.Log("Future is complete!");
            else if (inFuture.IsFailed())
                Debug.Log("Future has failed!");
        }
    }
}