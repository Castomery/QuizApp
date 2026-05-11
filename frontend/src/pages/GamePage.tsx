import { useRef, useEffect } from 'react'
import { useGame } from '../hooks/useGame'
import { QuestionCard } from '../components/game/QuestionCard'
import { TimerBar } from '../components/game/TimerBar'
import { Leaderboard } from '../components/game/Leaderboard'
import { AiHostBubble } from '../components/game/AiHostBubble'
import type { Question } from '../types/game.types'

interface RoundProps {
  question: Question
  round: number
  totalRounds: number
  onAnswer: (index: number, timeMs: number) => void
}

const RoundView = ({ question, round, totalRounds, onAnswer }: RoundProps) => {
  const answeredRef = useRef(false)
  const startTimeRef = useRef(0)

  useEffect(() => {
    startTimeRef.current = Date.now()
  }, [])

  const handleAnswer = (index: number) => {
    if (answeredRef.current) return
    answeredRef.current = true
    onAnswer(index, Date.now() - startTimeRef.current)
  }

  return (
    <div className="flex flex-col gap-4">
      <TimerBar
        durationSec={question.timeoutSec}
        running={true}
        onTimeUp={() => handleAnswer(-1)}
      />
      <QuestionCard
        question={question}
        onAnswer={handleAnswer}
        answered={false}
      />
      <p className="text-white/40 text-xs text-center">
        Раунд {round} / {totalRounds}
      </p>
    </div>
  )
}

export const GamePage = () => {
  const {
    currentQuestion, currentRound, totalRounds,
    players, myScore, myStreak, username,
    lastAnswerCorrect, lastPoints, aiComment,
    phase, submitAnswer,
  } = useGame()

  const handleAnswer = async (index: number, timeMs: number) => {
    await submitAnswer(index, timeMs)
  }

  if (!currentQuestion) return null

  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 p-4">
      <div className="max-w-2xl mx-auto">

        {/* Хедер */}
        <div className="flex justify-between items-center mb-4">
          <span className="text-white/50 text-sm">
            Раунд {currentRound}/{totalRounds}
          </span>
          <div className="flex items-center gap-2">
            {myStreak > 1 && (
              <span className="text-orange-400 text-sm">🔥 ×{myStreak}</span>
            )}
            <span className="text-white font-bold">{myScore.toLocaleString()}</span>
          </div>
        </div>

        {/* AI коментар після раунду */}
        {phase === 'roundEnd' && aiComment && (
          <div className="mb-6">
            <AiHostBubble message={aiComment} />
          </div>
        )}

        {/* Результат відповіді */}
        {lastAnswerCorrect !== null && (
          <div className={`mb-4 px-4 py-3 rounded-xl text-sm font-medium ${
            lastAnswerCorrect
              ? 'bg-green-600/20 border border-green-500/50 text-green-400'
              : 'bg-red-600/20 border border-red-500/50 text-red-400'
          }`}>
            {lastAnswerCorrect
              ? `✅ Правильно! +${lastPoints.toLocaleString()} очок`
              : '❌ Неправильно'}
          </div>
        )}

        {/* Питання — key скидає RoundView при кожному новому питанні */}
        {phase === 'playing' && (
          <div className="bg-white/5 border border-white/10 rounded-2xl p-6 mb-6">
            <RoundView
              key={currentRound}
              question={currentQuestion}
              round={currentRound}
              totalRounds={totalRounds}
              onAnswer={handleAnswer}
            />
          </div>
        )}

        {/* Лідерборд */}
        <div className="bg-white/5 border border-white/10 rounded-2xl p-4">
          <p className="text-white/40 text-xs mb-3">Рейтинг</p>
          <Leaderboard players={players} myUsername={username} />
        </div>

      </div>
    </div>
  )
}