import { useGame } from '../hooks/useGame'
import { Avatar } from '../components/ui/Avatar'
import { Button } from '../components/ui/Button'

export const WaitingPage = () => {
  const { players, roomCode, topic, difficulty, username, startGame } = useGame()
  const isHost = players[0]?.username === username

  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 flex items-center justify-center p-4">
      <div className="w-full max-w-md">

        <div className="text-center mb-8">
          <p className="text-white/40 text-sm mb-1">Код кімнати</p>
          <h2 className="text-4xl font-bold text-white tracking-widest font-mono">
            {roomCode}
          </h2>
          <p className="text-white/50 text-sm mt-2">
            {topic} · Складність {difficulty}/5
          </p>
        </div>

        <div className="bg-white/5 border border-white/10 rounded-2xl p-4 mb-6">
          <p className="text-white/40 text-xs mb-3">
            Гравці — {players.length}
          </p>
          <div className="flex flex-col gap-2">
            {players.map((player, i) => (
              <div key={player.username} className="flex items-center gap-3">
                <Avatar username={player.username} color={player.avatarColor} size="sm" />
                <span className="text-white text-sm">{player.username}</span>
                {i === 0 && (
                  <span className="text-yellow-400 text-xs ml-auto">👑 хост</span>
                )}
              </div>
            ))}
          </div>
        </div>

        {isHost ? (
          <Button
            label={`Почати гру (${players.length} гравців)`}
            onClick={startGame}
            fullWidth
            disabled={players.length < 1}
          />
        ) : (
          <p className="text-center text-white/40 text-sm">
            Чекаємо поки хост розпочне гру...
          </p>
        )}

      </div>
    </div>
  )
}