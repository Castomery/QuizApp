import { AiHostBubble } from '../components/game/AiHostBubble'

export const GeneratingPage = () => (
  <div className="min-h-screen bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 flex items-center justify-center p-4">
    <div className="max-w-sm w-full">
      <AiHostBubble message="" loading />
      <p className="text-white/50 text-sm text-center mt-4">AI готує питання...</p>
    </div>
  </div>
)