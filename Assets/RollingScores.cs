using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets {
    public class RollingScores {
        public float span;
        private Dictionary<string, int> scores, rollingScores;
        private Dictionary<string, Deque<Tuple<int, float>>> events;
        private Dictionary<string, int> pendingEventCounts;

        public RollingScores(float span) {
            this.span = span;
            scores = new Dictionary<string, int>();
            rollingScores = new Dictionary<string, int>();
            events = new Dictionary<string, Deque<Tuple<int, float>>>();
            pendingEventCounts = new Dictionary<string, int>();
        }

        public bool Update(float time) {
            float bound = time - span;
            bool changed = false;
            foreach (var kvp in events) {
                while (!kvp.Value.IsEmpty && kvp.Value[0].Item2 < bound) {
                    Tuple<int, float> e = kvp.Value.RemoveFront();
                    rollingScores[kvp.Key] -= e.Item1;
                    changed = true;
                }
            }
            return changed;
        }
        public void Award(string viewer, int points, float time) {
            if (!scores.ContainsKey(viewer)) {
                scores.Add(viewer, 0);
            }
            if (!rollingScores.ContainsKey(viewer)) {
                rollingScores.Add(viewer, 0);
                events.Add(viewer, new Deque<Tuple<int, float>>());
            }
            scores[viewer] += points;
            rollingScores[viewer] += points;
            events[viewer].AddBack(new Tuple<int, float>(points, time));
            if (!pendingEventCounts.ContainsKey(viewer)) {
                pendingEventCounts.Add(viewer, 1);
            } else {
                pendingEventCounts[viewer]++;
            }
        }
        public void SetTotalScore(string viewer, int points) {
            scores[viewer] = points;
        }

        public int GetScore(string viewer) {
            return GetScore(viewer, true);
        }
        public int GetScore(string viewer, bool rolling) {
            if (!rollingScores.ContainsKey(viewer)) {
                return 0;
            }
            return rolling ? rollingScores[viewer] : scores[viewer];
        }
        public int NumViewers() {
            return scores.Count;
        }
        public bool SeenViewer(string viewer) {
            return scores.ContainsKey(viewer);
        }
        public List<KeyValuePair<string, int>> GetRollingScoresDescending(HashSet<string> subscribers) {
            return rollingScores.Where(x => x.Value > 0).OrderByDescending(x => x.Value).ThenByDescending(x => subscribers.Contains(x.Key)).ToList();
        }
        public Dictionary<string, int> GetTotalScores() {
            return scores;
        }

        public void FinalizeScores() {
            pendingEventCounts.Clear();
        }
        public void Rollback() {
            foreach (var kvp in pendingEventCounts) {
                for (int i = 0; i < kvp.Value; i++) {
                    Tuple<int, float> e = events[kvp.Key].RemoveBack();
                    scores[kvp.Key] -= e.Item1;
                    rollingScores[kvp.Key] -= e.Item1;
                }
            }
            pendingEventCounts.Clear();
        }
    }
}
