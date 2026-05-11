import { Avatar } from '../ui/Avatar'
import type { Player } from '../../types/game.types'

interface Props {
  players: Player[]
  myUsername: string
}

export const Leaderboard = ({ players, myUsername }: Props) => (
  <div className="flex flex-col gap-2">
    {players.map((player, i) => (
      <div
        key={player.username}
        className={`flex items-center gap-3 px-4 py-3 rounded-xl transition-all
          ${player.username === myUsername
            ? 'bg-indigo-600/30 border border-indigo-500/50'
            : 'bg-white/5 border border-white/10'
          }`}
      >
        <span className="text-white/40 font-mono text-sm w-5 text-center">
          {i + 1}
        </span>
        <Avatar username={player.username} color={player.avatarColor} size="sm" />
        <span className="flex-1 text-white font-medium text-sm">
          {player.username}
          {player.username === myUsername && (
            <span className="text-indigo-400 text-xs ml-2">(ти)</span>
          )}
        </span>
        {player.hasAnswered && (
          <span className="text-green-400 text-xs">✓</span>
        )}
        <span className="text-white font-bold text-sm">
          {player.score.toLocaleString()}
        </span>
      </div>
    ))}
  </div>
)