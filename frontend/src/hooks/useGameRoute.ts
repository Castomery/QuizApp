import { useEffect } from 'react'
import { useNavigate, useLocation } from 'react-router'
import { useGameStore } from '../store/gameStore'

const phaseToPath: Record<string, string> = {
  lobby: '/',
  waiting: '/waiting',
  generating: '/generating',
  playing: '/game',
  roundEnd: '/game',
  results: '/results',
}

export const useGameRoute = () => {
  const phase = useGameStore(s => s.phase)
  const navigate = useNavigate()
  const location = useLocation()

  useEffect(() => {
    const targetPath = phaseToPath[phase]
    if (targetPath && location.pathname !== targetPath) {
      navigate(targetPath, { replace: true })
    }
  }, [phase, navigate, location.pathname])
}