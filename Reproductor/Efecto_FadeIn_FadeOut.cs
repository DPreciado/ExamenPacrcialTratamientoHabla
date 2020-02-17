using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace Reproductor
{
    class Efecto_FadeIn_FadeOut : ISampleProvider
    {
        private ISampleProvider fuente;
        private int muestrasLeidas = 0;
        private float segundosTranscurridos = 0;
        private float duracionIn;
        private float inicioOut;
        private float duracionOut;

        public Efecto_FadeIn_FadeOut(ISampleProvider fuente,
            float duracionIn, float inicioOut, float duracionOut)
        {
            this.fuente = fuente;
            this.duracionIn = duracionIn;
            this.inicioOut = inicioOut;
            this.duracionOut = duracionOut;
        }

        public WaveFormat WaveFormat
        {
            get
            {
                return fuente.WaveFormat;
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = fuente.Read(buffer, offset, count);


            muestrasLeidas += read;
            segundosTranscurridos =
                (float)muestrasLeidas /
                (float)fuente.WaveFormat.SampleRate /
                (float)fuente.WaveFormat.Channels;

            if (segundosTranscurridos <= duracionIn)
            {
                //Aplicar el efecto
                float factorEscala =
                    segundosTranscurridos /
                        duracionIn;
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] *=
                        factorEscala;
                }
            }

            if (segundosTranscurridos >= inicioOut &&
                segundosTranscurridos <= inicioOut + duracionOut)
            {
                //Aplicar el efecto
                float factorEscala =
                    1 - ((segundosTranscurridos - inicioOut) /
                        duracionOut);
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] *=
                        factorEscala;
                }
            }
            else if (segundosTranscurridos >= inicioOut + duracionOut)
            {
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] = 0.0f;
                }
            }

            return read;
        }
    }
}
