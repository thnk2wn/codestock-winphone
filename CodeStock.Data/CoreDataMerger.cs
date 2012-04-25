using System;
using System.Linq;
using CodeStock.Data.ServiceAccess;
using Phone.Common.Diagnostics;
using Phone.Common.Diagnostics.Logging;

namespace CodeStock.Data
{
    public class CoreDataMerger
    {
        public void Merge(ISessionsService sessionsService, ISpeakersService speakersService)
        {
            if (null == sessionsService || null == speakersService)
                throw new NullReferenceException("Sessions and speakers services must both be set");

            var timer = CodeTimer.StartNew();

            if (null != sessionsService.Data)
            {
                sessionsService.Data.ToList().ForEach(s =>
                {
                    s.Speaker = speakersService.Data.Where(
                            sp => sp.SpeakerId == s.SpeakerId).FirstOrDefault();

                    if (null == s.Speaker)
                    {
                        // this can possibly happen if a new speaker was added and the speaker data is cached longer than the session data
                        // and session includes the new speaker that wasn't there at the point the speaker data was last cached
                        LogInstance.LogWarning("WARNING: No speaker found with speaker id {0} for session {1}. Speaker data may need to be refreshed", 
                            s.SpeakerId, s.Title);
                    }
                });
            }

            // we don't need to load sessions for speaker anymore upfront; we delay load it later only if/when needed

            
            // 2.316 seconds roughly doing both. .031 seconds with skipping sessions for each speaker
            LogInstance.LogInfo("Session / speaker data merged in {0:##.000} seconds", timer.Stop());
            
        }
    }
}
