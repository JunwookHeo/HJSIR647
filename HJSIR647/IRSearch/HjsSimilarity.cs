//#define _DEBUGGING_

using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.IRSearch
{
    class HjsSimilarity : DefaultSimilarity
    {
        //
        // Summary:
        //     Implemented as state.getBoost()*lengthNorm(numTerms), where numTerms is Lucene.Net.Index.FieldInvertState.Length
        //     if Lucene.Net.Search.DefaultSimilarity.DiscountOverlaps is false, else it's Lucene.Net.Index.FieldInvertState.Length
        //     - Lucene.Net.Index.FieldInvertState.NumOverlap . WARNING: This API is new and
        //     experimental, and may suddenly change.
        public override float ComputeNorm(string field, FieldInvertState state)
        {
            float v = base.ComputeNorm(field, state);
            DebugMsg($"=====> ComputeNorm : field({field}), state({state}), out({v})");
            return v;
        }
        //
        // Summary:
        //     Implemented as overlap / maxOverlap.
        public override float Coord(int overlap, int maxOverlap)
        {
            float v = base.Coord(overlap, maxOverlap);
            DebugMsg($"=====> Coord : overlap({overlap}), maxOverlap({maxOverlap}),  out({v})");
            return v;
        }
        //
        // Summary:
        //     Implemented as log(numDocs/(docFreq+1)) + 1.
        public override float Idf(int docFreq, int numDocs)
        {
            float v = base.Idf(docFreq, numDocs);
            DebugMsg($"=====> Idf : docFreq({docFreq}), numDocs({numDocs}), out({v})");
            return v;
        }
        //
        // Summary:
        //     Implemented as 1/sqrt(numTerms).
        public override float LengthNorm(string fieldName, int numTerms)
        {
            float v = base.LengthNorm(fieldName, numTerms);
            DebugMsg($"=====> LengthNorm : fieldName({fieldName}), numTerms({numTerms}), out({v})");
            return v;
        }
        //
        // Summary:
        //     Implemented as 1/sqrt(sumOfSquaredWeights).
        public override float QueryNorm(float sumOfSquaredWeights)
        {
            float v = base.QueryNorm(sumOfSquaredWeights);
            DebugMsg($"=====> QueryNorm : sumOfSquaredWeights({sumOfSquaredWeights}), out({v})");
            return v;
        }
        //
        // Summary:
        //     Implemented as 1 / (distance + 1).
        public override float SloppyFreq(int distance)
        {
            float v = base.SloppyFreq(distance);
            DebugMsg($"=====> SloppyFreq : distance({distance}), out({v})");
            return v;
        }
        //
        // Summary:
        //     Implemented as sqrt(freq).
        public override float Tf(float freq)
        {
            float v = base.Tf(freq);
            DebugMsg($"=====> Tf : freq({freq}), out({v})");
            if (freq > 0f)
            {
                v = (float)Math.Log10((double)freq);
                v += 1;

            }
            else
                v = 0;

            DebugMsg($"=====> Tf modeify: freq({freq}), out({v})");
            return v;
        }

        void DebugMsg(string fmt, params object[] args)
        {
#if (_DEBUGGING_)
            Console.WriteLine(fmt, args);
#endif
        }
    }
}
