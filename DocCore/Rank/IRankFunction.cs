using System;
namespace DocCore
{
    public interface IRankFunction
    {
        double CalcRankFactor(WordOccurrenceNode occ, Query query);
    }
}
