import { useEffect, useRef, useState } from 'react'

interface Props {
  durationSec: number
  onTimeUp: () => void
  running: boolean
}

export const TimerBar = ({ durationSec, onTimeUp, running }: Props) => {
  const [timeLeft, setTimeLeft] = useState(durationSec)
  const onTimeUpRef = useRef(onTimeUp)
  const startTimeRef = useRef<number | null>(null)

  useEffect(() => {
    onTimeUpRef.current = onTimeUp
  }, [onTimeUp])

  useEffect(() => {
    if (!running) return

    startTimeRef.current = Date.now()

    const timer = setInterval(() => {
      const elapsed = Math.floor((Date.now() - startTimeRef.current!) / 1000)
      const remaining = durationSec - elapsed

      if (remaining <= 0) {
        clearInterval(timer)
        setTimeLeft(0)
        onTimeUpRef.current()
        return
      }

      setTimeLeft(remaining)
    }, 200)

    return () => clearInterval(timer)
  }, [durationSec, running])

  const pct = (timeLeft / durationSec) * 100
  const color = pct > 50 ? 'bg-green-500' : pct > 25 ? 'bg-yellow-500' : 'bg-red-500'

  return (
    <div className="flex items-center gap-3">
      <span className="text-white font-bold text-lg w-8 text-center">
        {timeLeft}
      </span>
      <div className="flex-1 h-3 bg-white/10 rounded-full overflow-hidden">
        <div
          className={`h-full rounded-full transition-all duration-200 ${color}`}
          style={{ width: `${pct}%` }}
        />
      </div>
    </div>
  )
}