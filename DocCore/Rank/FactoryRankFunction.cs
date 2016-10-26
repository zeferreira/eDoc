using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class FactoryRankFunction
    {
        public static IRankFunction GetRankFunction()
        {
            EngineConfiguration engConf = EngineConfiguration.Instance;

            string type = engConf.RankTypeFunction;

            switch (type)
            {
                case "simple-tf-idf":
                    return RankFunctionSimpleTF_IDF.Instance;

                case "bm25":
                    return RankFunctionSimpleTF_IDF.Instance;

                case "bm25_okapi":
                    return RankFunctionBM25_Okapi.Instance;

                case "pivoted_len_norm_vsm":
                    return RankFunctionPivotedLengthNormVSM.Instance;

                default:
                    throw new NotImplementedException(Messages.RankedFunctionNotImplemented);

            }
 
        }
    }
}
