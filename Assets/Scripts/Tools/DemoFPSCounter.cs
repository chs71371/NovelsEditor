// Amplify Bloom - Advanced Bloom Post-Effect for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEngine.UI;
namespace AmplifyBloom
{
	public class DemoFPSCounter : MonoBehaviour
	{
		public float UpdateInterval = 0.5F;
		private Text m_fpsText;

		private float m_accum = 0; // FPS accumulated over the interval
		private int m_frames = 0; // Frames drawn over the interval
		private float m_timeleft; // Left time for current interval
		private float m_fps;
		private string m_format;

		void Start()
		{
			m_fpsText = GetComponent<Text>();
			m_timeleft = UpdateInterval;
		}

		void Update()
		{
			m_timeleft -= Time.deltaTime;
			m_accum += Time.timeScale / Time.deltaTime;
			++m_frames;
			if ( m_timeleft <= 0.0 )
			{
				m_fps = m_accum / m_frames;
				m_format = System.String.Format( "{0:F2} FPS", m_fps );
				m_fpsText.text = m_format;

				if ( m_fps < 50 )
					m_fpsText.color = Color.yellow;
				else
					if ( m_fps < 30 )
					m_fpsText.color = Color.red;
				else
					m_fpsText.color = Color.green;

				m_timeleft = UpdateInterval;
				m_accum = 0.0F;
				m_frames = 0;
			}
		}
	}
}