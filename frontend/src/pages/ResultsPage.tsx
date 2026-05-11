import { useGame } from '../hooks/useGame'
import { Avatar } from '../components/ui/Avatar'
import { AiHostBubble } from '../components/game/AiHostBubble'
import { Button } from '../components/ui/Button'

export const ResultsPage = () => {
  const { players, aiSummary, username, leaveGame } = useGame()
  const medals = ['🥇', '🥈', '🥉']

  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 flex items-center justify-center p-4">
      <div className="w-full max-w-md">

        <h2 className="text-3xl font-bold text-white text-center mb-6">
          🏆 Результати
        </h2>

        {aiSummary && (
          <div className="mb-6">
            <AiHostBubble message={aiSummary} />
          </div>
        )}

        <div className="flex flex-col gap-2 mb-8">
          {players.map((player, i) => (
            <div
              key={player.username}
              className={`flex items-center gap-3 px-4 py-3 rounded-xl border
                ${player.username === username
                  ? 'bg-indigo-600/30 border-indigo-500/50'
                  : 'bg-white/5 border-white/10'
                }`}
            >
              <span className="text-xl w-8 text-center">
                {medals[i] ?? `${i + 1}.`}
              </span>
              <Avatar username={player.username} color={player.avatarColor} size="sm" />
              <span className="flex-1 text-white font-medium text-sm">
                {player.username}
                {player.username === username && (
                  <span className="text-indigo-400 text-xs ml-2">(ти)</span>
                )}
              </span>
              <span className="text-white font-bold">
                {player.score.toLocaleString()}
              </span>
            </div>
          ))}
        </div>

        <Button
          label="Грати знову"
          onClick={leaveGame}
          fullWidth
        />

      </div>
    </div>
  )
}