import { useEffect, useRef, useState } from 'react'

interface Props {
  message: string
  loading?: boolean
}

export const AiHostBubble = ({ message, loading }: Props) => {
  const [displayed, setDisplayed] = useState('')
  const indexRef = useRef(0)

  useEffect(() => {
    if (!message) return

    // скидаємо через ref — без зайвого рендеру
    indexRef.current = 0

    const interval = setInterval(() => {
      indexRef.current += 1
      setDisplayed(message.slice(0, indexRef.current))

      if (indexRef.current >= message.length) {
        clearInterval(interval)
      }
    }, 25)

    return () => clearInterval(interval)
  }, [message])

  return (
    <div className="flex items-start gap-3">
      <div className="w-10 h-10 rounded-full bg-indigo-600 flex items-center justify-center text-lg flex-shrink-0">
        🤖
      </div>
      <div className="bg-white/10 border border-white/20 rounded-2xl rounded-tl-none px-4 py-3 max-w-md">
        {loading ? (
          <div className="flex gap-1 items-center h-5">
            <span className="w-2 h-2 bg-white/40 rounded-full animate-bounce [animation-delay:0ms]" />
            <span className="w-2 h-2 bg-white/40 rounded-full animate-bounce [animation-delay:150ms]" />
            <span className="w-2 h-2 bg-white/40 rounded-full animate-bounce [animation-delay:300ms]" />
          </div>
        ) : (
          <p className="text-white text-sm leading-relaxed">{displayed}</p>
        )}
      </div>
    </div>
  )
}