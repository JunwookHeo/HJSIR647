using HJSIR647.Set;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.IRSearch
{
    class HjsStandardAnalyzer : StandardAnalyzer
    {        
        protected Lucene.Net.Util.Version VERSION;
        private ISet<string> StopSet;
        private bool enableSPI;
        public HjsStandardAnalyzer(Lucene.Net.Util.Version matchVersion, ISet<string> sws)
            : base(matchVersion, sws)
        {
            VERSION = matchVersion;
            StopSet = sws;
            enableSPI = StopFilter.GetEnablePositionIncrementsVersionDefault(matchVersion);

        }
        override public TokenStream TokenStream(string fieldName, TextReader reader)
        {
            StandardTokenizer tokenStream = new StandardTokenizer(VERSION, reader);
            tokenStream.MaxTokenLength = DEFAULT_MAX_TOKEN_LENGTH;
            TokenStream result = new StandardFilter(tokenStream);
            result = new LowerCaseFilter(result);

            if (SettingsViewModel.Instance.StopWords == true)
            {
                result = new StopFilter(enableSPI, result, StopSet);
            }

            if (SettingsViewModel.Instance.Stemming == true)
            {
                result = new PorterStemFilter(result);
            }
            
            return result;
        }
        
    }
}
