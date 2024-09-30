using Yaong_Omok_Server;

public class MatchMaking {
    public class Player {
        public Client client;
        public int mmr;
        public int winPoint;
        public int defeatPoint;

        public Player(Client client) {
            this.client = client;
            this.mmr = client.userInfo.mmr;
            this.winPoint = 0;
            this.defeatPoint = 0;
        }
    }

    public static void Match(List<Client> clients) {
        if(clients.Count == 0) return;

        Player[] players = clients.Select(client => new Player(client)).ToArray();
        MatchPlayers(players);
    }

    private static void MatchPlayers(Player[] players) {
        List<Player> unmatchedPlayers = new List<Player>(players);
        int mmrLimit = 100;

        while(unmatchedPlayers.Count > 0) {

            double kFactor = AdjustKFactorBasedOnMMRRange(mmrLimit);
            var matchedPairs = FindBestMatching(unmatchedPlayers.ToArray(), mmrLimit, out unmatchedPlayers);

            if(matchedPairs.Count > 0) {
                mmrLimit = 100;
            }

            foreach(var match in matchedPairs) {
                Player playerA = match.Item1;
                Player playerB = match.Item2;

                CalculateMMR(playerA, playerB, kFactor);
                PrintMatchResult(playerA, playerB);
            }

            if(mmrLimit >= 1000) {
                break;
            }

            if(unmatchedPlayers.Count > 0) {
                mmrLimit += 30; // MMR 범위를 점진적으로 넓힘
            }
        }

        if(unmatchedPlayers.Count == 0) {
            Console.WriteLine("All players have been matched.");
        }
        else {
            foreach(var player in unmatchedPlayers) {
                Console.WriteLine($"Player with MMR {player.mmr} is unmatched.");
            }
        }
    }

    // MMR 범위에 따라 kFactor 값을 조정
    private static double AdjustKFactorBasedOnMMRRange(int mmrRange) {
        return Math.Max(15, 30 - (mmrRange / 500.0 * 15));
    }

    private static void CalculateMMR(Player playerA, Player playerB, double kFactor) {
        double expectedScoreA = 1.0 / (1.0 + Math.Pow(10, (playerB.mmr - playerA.mmr) / 400.0));
        double expectedScoreB = 1.0 / (1.0 + Math.Pow(10, (playerA.mmr - playerB.mmr) / 400.0));

        int winPointA = Math.Max((int)(kFactor * (1 - expectedScoreA)), 1);
        int winPointB = Math.Max((int)(kFactor * (1 - expectedScoreB)), 1);

        int defeatPointA = Math.Min((int)(-kFactor * expectedScoreA), -1);
        int defeatPointB = Math.Min((int)(-kFactor * expectedScoreB), -1);

        int totalScoreA = winPointA - defeatPointA;
        int totalScoreB = winPointB - defeatPointB;

        if(totalScoreA != 30) {
            int difference = 30 - totalScoreA;
            winPointA += difference / 2;
            defeatPointA -= difference / 2;
        }

        if(totalScoreB != 30) {
            int difference = 30 - totalScoreB;
            winPointB += difference / 2;
            defeatPointB -= difference / 2;
        }

        playerA.winPoint = winPointA;
        playerA.defeatPoint = defeatPointA;
        playerB.winPoint = winPointB;
        playerB.defeatPoint = defeatPointB;
    }

    private static void PrintMatchResult(Player playerA, Player playerB) {
        Console.WriteLine($"[{playerA.mmr}] and [{playerB.mmr}] are matched.");
        Program.matchWatingClients.Remove(playerA.client);
        Program.matchWatingClients.Remove(playerB.client);

        Console.WriteLine($"[{playerA.mmr}] WinPoint: {playerA.winPoint}, DefeatPoint: {playerA.defeatPoint}");
        playerA.client.matchInfo.winPoint = playerA.winPoint;
        playerA.client.matchInfo.defeatPoint = playerA.defeatPoint;

        Console.WriteLine($"[{playerB.mmr}] WinPoint: {playerB.winPoint}, DefeatPoint: {playerB.defeatPoint}");
        playerB.client.matchInfo.winPoint = playerB.winPoint;
        playerB.client.matchInfo.defeatPoint = playerB.defeatPoint;
    }

    private static List<Tuple<Player, Player>> FindBestMatching(Player[] players, int mmrLimit, out List<Player> unmatchedPlayers) {
        int n = players.Length;

        var sortedPlayers = players.OrderBy(p => p.mmr).ToList();
        var matchedPairs = new List<Tuple<Player, Player>>();
        var unmatchedIndices = new HashSet<int>();

        for(int i = 0; i < n; i += 2) {
            if(i + 1 < n && Math.Abs(sortedPlayers[i].mmr - sortedPlayers[i + 1].mmr) <= mmrLimit) {
                matchedPairs.Add(Tuple.Create(sortedPlayers[i], sortedPlayers[i + 1]));
            }
            else {
                unmatchedIndices.Add(i);
                if(i + 1 < n) {
                    unmatchedIndices.Add(i + 1);
                }
            }
        }

        unmatchedPlayers = unmatchedIndices.Select(index => players[index]).ToList();
        return matchedPairs;
    }
}