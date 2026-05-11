import type { Question } from '../../types/game.types'

interface Props {
  question: Question
  onAnswer: (index: number) => void
  answered: boolean
  correctIndex?: number
}

export const QuestionCard = ({ question, onAnswer, answered, correctIndex }: Props) => {
  const letters = ['A', 'B', 'C', 'D']

  const getStyle = (i: number) => {
    if (!answered) return 'bg-white/10 hover:bg-white/20 border-white/20 cursor-pointer'
    if (i === correctIndex) return 'bg-green-600/40 border-green-500 cursor-default'
    return 'bg-white/5 border-white/10 cursor-default opacity-50'
  }

  return (
    <div className="flex flex-col gap-4">
      <p className="text-white text-xl font-semibold leading-snug">
        {question.text}
      </p>
      <div className="grid grid-cols-1 gap-3">
        {question.options.map((option, i) => (
          <button
            key={i}
            onClick={() => !answered && onAnswer(i)}
            className={`flex items-center gap-3 px-4 py-3 rounded-xl border transition-all duration-200 text-left ${getStyle(i)}`}
          >
            <span className="w-8 h-8 rounded-lg bg-white/10 flex items-center justify-center text-white/60 font-bold text-sm flex-shrink-0">
              {letters[i]}
            </span>
            <span className="text-white text-sm">{option}</span>
          </button>
        ))}
      </div>
    </div>
  )
}